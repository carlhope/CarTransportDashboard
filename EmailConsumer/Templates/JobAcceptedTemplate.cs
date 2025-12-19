using EmailConsumer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsumer.Templates
{
    internal class JobAcceptedTemplate:IEmailTemplate
    {
        public string GenerateMessage(Email email) =>
            $"User {email.RecipientUserId} has accepted the job.";

    }
}
