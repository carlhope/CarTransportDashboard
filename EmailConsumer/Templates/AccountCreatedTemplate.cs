using EmailConsumer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsumer.Templates
{
    public class AccountCreatedTemplate : IEmailTemplate
    {
        public string GenerateMessage(Email email) =>
            $"Welcome {email.RecipientUserId}, your account has been created.";
    }

}
