using Lafise.API.controllers;
using Lafise.API.services.Accounts;
using Lafise.API.services.Accounts.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace Lafise.Tests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _mockAccountService = new Mock<IAccountService>();
        _controller = new AccountController(_mockAccountService.Object);
    }

    [Fact]
    public async Task GetAllAccounts_ReturnsOkResultWithAccounts()
    {
        // Arrange
        var expectedAccounts = new List<AccountDetailsDto>
        {
            new AccountDetailsDto
            {
                AccountNumber = "1000000",
                AccountType = "Checking",
                CurrentBalance = 1000.00m,
                Owner = new ClientInfoDto
                {
                    Name = "John",
                    LastName = "Doe"
                }
            },
            new AccountDetailsDto
            {
                AccountNumber = "1000001",
                AccountType = "Savings",
                CurrentBalance = 5000.00m,
                Owner = new ClientInfoDto
                {
                    Name = "Jane",
                    LastName = "Smith"
                }
            }
        };

        _mockAccountService
            .Setup(x => x.GetAllAccounts())
            .ReturnsAsync(expectedAccounts);

        // Act
        var result = await _controller.GetAllAccounts();

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var accounts = okResult.Value.Should().BeAssignableTo<List<AccountDetailsDto>>().Subject;
        accounts.Should().HaveCount(2);
        accounts.Should().BeEquivalentTo(expectedAccounts);
        
        _mockAccountService.Verify(x => x.GetAllAccounts(), Times.Once);
    }

    [Fact]
    public async Task GetAllAccounts_WhenServiceThrowsException_Returns500()
    {
        // Arrange
        _mockAccountService
            .Setup(x => x.GetAllAccounts())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllAccounts();

        // Assert
        result.Should().NotBeNull();
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetAllAccounts_WhenNoAccountsExist_ReturnsEmptyList()
    {
        // Arrange
        var emptyList = new List<AccountDetailsDto>();

        _mockAccountService
            .Setup(x => x.GetAllAccounts())
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAllAccounts();

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var accounts = okResult.Value.Should().BeAssignableTo<List<AccountDetailsDto>>().Subject;
        accounts.Should().BeEmpty();
    }
}

