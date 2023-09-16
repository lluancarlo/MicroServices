namespace ChatAPI.Domain.Commands.Responses
{
    public class CreateSessionResponse
    {
        public Guid SessionId { get; set; }
        public bool SessionCreated { get; set; }
        public string ErrorMessage { get; set; }
    }
}