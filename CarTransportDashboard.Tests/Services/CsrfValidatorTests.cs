using CarTransportDashboard.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarTransportDashboard.Tests.Services
{
    public class CsrfValidatorTests
    {
        private readonly CsrfValidator _validator = new CsrfValidator();

        private HttpRequest CreateRequest(string headerValue, string cookieValue)
        {
            var context = new DefaultHttpContext();

            // Set header
            if (headerValue != null)
                context.Request.Headers["X-CSRF-Token"] = headerValue;

            // Set cookie via response, then copy to request
            context.Response.Cookies.Append("X-CSRF-Token", cookieValue ?? "");

            // Simulate cookie round-trip
            var cookieHeader = context.Response.Headers["Set-Cookie"];
            context.Request.Headers["Cookie"] = cookieHeader;

            return context.Request;
        }


        [Fact]
        public void IsValid_ReturnsTrue_WhenHeaderAndCookieMatch()
        {
            var request = CreateRequest("abc%2F123%3D%3D", "abc/123==");
            Assert.True(_validator.IsValid(request));
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenHeaderMissing()
        {
            var request = CreateRequest(null, "abc/123==");
            Assert.False(_validator.IsValid(request));
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenCookieMissing()
        {
            var request = CreateRequest("abc%2F123%3D%3D", null);
            Assert.False(_validator.IsValid(request));
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenHeaderAndCookieMismatch()
        {
            var request = CreateRequest("wrong%2Fvalue%3D%3D", "abc/123==");
            Assert.False(_validator.IsValid(request));
        }

        [Fact]
        public void IsValid_ReturnsFalse_WhenBothValuesAreEmpty()
        {
            var request = CreateRequest("", "");
            Assert.False(_validator.IsValid(request));
        }
    }

}
