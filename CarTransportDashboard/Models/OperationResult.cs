namespace CarTransportDashboard.Models
{
    public record OperationResult<T>(bool Success, string Message, T? Data)
    {
        public static OperationResult<T> CreateSuccess(T data, string message = "Operation completed successfully.") =>
            new(true, message, data);

        public static OperationResult<T> CreateFailure(string message) =>
            new(false, message, default);
    }
}
