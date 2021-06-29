namespace PoC.Example.Abstract.Capabilities.CommunicationMgmt
{
    public interface ICommunicationMgmtCapability
    {
        IEmailService Email { get; }
    }
}