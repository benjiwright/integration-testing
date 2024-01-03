using System.Net;
using System.Net.Http.Json;
using Customers.Api.Contracts.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Bogus;
using Customers.Api.Contracts.Responses;

namespace Customers.Api.Tests.Integration;

public class CustomersControllerTests : IClassFixture<WebApplicationFactory<IApiMarker>>, IAsyncLifetime
{
    private readonly HttpClient _client;

    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.FullName, f => f.Person.FullName)
        .RuleFor(x => x.GitHubUsername, f => "benjiwright")
        .RuleFor(x => x.DateOfBirth, f => f.Person.DateOfBirth)
        .RuleFor(x => x.Email, f => f.Person.Email);
    
    private readonly List<Guid> _createdCustomers = new();


    public CustomersControllerTests(WebApplicationFactory<IApiMarker> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateCustomer_ReturnsCreated_WhenCustomerCreated()
    {
        // Arrange
        var customer = _customerGenerator.Generate();

        // Act
        var response = await _client.PostAsJsonAsync("/customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        
        // Cleanup
        _createdCustomers.Add(customerResponse!.Id);
    }


    [Fact(Skip = "have not implemented inserting without calling api")]
    public async Task GetCustomer_ReturnsOk_WhenCustomerFound()
    {
        // Arrange
        var customerId = "0c3feae0-d55e-46ec-b86e-c9d327674817";

        // Act
        var response = await _client.GetAsync($"/customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCustomer_ReturnsNotFound_WhenCustomerNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.GetAsync($"/customers/{customerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem!.Title.Should().Be("Not Found");
        problem.Status.Should().Be(404);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    

    public async Task DisposeAsync()
    {
        foreach (var customerId in _createdCustomers)
        {
            await _client.DeleteAsync($"/customers/{customerId}");
        }
    }
}