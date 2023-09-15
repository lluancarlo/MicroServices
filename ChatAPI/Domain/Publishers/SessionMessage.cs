namespace ChatAPI.Domain.Publishers
{
    public class SessionMessage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}