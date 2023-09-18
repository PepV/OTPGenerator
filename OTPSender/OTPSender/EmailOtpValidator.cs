using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTPSender
{
    internal class EmailOtpValidator
    {
        static int maxRetryCount = 10; // Maximum number of OTP validation retries
        static int userMessageTimeoutSeconds = 100; // Timeout duration in seconds for user messages
        internal static async Task<bool> ValidateOTPAsync(string email, Dictionary<string, OTPData> otpDictionary, CancellationToken cancellationToken)
        {
            if (otpDictionary.ContainsKey(email))
            {
                OTPData otpData = otpDictionary[email];

                if (DateTime.Now <= otpData.ExpiryTime) // Check if OTP is still valid
                {
                    int retryCount = 0;
                    while (true)
                    {
                        Console.Write($"Enter the OTP for {email}: ");

                        using (var cts = new CancellationTokenSource())
                        {
                            Task<string> inputTask = GetUserInputAsync(cts.Token);

                            if (inputTask.Wait(userMessageTimeoutSeconds * 1000, cts.Token)) // Wait for user input with timeout
                            {
                                string userEnteredOTP = inputTask.Result;
                                if (userEnteredOTP == otpData.OTP) // Validate OTP
                                {
                                    return true;
                                }
                                else
                                {
                                    retryCount++;
                                    Console.WriteLine($"Invalid OTP entered for {email}. Retrying ({retryCount}/{maxRetryCount})...");
                                    if (retryCount >= maxRetryCount)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                cts.Cancel(); // Cancel the user input task if timeout occurs
                                Console.WriteLine($"Timeout occurred for OTP entry. OTP validation failed.");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"OTP for {email} has expired.");
                }
            }

            return false;
        }

        static async Task<string> GetUserInputAsync(CancellationToken cancellationToken)
        {
            Task<string> inputTask = Task.Run(() => Console.ReadLine());
            Task timeoutTask = Task.Delay(-1, cancellationToken);

            Task completedTask = await Task.WhenAny(inputTask, timeoutTask);

            if (completedTask == inputTask)
            {
                return await inputTask;
            }

            return string.Empty;
        }
    }
}
