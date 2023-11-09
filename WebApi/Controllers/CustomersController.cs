using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RocketStoreApi.Managers;
using RocketStoreApi.Models;
using RocketStoreApi.PositionStack;

namespace RocketStoreApi.Controllers
{
    /// <summary>
    /// Defines the customers controller.
    /// This controller provides actions on customers.
    /// </summary>
    [ControllerName("Customers")]
    [ApiController]
    public partial class CustomersController : ControllerBase
    {
        // Ignore Spelling: api

        #region Public Methods

        /// <summary>
        /// Creates the specified customer.
        /// </summary>
        /// <param name="customer">The customer that should be created.</param>
        /// <returns>
        /// The new customer identifier.
        /// </returns>
        [HttpPost("api/customers")]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateCustomerAsync(Models.Customer customer)
        {
            Result<Guid> result = await this.HttpContext.RequestServices.GetRequiredService<ICustomersManager>()
                .CreateCustomerAsync(customer).ConfigureAwait(false);

            if (result.FailedWith(ErrorCodes.CustomerAlreadyExists))
            {
                return this.Conflict(
                    new ProblemDetails()
                    {
                        Status = (int)HttpStatusCode.Conflict,
                        Title = result.ErrorCode,
                        Detail = result.ErrorDescription
                    });
            }
            else if (result.Failed)
            {
                return this.BadRequest(
                    new ProblemDetails()
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = result.ErrorCode,
                        Detail = result.ErrorDescription
                    });
            }

            return this.Created(
                this.GetUri("customers", result.Value),
                result.Value);
        }

        /// <summary>
        /// Retrieves all customers.
        /// </summary>
        /// <param name="name">Filtro para o nome.</param>
        /// <param name="emailAddress">filtro para o endereço de email.</param>
        /// <returns>
        /// return a list of custumers.
        /// </returns>
        [HttpGet("api/customers")]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Collection<CustomerLista>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomersAsync([FromQuery] string name = null, [FromQuery] string emailAddress = null)
        {
            Collection<CustomerLista> customers = await this.HttpContext.RequestServices.GetRequiredService<ICustomersManager>()
                .GetCustomersAsync(name, emailAddress).ConfigureAwait(false);

            if (customers.Count == 0)
            {
                return this.NotFound(
                    new ProblemDetails()
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = "Customers not Found"
                    });
            }

            return this.Ok(customers);
        }

        /// <summary>
        /// Retrieves A customer by id.
        /// </summary>
        /// <param name="id">recebemos o id do customer que qwueremos.</param>
        /// <returns>
        /// return A customer.
        /// </returns>
        [HttpGet("api/customers/{id}")]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CustomerByID), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCustomersByIDAsync(string id)
        {
            Result<CustomerByID> customers = await this.HttpContext.RequestServices.GetRequiredService<ICustomersManager>()
                .GetCustomersByIDAsync(id).ConfigureAwait(false);

            RestService rs = new RestService();
            Forward f = new Forward();

            if (customers.FailedWith(ErrorCodes.InvalidID))
            {
                return this.BadRequest(
                    new ProblemDetails()
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = customers.ErrorCode,
                        Detail = customers.ErrorDescription
                    });
            }
            else if (customers.Failed)
            {
                return this.NotFound(
                    new ProblemDetails()
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = customers.ErrorCode,
                        Detail = customers.ErrorDescription
                    });
            }

            f = await rs.GetForwardAsync(customers.Value.City).ConfigureAwait(false);

            if (f != null && f.Longitude != 0)
            {
                customers.Value.Forward = f;
            }

            return this.Ok(customers);
        }

        /// <summary>
        /// Delete a customer by id.
        /// </summary>
        /// <param name="id"> Id of the wanted customer.</param>
        /// <returns>
        /// return a customer.
        /// </returns>
        [HttpDelete("api/customers/{id}")]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCustomersByIDAsync(string id)
        {
            Result<bool> result = await this.HttpContext.RequestServices.GetRequiredService<ICustomersManager>()
                                    .DeleteCustomersByIDAsync(id).ConfigureAwait(false);

            if (result.FailedWith(ErrorCodes.InvalidID))
            {
                return this.BadRequest(
                    new ProblemDetails()
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = result.ErrorCode,
                        Detail = result.ErrorDescription
                    });
            }
            else if (result.Failed)
            {
                return this.NotFound(
                    new ProblemDetails()
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = result.ErrorCode,
                        Detail = result.ErrorDescription
                    });
            }

            return this.Ok();
        }
        #endregion

        #region Private Methods

        private Uri GetUri(params object[] parameters)
        {
            string result = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            foreach (object pathParam in parameters)
            {
                result = $"{result}/{pathParam}";
            }

            return new Uri(result);
        }

        #endregion
    }
}
