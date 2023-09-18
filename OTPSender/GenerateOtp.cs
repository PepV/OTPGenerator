using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTPSender
{
    internal class GenerateOtp
    {
        static int otpTimeoutSeconds = 60; // Timeout duration in seconds
        internal static void GenerateAndSendOTP(string email, Dictionary<string, OTPData> otpDictionary)
        {
            string otp = GenerateOTP(); // Generate the OTP
            otpDictionary[email] = new OTPData(otp, DateTime.Now.AddSeconds(otpTimeoutSeconds)); // Store the OTP and expiry time for validation

            MailSender.SendEmail(email, otp); // Send email with the OTP
        }

        internal static string GenerateOTP()
        {
            // Generate a random 6-digit OTP
            Random random = new Random();
            int otpValue = random.Next(1, 1000000); // Exclude 0 to avoid "000000" OTP
            return otpValue.ToString("D6");
        }
    }
}
