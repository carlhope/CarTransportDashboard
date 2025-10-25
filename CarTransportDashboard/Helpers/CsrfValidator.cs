using CarTransportDashboard.Helpers.interfaces;
using CarTransportDashboard.Services.Interfaces;

namespace CarTransportDashboard.Helpers
{
    public class CsrfValidator: ICsrfValidator
    {
        public bool IsValid(HttpRequest request)
        {
            var rawHeader = request.Headers["X-CSRF-Token"].FirstOrDefault();
            var rawCookie = request.Cookies["X-CSRF-Token"];

            if (string.IsNullOrWhiteSpace(rawHeader) || string.IsNullOrWhiteSpace(rawCookie))
                return false;

            var decodedHeader = Uri.UnescapeDataString(rawHeader);
            var decodedCookie = Uri.UnescapeDataString(rawCookie);

            return decodedHeader == decodedCookie;
        }

    }



}
