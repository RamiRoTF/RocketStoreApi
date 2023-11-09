using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RocketStoreApi.Controllers;
using RocketStoreApi.Managers;
using RocketStoreApi.Models;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RocketStoreApi.Tests
{
    /// <summary>
    /// Provides integration tests for the <see cref="CustomersController"/> type.
    /// </summary>
    public partial class CustomersControllerTests : TestsBase, IClassFixture<CustomersFixture>
    {
        // Ignore Spelling: api

        #region Fields

        private readonly CustomersFixture fixture;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersControllerTests"/> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        public CustomersControllerTests(CustomersFixture fixture)
        {
            this.fixture = fixture;
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(Customer)"/> method
        /// to ensure that it requires name and email.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateRequiresNameAndEmailAsync()
        {
            // Arrange

            IDictionary<string, string[]> expectedErrors = new Dictionary<string, string[]>
            {
                { "Name", new string[] { "The Name field is required." } },
                { "EmailAddress", new string[] { "The Email field is required." } },
                { "City", new string[] { "The City field is required." } }
            };
            
            Customer customer = new Customer();

            // Act

            HttpResponseMessage httpResponse = await this.fixture.PostAsync("api/customers", customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            ValidationProblemDetails error = await this.GetResponseContentAsync<ValidationProblemDetails>(httpResponse).ConfigureAwait(false);
            error.Should().NotBeNull();
            error.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(Customer)"/> method
        /// to ensure that it requires a valid email address.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateRequiresValidEmailAsync()
        {
            // Arrange

            IDictionary<string, string[]> expectedErrors = new Dictionary<string, string[]>
            {
                { "EmailAddress", new string[] { "The Email field is not a valid e-mail address." } }
            };

            Customer customer = new Customer()
            {
                Name = "A customer",
                EmailAddress = "An invalid email",
                City = "A city"
            };

            // Act

            HttpResponseMessage httpResponse = await this.fixture.PostAsync("api/customers", customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            ValidationProblemDetails error = await this.GetResponseContentAsync<ValidationProblemDetails>(httpResponse).ConfigureAwait(false);
            error.Should().NotBeNull();
            error.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(Customer)"/> method
        /// to ensure that it requires a valid VAT number.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateRequiresValidVatNumberAsync()
        {
            // Arrange

            IDictionary<string, string[]> expectedErrors = new Dictionary<string, string[]>
            {
                { "VatNumber", new string[] { "The field VAT Number must match the regular expression '^[0-9]{9}$'." } }
            };

            Customer customer = new Customer()
            {
                Name = "A customer",
                EmailAddress = "customer@server.pt",
                VatNumber = "1234567899",
                City = "A city"
            };

            // Act

            HttpResponseMessage httpResponse = await this.fixture.PostAsync("api/customers", customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            ValidationProblemDetails error = await this.GetResponseContentAsync<ValidationProblemDetails>(httpResponse).ConfigureAwait(false);
            error.Should().NotBeNull();
            error.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(Customer)"/> method
        /// to ensure that it requires a unique email address.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateRequiresUniqueEmailAsync()
        {
            // Arrange

            Customer customer1 = new Customer()
            {
                Name = "A customer",
                EmailAddress = "customer@server.pt",
                City = "A city"
            };

            Customer customer2 = new Customer()
            {
                Name = "Another customer",
                EmailAddress = "customer@server.pt",
                City = "A city"
            };

            // Act

            HttpResponseMessage httpResponse1 = await this.fixture.PostAsync("api/customers", customer1).ConfigureAwait(false);

            HttpResponseMessage httpResponse2 = await this.fixture.PostAsync("api/customers", customer2).ConfigureAwait(false);

            // Assert

            httpResponse1.StatusCode.Should().Be(HttpStatusCode.Created);
            
            httpResponse2.StatusCode.Should().Be(HttpStatusCode.Conflict);

            ProblemDetails error = await this.GetResponseContentAsync<ProblemDetails>(httpResponse2).ConfigureAwait(false);
            error.Should().NotBeNull();
            error.Title.Should().Be(ErrorCodes.CustomerAlreadyExists);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.CreateCustomerAsync(Customer)"/> method
        /// to ensure that it requires a valid VAT number.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task CreateSucceedsAsync()
        {
            // Arrange

            Customer customer = new Customer()
            {
                Name = "My customer",
                EmailAddress = "mycustomer@server.pt",
                City = "A city"
            };

            // Act

            HttpResponseMessage httpResponse = await this.fixture.PostAsync("api/customers", customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            Guid? id = await this.GetResponseContentAsync<Guid?>(httpResponse).ConfigureAwait(false);
            id.Should().NotBeNull();

            httpResponse.Headers.Location.Should().NotBeNull();
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.GetCustomersAsync(string, string)"/> method
        /// to ensure that it gets all customers.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task ListaCustomerSuccedsAsync()
        {
            // Arrange

            Customer customer = new Customer()
            { 
                Name = "My customer",
                EmailAddress = "mycustomer@server.pt",
                City = "A city"
            };

            HttpResponseMessage httpResponse1 = await this.fixture.PostAsync("api/customers", customer).ConfigureAwait(false);

            customer = new Customer()
            {
                Name = "My customer",
                EmailAddress = "mycustomer@server.pt",
                City = "A city"
            };

            httpResponse1 = await this.fixture.PostAsync("api/customers", customer).ConfigureAwait(false);

            // Act

            HttpResponseMessage httpResponse = await this.fixture.GetAsync("api/customers", customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.GetCustomersByIDAsync(string)"/> method
        /// to ensure that it gets the right customer.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task GetCustomerByIDSuccedsAsync()
        {
            // Arrange

            Customer customer = new Customer()
            {
                Name = "My customer",
                EmailAddress = "mycustomer@server.pt",
                City = "A city"
            };

            HttpResponseMessage httpResponse1 = await this.fixture.PostAsync("api/customers", customer).ConfigureAwait(false);
            Guid? id = await this.GetResponseContentAsync<Guid?>(httpResponse1).ConfigureAwait(false);

            // Act

            HttpResponseMessage httpResponse = await this.fixture.GetAsync("api/customers/" + id, customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.DeleteCustomersByIDAsync(string)"/> method
        /// to ensure that it delete the customer wanted.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task DeleteByIDSuccedsAsync()
        {
            // Arrange

            Customer customer = new Customer()
            {
                Name = "My customer",
                EmailAddress = "mycustomer@server.pt",
                City = "A city"
            };

            HttpResponseMessage httpResponse1 = await this.fixture.PostAsync("api/customers", customer).ConfigureAwait(false);
            Guid? id = await this.GetResponseContentAsync<Guid?>(httpResponse1).ConfigureAwait(false);

            // Act

            HttpResponseMessage httpResponse = await this.fixture.DeleteAsync("api/customers/" + id.ToString(), customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.DeleteCustomersByIDAsync(string)"/> method
        /// to ensure that it return the error NOT FOUND when it doesnt find the customer.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task DeleteByIDNotFoundAsync()
        {
            // Arrange
            Customer customer = new Customer();
            Guid id = Guid.NewGuid();

            // Act

            HttpResponseMessage httpResponse = await this.fixture.DeleteAsync("api/customers/" + id.ToString(), customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            ProblemDetails error = await this.GetResponseContentAsync<ProblemDetails>(httpResponse).ConfigureAwait(false);
            error.Should().NotBeNull();
            error.Title.Should().Be(ErrorCodes.CustomerDoesntExists);
        }

        /// <summary>
        /// Tests the <see cref="CustomersController.GetCustomersByIDAsync(string)"/> method
        /// to ensure that it returns Not Found when it cant find the customer by ID.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task GetCustomerByIDNotFoundAsync()
        {
            // Arrange
            Customer customer = new Customer();
            Guid id = Guid.NewGuid();

            // Act

            HttpResponseMessage httpResponse = await this.fixture.GetAsync("api/customers/" + id.ToString(), customer).ConfigureAwait(false);

            // Assert

            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            ProblemDetails error = await this.GetResponseContentAsync<ProblemDetails>(httpResponse).ConfigureAwait(false);
            error.Should().NotBeNull();
            error.Title.Should().Be(ErrorCodes.CustomerDoesntExists);
        }
        #endregion
    }
}
