using OTPSender;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static Dictionary<string, OTPData> otpDictionary = new Dictionary<string, OTPData>();
    static int otpTimeoutSeconds = 60; // Timeout duration in seconds
    static int maxRetryCount = 10; // Maximum number of OTP validation retries
    static int userMessageTimeoutSeconds = 100; // Timeout duration in seconds for user messages
    static void Main(string[] args)
    {

        try
        {
            List<string> emailList = new List<string>()
        {
            "pradeep.venkadachalam@gmail.com",
            "pradsanv@gmail.com"
        };

            foreach (string email in emailList)
            {
                string validEmail = EmailHelper.GetValidEmail(email); // Prompt the user to enter a valid email address
                if (validEmail != null)
                {
                    GenerateOtp.GenerateAndSendOTP(validEmail, otpDictionary); // Generate and send OTP for each valid email
                }
                else
                {
                    Console.WriteLine($"Invalid email format or domain: {email}. Skipping OTP generation.");
                }
            }

            foreach (string email in emailList)
            {
                bool isValidOTP = false;

                int retryCount = 0;
                while (!isValidOTP && retryCount < maxRetryCount)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        Task<bool> validationTask = EmailOtpValidator.ValidateOTPAsync(email, otpDictionary, cts.Token);

                        if (validationTask.Wait(otpTimeoutSeconds * 1000, cts.Token)) // Wait for OTP entry with timeout
                        {
                            isValidOTP = validationTask.Result;
                        }
                        else
                        {
                            cts.Cancel(); // Cancel the validation task if timeout occurs
                            Console.WriteLine($"Timeout occurred for {email}. OTP validation failed.");
                            break;
                        }
                    }

                    if (!isValidOTP)
                    {
                        retryCount++;

                        if (retryCount < maxRetryCount)
                        {
                            Console.WriteLine($"Invalid OTP for {email}. Retrying ({retryCount}/{maxRetryCount}) in {userMessageTimeoutSeconds} seconds...");
                            Task.Delay(userMessageTimeoutSeconds * 1000).Wait(); // Wait for user message timeout
                        }
                        else
                        {
                            Console.WriteLine($"Maximum retries reached for {email}. OTP validation failed.");
                        }
                    }
                }

                if (isValidOTP)
                {
                    Console.WriteLine($"OTP for {email} is valid.");
                }
            }

            Console.ReadLine();
        }
        catch (Exception)
        {
            throw;
        }
    }

}

class OTPData
{
    public string OTP { get; }
    public DateTime ExpiryTime { get; }

    public OTPData(string otp, DateTime expiryTime)
    {
        OTP = otp;
        ExpiryTime = expiryTime;
    }
}
