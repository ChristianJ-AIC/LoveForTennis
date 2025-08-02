using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;

namespace LoveForTennis.Web.Tests;

public class ErrorPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ErrorPageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ErrorPage_InDevelopmentEnvironment_ShowsTechnicalDetails()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Development")
        ).CreateClient();

        // Act
        var response = await client.GetAsync("/Home/Error");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Development Mode", content);
        Assert.Contains("Request ID:", content);
        Assert.Contains("ASPNETCORE_ENVIRONMENT", content);
    }

    [Fact]
    public async Task ErrorPage_InProductionEnvironment_HidesTechnicalDetails()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Production")
        ).CreateClient();

        // Act
        var response = await client.GetAsync("/Home/Error");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("Development Mode", content);
        Assert.DoesNotContain("Request ID:", content);
        Assert.DoesNotContain("ASPNETCORE_ENVIRONMENT", content);
        Assert.Contains("We apologize for the inconvenience", content);
        Assert.Contains("return to the home page", content);
    }

    [Fact]
    public async Task ErrorPage_InStagingEnvironment_HidesTechnicalDetails()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Staging")
        ).CreateClient();

        // Act
        var response = await client.GetAsync("/Home/Error");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("Development Mode", content);
        Assert.DoesNotContain("Request ID:", content);
        Assert.DoesNotContain("ASPNETCORE_ENVIRONMENT", content);
        Assert.Contains("We apologize for the inconvenience", content);
        Assert.Contains("return to the home page", content);
    }

    [Fact]
    public async Task ErrorPage_Always_ShowsGenericErrorMessage()
    {
        // Arrange
        var developmentClient = _factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Development")
        ).CreateClient();

        var productionClient = _factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Production")
        ).CreateClient();

        // Act
        var devResponse = await developmentClient.GetAsync("/Home/Error");
        var prodResponse = await productionClient.GetAsync("/Home/Error");

        // Assert
        Assert.Equal(HttpStatusCode.OK, devResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, prodResponse.StatusCode);
        
        var devContent = await devResponse.Content.ReadAsStringAsync();
        var prodContent = await prodResponse.Content.ReadAsStringAsync();
        
        // Both should show these generic error messages
        Assert.Contains("Error.", devContent);
        Assert.Contains("An error occurred while processing your request.", devContent);
        Assert.Contains("Error.", prodContent);
        Assert.Contains("An error occurred while processing your request.", prodContent);
    }

    [Fact]
    public async Task NotFoundPage_ReturnsCorrectStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Home/PageNotFound");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task NotFoundPage_ShowsCustom404Content()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Home/PageNotFound");

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("404", content);
        Assert.Contains("This shot went out of bounds!", content);
        Assert.Contains("Return to Home Court", content);
        Assert.Contains("tennis-ball.svg", content);
    }

    [Fact]
    public async Task NonExistentRoute_RedirectsToNotFoundPage()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/NonExistent/Route");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("404", content);
        Assert.Contains("This shot went out of bounds!", content);
    }

    [Fact]
    public async Task NonExistentAction_RedirectsToNotFoundPage()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/Home/NonExistentAction");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("404", content);
        Assert.Contains("This shot went out of bounds!", content);
    }
}