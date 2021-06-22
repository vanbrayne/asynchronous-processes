using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoC.SystemTest.WorkFlowServer
{
    internal class WorkFlowPersistence
    {
        public Dictionary<Guid, Customer> Customers { get; } = new Dictionary<Guid, Customer>();


        public Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return Task.FromResult(Customers[customerId]);
        }

        public Task UpdateCustomerAsync(Guid customerId, Customer customer)
        {
            Customers[customerId] = customer;
            return Task.CompletedTask;
        }

        public Task<Guid> CreateCustomerAsync(Customer customer)
        {
            customer.Id = Guid.NewGuid();
            Customers[customer.Id] = customer;
            return Task.FromResult(customer.Id);
        }
    }
}