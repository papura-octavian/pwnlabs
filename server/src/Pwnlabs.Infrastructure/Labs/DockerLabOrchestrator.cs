using System.Diagnostics;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Application.Labs;

namespace Pwnlabs.Infrastructure.Labs;

public class DockerLabOrchestrator : ILabOrchestrator, IDisposable
{
    private readonly DockerClient _docker;
    private readonly LabOptions _options;

    public DockerLabOrchestrator(IOptions<LabOptions> options)
    {
        _options = options.Value;
        _docker = new DockerClientConfiguration().CreateClient();
    }

    public async Task<StartedLab> StartLabAsync(string image, Guid labId, Guid userId)
    {
        var containerPort = $"{_options.ContainerPort}/tcp";

        // 1. "docker create"
        var created = await _docker.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = image,
            Name = $"pwnlabs-{labId}",
            Labels = new Dictionary<string, string>
            {
                ["pwnlabs.managed"] = "true",
                ["pwnlabs.lab-id"] = labId.ToString(),
                ["pwnlabs.user-id"] = userId.ToString()
            },
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                [containerPort] = default
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    [containerPort] = [new PortBinding { HostIP = "127.0.0.1", HostPort = "" }]
                },
                Memory = _options.MemoryLimitMb * 1024L * 1024L
            }
        });

        // 2. "docker start"
        await _docker.Containers.StartContainerAsync(created.ID, new ContainerStartParameters());

        // 3. "docker port" / "docker inspect"
        var inspect = await _docker.Containers.InspectContainerAsync(created.ID);
        var hostPort = int.Parse(inspect.NetworkSettings.Ports[containerPort][0].HostPort);

        return new StartedLab
        {
            ContainerId = created.ID,
            HostPort = hostPort
        };
    }

    public async Task StopLabAsync(string containerId)
    {
        try
        {
            // "docker stop" + "docker rm" intr-un singur apel
            await _docker.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters
            {
                Force = true
            });
        }
        catch (DockerContainerNotFoundException)
        {
            // Already removed (e.g. manually) - the desired state is reached.
        }
    }

    public void Dispose()
    {
        _docker.Dispose();
    }

    public async Task<List<ManagedContainer>> ListManagedContainersAsync()
    {
        var containers = await _docker.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true,
            Filters = new Dictionary<string, IDictionary<string, bool>>
            {
                ["label"] = new Dictionary<string, bool> { ["pwnlabs.managed=true"] = true }
            }
        });

        return containers.Select(c => new ManagedContainer
        {
            ContainerId = c.ID,
            LabId = ParseLabId(c.Labels)
        }).ToList();
    }

    private static Guid? ParseLabId(IDictionary<string, string> labels)
    {
        if (labels.TryGetValue("pwnlabs.lab-id", out var text) && Guid.TryParse(text, out var labId))
        {
            return labId;
        }

        return null;
    }
}