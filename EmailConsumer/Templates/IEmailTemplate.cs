using EmailConsumer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailConsumer.Templates
{
    public interface IEmailTemplate
    {
        string GenerateMessage(Email email);

    }
}
