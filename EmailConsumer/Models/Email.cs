using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsumer.Models
{
    public record Email(string RecipientUserId, EmailType EmailType, string SenderUserId);

    public enum EmailType
    {
        JobAccepted,
        JobAssigned,
        PasswordReset,
        AccountCreated,
        Unknown
    }
}
