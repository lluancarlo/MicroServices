namespace ChatAPI.Domain.Commands.Responses
{
    public class CreateSessionResponse
    {
        public bool SessionCreated { get; set; }
        public string ErrorMessage { get; set; }
    }
}