namespace DomainLib.Contracts
{
    public class CreateSessionMessage
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}