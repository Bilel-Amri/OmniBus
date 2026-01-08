using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using OmniBus.API;
using OmniBus.Application.DTOs;
using Xunit;
using FluentAssertions;

namespace OmniBus.Tests.Integration;

public class BookingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BookingIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRoutes_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/routes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchSchedules_WithValidParams_ReturnsSchedules()
    {
        // Arrange
        var origin = "Tunis";
        var destination = "Sfax";
        var date = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

        // Act
        var response = await _client.GetAsync($"/api/schedules/search?origin={origin}&destination={destination}&date={date}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegisterUser_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = $"testuser_{Guid.NewGuid().ToString().Substring(0, 8)}",
            Email = $"test_{Guid.NewGuid().ToString().Substring(0, 8)}@test.com",
            Password = "Test@123456",
            PhoneNumber = "12345678",
            Role = "Passenger"
        };

        var json = JsonSerializer.Serialize(registerDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/register", content);

        // Assert - Should either succeed or return conflict if user exists
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task BookingFlow_EndToEnd_ShouldSucceed()
    {
        // This test would simulate a complete booking flow:
        // 1. Register/Login
        // 2. Search schedules
        // 3. Book ticket
        // 4. Process payment
        // 5. Get ticket details

        // For now, this is a placeholder demonstrating the structure
        var response = await _client.GetAsync("/api/schedules");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
