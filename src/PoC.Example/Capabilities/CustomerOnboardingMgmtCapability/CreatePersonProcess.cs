using PoC.Example.Abstract.Capabilities.Common;
using PoC.Example.Abstract.Capabilities.CommunicationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.Example.Abstract.Capabilities.CustomerOnboardingMgmt;
using PoC.Example.Capabilities.CustomerOnboardingMgmtCapability.Processes;
using PoC.LinkLibraries.LibraryCode;

namespace PoC.Example.Capabilities.CustomerOnboardingMgmtCapability
{
    public class CreatePersonProcess : ProcessDefinition<Person>, ICreatePersonProcess
    {
        public ICustomerInformationMgmtCapability CustomerInformationMgmt { get; }
        public ICommunicationMgmtCapability CommunicationMgmt { get; set; }

        public CreatePersonProcess(ICustomerInformationMgmtCapability customerInformationMgmt, ICommunicationMgmtCapability communicationMgmt)
        : base("Initialize person", "81B696D3-E27A-4CA8-9DC1-659B78DFE474")
        {
            CustomerInformationMgmt = customerInformationMgmt;
            CommunicationMgmt = communicationMgmt;
            // TODO: How does the major/minor version affect the data model?
            //var version = Versions.Add(1, 2, Version1Async); 
            //version.Parameters.Add("personalNumber");
            var version = ProcessVersions.Add<CreatePersonProcessV2>(2, 1);
            version.AddParameter<Person>("Person");
        }
    }
}