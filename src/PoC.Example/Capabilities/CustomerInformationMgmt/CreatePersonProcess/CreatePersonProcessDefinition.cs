using PoC.Example.Abstract.Capabilities.CommunicationMgmtCapability;
using PoC.Example.Abstract.Capabilities.CustomerInformationMgmt;
using PoC.LinkLibraries.LibraryCode;

namespace PoC.Example.Capabilities.CustomerInformationMgmt.CreatePersonProcess
{
    public class CreatePersonProcessDefinition : ProcessDefinition<Person>
    {
        public ICustomerInformationMgmtCapability CustomerInformationMgmt { get; }
        public ICommunicationMgmtCapability CommunicationMgmt { get; set; }

        public CreatePersonProcessDefinition(ICustomerInformationMgmtCapability customerInformationMgmt, ICommunicationMgmtCapability communicationMgmt)
        : base("Initialize person", "81B696D3-E27A-4CA8-9DC1-659B78DFE474")
        {
            CustomerInformationMgmt = customerInformationMgmt;
            CommunicationMgmt = communicationMgmt;
            // TODO: How does the major/minor version affect the data model?
            //var version = Versions.Add(1, 2, Version1Async); 
            //version.Parameters.Add("personalNumber");
            var version = ProcessVersions.Add<CreatePersonProcessV2>(2, 1);
            // TODO: Make Parameters a class and change Add to Register(string parameterName, Type parameterType)
            // TODO: NO need for numbers, they are sequential
            version.Parameters.Add(1, "Person");

        }
    }
}