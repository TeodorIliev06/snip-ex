namespace SnipEx.WebApi
{
    public class ApiResponse(bool success, string message)
    {
        public bool Success { get; set; } = success;
        public string Message { get; set; } = message;

        public static ApiResponse Fail(string message) => new ApiResponse(false, message);
        public static ApiResponse SuccessResponse(string message) => new ApiResponse(true, message);
    }
}
