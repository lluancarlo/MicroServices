namespace ChatCoordinatorService.Domain
{
    public class SessionMessage
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}