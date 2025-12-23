
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsumer.Templates
{
    public class UnknownTemplate : IEmailTemplate
    {
        public string GenerateMessage(Email email) =>
            $"Unknown email type for {email.RecipientUserId}.";
    }

}
