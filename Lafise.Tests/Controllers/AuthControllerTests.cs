using Lafise.API.controllers;
using Lafise.API.services.Auth;
using Lafise.API.services.Auth.Dto;
using Lafise.API.utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace Lafise.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test1234!"
        };

        var expectedResult = new LoginResultDto
        {
            UserId = "user-123",
            UserName = "Test User",
            UserEmail = "test@example.com",
            AuthToken = "token-123",
            RefreshToken = "refresh-token-123",
            AuthTokenExpiration = DateTime.UtcNow.AddHours(1)
        };

        _mockAuthService
            .Setup(x => x.Login(loginDto.Email, loginDto.Password))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedResult);
        
        _mockAuthService.Verify(x => x.Login(loginDto.Email, loginDto.Password), Times.Once);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsLafiseException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        _mockAuthService
            .Setup(x => x.Login(loginDto.Email, loginDto.Password))
            .ThrowsAsync(new LafiseException(401, "Invalid password."));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().NotBeNull();
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(401);
        
        _mockAuthService.Verify(x => x.Login(loginDto.Email, loginDto.Password), Times.Once);
    }

    [Fact]
    public async Task Login_WithUserNotFound_Returns404()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Test1234!"
        };

        _mockAuthService
            .Setup(x => x.Login(loginDto.Email, loginDto.Password))
            .ThrowsAsync(new LafiseException(404, "User not found."));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().NotBeNull();
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_Returns400()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "",
            Password = "Test1234!"
        };

        _mockAuthService
            .Setup(x => x.Login(loginDto.Email, loginDto.Password))
            .ThrowsAsync(new LafiseException(400, "Email cannot be empty."));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().NotBeNull();
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Login_WithUnexpectedException_Returns500()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Test1234!"
        };

        _mockAuthService
            .Setup(x => x.Login(loginDto.Email, loginDto.Password))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().NotBeNull();
        var statusResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }
}

