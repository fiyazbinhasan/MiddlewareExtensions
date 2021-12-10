namespace UseMiddleware
{
    public class ErrorEnvelope
    {
        public string Message { get; }
        public int Status { get; }

        public ErrorEnvelope(string message, int status)
        {
            Message = message;
            Status = status;
        }
    }
}
