using System;
using System.Collections.Generic;
using System.Text;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Example;

namespace PoC.Example.Capabilities.CustomerInformationMgmt
{
    public class CustomerInformationMgmtCapability : ICustomerInformationMgmtCapability
    {
        public CustomerInformationMgmtCapability()
        {
            Person = new PersonService();
        }

        /// <inheritdoc />
        public IPersonService Person { get; }
    }
}
