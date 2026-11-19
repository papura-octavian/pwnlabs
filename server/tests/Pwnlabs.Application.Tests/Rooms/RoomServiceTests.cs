using Pwnlabs.Application.Abstractions;
using Pwnlabs.Domain.Entities;
using NSubstitute;
using Pwnlabs.Application.Rooms;
using Pwnlabs.Application.Common.Exceptions;
using Pwnlabs.Domain.Enums;

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

    [Fact]
    public async Task GetBySlugAsync_ExistingRoom_ReturnsMappedResponse()
    {
        // Arrange
        var room = new Room
        {
            Slug = "get-pwned",
            Title = "Room101",
            Description = "reverse connection tcp exploit to get a shell",
            Category = RoomCategory.Pwn,
            Difficulty = RoomDifficulty.Easy,
            Points = 18,
            Image = "pwnlabs/get-pwned:latest",
            ContainerPort = 54322,
            IsPublished = true
        };

        _roomRepository.GetPublishedBySlugAsync("get-pwned").Returns(room);

        // Act
        var result = await _sut.GetBySlugAsync("get-pwned");

        // Assert
        Assert.Equal("get-pwned", result.Slug);
        Assert.Equal("Room101", result.Title);
        Assert.Equal("reverse connection tcp exploit to get a shell", result.Description);
        Assert.Equal("Pwn", result.Category);
        Assert.Equal("Easy", result.Difficulty);
        Assert.Equal(18, result.Points);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedRoomsInRepositoryOrder()
    {
        // Arrange
        var room1 = new Room
        {
            Slug = "get-pwned",
            Title = "Room101",
            Description = "reverse connection tcp exploit to get a shell",
            Category = RoomCategory.Pwn,
            Difficulty = RoomDifficulty.Easy,
            Points = 18,
            Image = "pwnlabs/get-pwned:latest",
            ContainerPort = 54322,
            IsPublished = true
        };

        var room2 = new Room
        {
            Slug = "basic-crypto",
            Title = "Room30",
            Description = "Use cryptographic tools to decrypt the message and get the flag",
            Category = RoomCategory.Crypto,
            Difficulty = RoomDifficulty.Easy,
            Points = 12,
            Image = "pwnlabs/basic-crypto:latest",
            ContainerPort = 54301,
            IsPublished = true
        };

        _roomRepository.GetPublishedAsync().Returns(new List<Room> { room1, room2 });

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("get-pwned", result[0].Slug);
        Assert.Equal("basic-crypto", result[1].Slug);
    }
}