namespace JwtTokenServer.Models
{
    public class JwtResult
    {
        public JwtResult() { }

        public JwtResult(bool success, object result)
        {
            Success = success;
            Result = result;
        }

        public bool Success { get; set; }
        public object Result { get; set; }
    }
}
