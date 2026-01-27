using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Core.Models;
using System.Linq;
using System.Text;
using System.Net.Http.Json;
using Infrastructure.Models;

namespace Burn_Out.AcceptanceTests.Steps
{
    [Binding]
    public class LoginSteps
    {
        private readonly TestWebApplicationFactory _factory;
        private HttpResponseMessage _response;

        public LoginSteps()
        {
            _factory = new TestWebApplicationFactory();
        }

        [Given("the application is running")]
        public void GivenTheApplicationIsRunning()
        {
            // WebApplicationFactory starts the app when creating a client
        }

        [Given("a user with email \"(.*)\" and password \"(.*)\" exists")]
        public async Task GivenAUserExists(string email, string password)
        {
            using var scope = _factory.Services.CreateScope();
            var providers = scope.ServiceProvider;
            var userManager = providers.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user, password);
            result.Succeeded.Should().BeTrue();
        }

        [When("I attempt to sign in with email \"(.*)\" and password \"(.*)\"")]
        public async Task WhenIAttemptToSignIn(string email, string password)
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var url = $"/auth/signin?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";
            _response = await client.GetAsync(url);
        }

        [Then("the response should be a redirect to \"(.*)\"")]
        public void ThenTheResponseShouldBeARedirectTo(string expected)
        {
            _response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _response.Headers.Location?.ToString().Should().Be(expected);
        }
    }
}
