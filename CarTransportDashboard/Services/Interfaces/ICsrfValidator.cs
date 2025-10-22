namespace CarTransportDashboard.Services.Interfaces
{
    public interface ICsrfValidator
    {
        bool IsValid(HttpRequest request);
    }
}
