namespace CarTransportDashboard.Helpers.Interfaces
{
    public interface ICsrfValidator
    {
        // Note: ICsrfValidator exists as an interface despite CsrfValidator being a helper.
        // This abstraction allows us to mock CSRF validation in controller tests without simulating full HttpRequest contexts.
        // Other helpers remain static due to their simplicity and deterministic behavior.
        // This is a pragmatic exception for testability, not a pattern to apply universally.
        bool IsValid(HttpRequest request);
       

    }
}
