using Lafise.API.controllers;
using Lafise.API.data.model;
using Lafise.API.services.clients;
using Lafise.API.services.Clients.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace Lafise.Tests.Controllers;

public class ClientControllerTests
{
    private readonly Mock<IClientService> _mockClientService;
    private readonly ClientController _controller;

    public ClientControllerTests()
    {
        _mockClientService = new Mock<IClientService>();
        _controller = new ClientController(_mockClientService.Object);
    }

    [Fact]
    public async Task GetAllClients_ReturnsOkResultWithClients()
    {
        // Arrange
        var expectedClients = new List<Client>
        {
            new Client
            {
                Id = "client-1",
                Name = "John",
                LastName = "Doe",
                Email = "john@example.com",
                TaxId = "123456789",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Gender = "M",
                Income = 50000m
            },
            new Client
            {
                Id = "client-2",
                Name = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                TaxId = "987654321",
                DateOfBirth = DateTime.UtcNow.AddYears(-25),
                Gender = "F",
                Income = 60000m
            }
        };

        _mockClientService
            .Setup(x => x.GetAllClients())
            .ReturnsAsync(expectedClients);

        // Act
        var result = await _controller.GetAllClients();

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var clients = okResult.Value.Should().BeAssignableTo<List<Client>>().Subject;
        clients.Should().HaveCount(2);
        clients.Should().BeEquivalentTo(expectedClients);
        
        _mockClientService.Verify(x => x.GetAllClients(), Times.Once);
    }

    [Fact]
    public async Task GetAllClients_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        _mockClientService
            .Setup(x => x.GetAllClients())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllClients();

        // Assert
        result.Should().NotBeNull();
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateClient_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var createClientDto = new CreateClientDto
        {
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com",
            TaxId = "123456789",
            Password = "Test1234!",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "M",
            Income = 50000m,
            AccountType = "Savings"
        };

        var expectedClient = new ClientResponseDto
        {
            Id = "client-123",
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        _mockClientService
            .Setup(x => x.CreateClient(createClientDto))
            .ReturnsAsync(expectedClient);

        // Act
        var result = await _controller.CreateClient(createClientDto);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedClient);
        
        _mockClientService.Verify(x => x.CreateClient(createClientDto), Times.Once);
    }

    [Fact]
    public async Task CreateClient_WithDuplicateTaxId_ReturnsBadRequest()
    {
        // Arrange
        var createClientDto = new CreateClientDto
        {
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com",
            TaxId = "123456789",
            Password = "Test1234!",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = "M",
            Income = 50000m,
            AccountType = "Savings"
        };

        _mockClientService
            .Setup(x => x.CreateClient(createClientDto))
            .ThrowsAsync(new InvalidOperationException("A client with Tax ID '123456789' already exists."));

        // Act
        var result = await _controller.CreateClient(createClientDto);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be("A client with Tax ID '123456789' already exists.");
    }
}

