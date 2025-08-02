using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LoveForTennis.Infrastructure.Data;
using LoveForTennis.Infrastructure.Extensions;
using Xunit;

namespace LoveForTennis.Web.Tests;

public class DatabaseConfigurationTests
{
    [Fact]
    public void AddDatabase_WithoutConnectionString_UsesInMemory()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddDatabase(configuration, "Test");
        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Assert
        Assert.True(context.Database.IsInMemory());
    }

    [Fact]
    public void AddDatabase_WithEmptyConnectionString_UsesInMemory()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ""
            })
            .Build();

        // Act
        services.AddDatabase(configuration, "Test");
        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Assert
        Assert.True(context.Database.IsInMemory());
    }

    [Fact]
    public void AddDatabase_WithConnectionString_ConfiguresContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=true;"
            })
            .Build();

        // Act
        services.AddDatabase(configuration, "Test");
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert - Just verify the service is registered properly
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.NotNull(context);
        Assert.False(context.Database.IsInMemory());
    }
}