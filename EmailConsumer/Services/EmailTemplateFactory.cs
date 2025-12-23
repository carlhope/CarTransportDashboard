
using EmailConsumer.Templates;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsumer.Services
{
    public static class EmailTemplateFactory
    {
        public static IEmailTemplate Create(EmailType emailType)
        {
            return emailType switch
            {
                EmailType.JobAccepted => new JobAcceptedTemplate(),
                EmailType.JobAssigned => new JobAssignedTemplate(),
                EmailType.PasswordReset => new PasswordResetTemplate(),
                EmailType.AccountCreated => new AccountCreatedTemplate(),
                _ => new UnknownTemplate()
            };

        }
    }
}
