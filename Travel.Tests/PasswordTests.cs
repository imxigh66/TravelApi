using BCrypt.Net;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Tests
{
    public class PasswordTests
    {
        public void HashPassword_ValidPassword_ReturnsHash()
        {
            var password= "Test123!";

            var hash=BCrypt.Net.BCrypt.HashPassword(password);

            hash.Should().NotBeNullOrEmpty();
            hash.Should().NotBe(password);
        }
    }
}
