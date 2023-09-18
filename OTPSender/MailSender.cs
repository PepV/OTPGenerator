using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OTPSender
{
    internal class MailSender
    {
        internal static void SendEmail(string recipient, string otp)
        {
            string senderEmail = "pradeep.venkadachalam@gmail.com"; // Replace with your email address
            string senderPassword = "jzovleeufespdztp"; // Replace with your email password

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderEmail);
            mailMessage.To.Add(recipient);
            mailMessage.Subject = "One-Time Password (OTP) Verification";
            mailMessage.Body = $"Your OTP is: {otp}";

            try
            {
                client.Send(mailMessage);
                Console.WriteLine($"OTP sent successfully to {recipient}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }
}
