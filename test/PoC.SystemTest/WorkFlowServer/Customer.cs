using System;

namespace PoC.SystemTest.WorkFlowServer
{
    public class Customer
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public Guid? CreditAccountId { get; set; }
        public Guid? SavingsAccountId { get; set; }
        public Guid Id { get; set; }
    }
}