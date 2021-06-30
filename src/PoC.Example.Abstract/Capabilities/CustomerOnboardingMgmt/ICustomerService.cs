using Nexus.Link.Libraries.Crud.Interfaces;
using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;

namespace PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt
{
    public interface ICustomerService : ICreateAndReturn<Person, string>
    {
    }
}