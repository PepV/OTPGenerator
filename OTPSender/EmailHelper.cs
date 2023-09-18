using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OTPSender
{
    internal static class EmailHelper
    {
        static string domain = "dso.org.sg";
        //static string domain = "gmail.com";

        internal static string GetValidEmail(string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    bool isValidFormat = IsValidEmailFormat(email);
                    bool isValidDomain = IsValidEmailDomain(email, domain);

                    while (!isValidFormat || !isValidDomain)
                    {
                        Console.WriteLine($"Invalid email format or domain: {email}. Please enter a valid email address:");
                        email = Console.ReadLine();

                        isValidFormat = IsValidEmailFormat(email);
                        isValidDomain = IsValidEmailDomain(email, domain);
                    }

                    return email;
                }
                else
                    return email;
            }
            catch (Exception)
            {
                Console.WriteLine($"Email ddress in empty.");
                return string.Empty;
            }
        }

        static bool IsValidEmailFormat(string email)
        {
            if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return false;
            }

            return true;
        }

        static bool IsValidEmailDomain(string email, string domain)
        {
            try
            {
                // Extract domain from email address
                string emailDomain = email.Split('@')[1];

                // Compare the email domain with the specified domain
                if (!string.IsNullOrEmpty(emailDomain))
                    return emailDomain.Equals(domain, StringComparison.OrdinalIgnoreCase);
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
