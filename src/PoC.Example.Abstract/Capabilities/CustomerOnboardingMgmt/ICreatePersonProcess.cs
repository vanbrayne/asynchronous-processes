using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.LinkLibraries.LibraryCode;

namespace PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt
{
    public interface ICreatePersonProcess : IProcessDefinition<Person>
    {
    }
}