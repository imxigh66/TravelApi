using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.Email
{
    public class MailtrapOptions
    {
        public string Host { get; set; } = "sandbox.smtp.mailtrap.io";
        public int Port { get; set; } = 2525;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FromEmail { get; set; } = "noreply@travelapp.com";
        public string FromName { get; set; } = "TravelApp";
    }
}
