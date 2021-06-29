namespace PoC.Example.Abstract.Capabilities.CustomerInformationMgmt
{
    public interface ICustomerInformationMgmtCapability
    {
        IPersonService Person { get; }

        ICreatePersonProcess CreatePersonProcess { get; }
    }
}