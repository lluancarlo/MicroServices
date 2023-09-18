namespace ChatAPI.Domain.Commands.Responses
{
    public class PollSessionResponse
    {
        public bool PollSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}