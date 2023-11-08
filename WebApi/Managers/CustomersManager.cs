using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RocketStoreApi.Models;
using RocketStoreApi.Storage;

namespace RocketStoreApi.Managers
{
    /// <summary>
    /// Defines the default implementation of <see cref="ICustomersManager"/>.
    /// </summary>
    /// <seealso cref="ICustomersManager" />
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Created via dependency injection.")]
    internal partial class CustomersManager : ICustomersManager
    {
        #region Private Properties

        private ApplicationDbContext Context
        {
            get;
        }

        private IMapper Mapper
        {
            get;
        }

        private ILogger Logger
        {
            get;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersManager" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public CustomersManager(ApplicationDbContext context, IMapper mapper, ILogger<CustomersManager> logger)
        {
            this.Context = context;
            this.Mapper = mapper;
            this.Logger = logger;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task<Result<Guid>> CreateCustomerAsync(Models.Customer customer, CancellationToken cancellationToken = default)
        {
            customer = customer ?? throw new ArgumentNullException(nameof(customer));

            Entities.Customer entity = this.Mapper.Map<Models.Customer, Entities.Customer>(customer);

            if (this.Context.Customers.Any(i => i.EmailAddress == entity.EmailAddress))
            {
                this.Logger.LogWarning($"A customer with email '{entity.EmailAddress}' already exists.");

                return Result<Guid>.Failure(
                    ErrorCodes.CustomerAlreadyExists,
                    $"A customer with email '{entity.EmailAddress}' already exists.");
            }

            this.Context.Customers.Add(entity);

            await this.Context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            this.Logger.LogInformation($"Customer '{customer.Name}' created successfully.");

            return Result<Guid>.Success(
                new Guid(entity.Id));
        }

        /// <inheritdoc/>
        public async Task<Collection<Models.CustomerLista>> GetCustomersAsync([FromQuery] string name = null, [FromQuery] string emailAddress = null)
        {
            Collection<Models.CustomerLista> result = new Collection<CustomerLista>();
            List<Entities.Customer> customers = new List<Entities.Customer>();
            IQueryable<Entities.Customer> query = this.Context.Customers;

            // Apply filters based on 'name' and 'emailAddress' parameters
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Name.Contains(name, System.StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(emailAddress))
            {
                query = query.Where(c => c.EmailAddress.Contains(emailAddress, System.StringComparison.OrdinalIgnoreCase));
            }

            customers = await query.ToListAsync().ConfigureAwait(false);
            
            foreach (Entities.Customer item in customers)
            {
                result.Add(this.Mapper.Map<Entities.Customer, Models.CustomerLista>(item));
            }

            if (customers.Count == 0)
            {
                this.Logger.LogWarning($"Customers not found.");

                return result;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<Result<Models.CustomerByID>> GetCustomersByIDAsync(string id)
        {
            Models.CustomerByID customer = null;

            if (string.IsNullOrEmpty(id))
            {
                this.Logger.LogWarning($"Id invalido.");

                return Result<Models.CustomerByID>.Failure(
                    ErrorCodes.InvalidID,
                    $"Invalid Id.");
            }

            Entities.Customer entityCustomer = await this.Context.Customers.FindAsync(id).ConfigureAwait(false);

            if (entityCustomer != null)
            {
                customer = this.Mapper.Map<Entities.Customer, Models.CustomerByID>(entityCustomer);
            }
            else
            {
                this.Logger.LogWarning($"This customer doesnt exists.");

                return Result<Models.CustomerByID>.Failure(
                    ErrorCodes.CustomerDontExists,
                    $"Customer Dont Exist.");
            }

            return Result<Models.CustomerByID>.Success(
                        customer);
        }

        /// <inheritdoc/>       
        public async Task<Result<bool>> DeleteCustomersByIDAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                this.Logger.LogWarning($"Id invalido.");

                return Result<bool>.Failure(
                    ErrorCodes.InvalidID,
                    $"Invalid Id.");
            }
            else
            {
                Entities.Customer customer = await this.Context.Customers.FindAsync(id).ConfigureAwait(false);

                if (customer != null)
                {
                    this.Context.Customers.Remove(customer);
                    await this.Context.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    this.Logger.LogWarning($"This customer doesnt exists.");

                    return Result<bool>.Failure(
                        ErrorCodes.CustomerDontExists,
                        $"Customer Dont Exist.");
                }
            }

            return Result<bool>.Success(true);
        }

        #endregion
    }
}
