using CarTransportDashboard.Context;
using System;

namespace CarTransportDashboard.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string CsrfToken { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}