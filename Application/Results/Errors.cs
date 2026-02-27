using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Results
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }

        private Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static Error BadRequest(string message)
            => new("BadRequest", message);

        public static Error NotFound(string message)
            => new("NotFound", message);

        public static Error Conflict(string message)
            => new("Conflict", message);
    }
}
