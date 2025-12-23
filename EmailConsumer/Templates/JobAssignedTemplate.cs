
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsumer.Templates
{
    public class JobAssignedTemplate : IEmailTemplate
    {
        public string GenerateMessage(Email email) =>
            $"User {email.RecipientUserId} has been assigned a new job by {email.SenderUserId}.";
    }

}
