using Pwnlabs.Application.Abstractions;
using Pwnlabs.Domain.Entities;
using NSubstitute;
using Pwnlabs.Application.Rooms;
using Pwnlabs.Application.Common.Exceptions;

namespace Pwnlabs.Application.Tests.Rooms;

public class RoomServiceTests
{
    private readonly IRoomRepository _roomRepository = Substitute.For<IRoomRepository>();
    private readonly RoomService _sut;

    public RoomServiceTests()
    {
        _sut = new RoomService(_roomRepository);
    }

    [Fact]
    public async Task GetBySlugAsync_UnknownSlug_ThrowsNotFoundException()
    {
        // Arrange
        _roomRepository.GetPublishedBySlugAsync("does-not-exist").Returns((Room?)null);

        // Act + Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetBySlugAsync("does-not-exist"));
    }
}