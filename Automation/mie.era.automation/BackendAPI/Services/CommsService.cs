using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackendAPI.Constants;
using BackendAPI.Database;
using BackendAPI.Interfaces;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Services
{
    public class CommsService
    {
        private readonly IDatabaseRepo _dbaction;
        private readonly IConfiguration _configmngr;
        private readonly HttpClient _httpClient;

       
        public CommsService(IDatabaseRepo dbaction, IConfiguration configmngr)
        {
            _dbaction = dbaction;
            _configmngr = configmngr;

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.Credentials = CredentialCache.DefaultCredentials;
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
            _httpClient = new HttpClient(handler);

        }


       

        public async Task<List<Request>> GetUpcomingRequests()
        {
            return await _dbaction.GetUpcomingRequests();
        }

        public string GetRefereePhoneNumber(int refereeID)
        {
            return _dbaction.GetRefereePhoneNumber(refereeID);   
        }

        public async Task SendSms(string phoneNumber, string message, int refereeId, bool saveNotification)
        {
            try
            {
                string token = await GetAuthToken();
                await SendSmsWithToken(phoneNumber, message, token, refereeId, saveNotification);
                SaveNotification(phoneNumber, "SMS", "Sent", message, refereeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
            }
        }

        private async Task SendSmsWithToken(string phoneNumber, string message, string token, int refereeId, bool saveNotification)
        {

            string _smsApiUrl = _configmngr.GetConnectionString("_smsApiUrl").ToString();

            try
            {
                string smsRequestUrl = $"{_smsApiUrl}?number={phoneNumber}&message={Uri.EscapeDataString(message)}";
                var requestMsg = new HttpRequestMessage(HttpMethod.Post, smsRequestUrl);
                requestMsg.Headers.Add("MIEAuthorization", "Bearer " + token);
                var response = await _httpClient.SendAsync(requestMsg);
                response.EnsureSuccessStatusCode();
                //if(saveNotification)
                  //SaveNotification(phoneNumber, "SMS", "Sent", message, refereeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending SMS: {ex.Message}");
            }
        }




        public async Task SendEmail(string email, string subject, string body, int refereeId, bool saveNotification)
        {
            try
            {
                string token = await GetAuthToken();
                await SendEmailWithToken(email, subject, body, token, refereeId, saveNotification);
                Console.WriteLine($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending Email: {ex.Message}");
            }
        }

        private async Task SendEmailWithToken(string email, string subject, string body, string token, int refereeId, bool saveNotification)
        {
            string _emailApiUrl = _configmngr.GetConnectionString("_emailApiUrl").ToString();

            try
            {
                var emailPayload = new
                {
                    priority = 1,
                    encoding = 1,
                    from = new { name = "noreply@mie.co.za", address = "noreply@mie.co.za" },
                    replyTo = new { name = "noreply@mie.co.za", address = "noreply@mie.co.za" },
                    to = new[] { new { name = email, address = email } },
                    subject = subject,
                    body = body,
                    readReceipt = false,
                    deliveryReceipt = false
                };

                var jsonPayload = JsonSerializer.Serialize(emailPayload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var requestMsg = new HttpRequestMessage(HttpMethod.Post, _emailApiUrl);
                requestMsg.Headers.Add("MIEAuthorization", "Bearer " + token);
                requestMsg.Content = content;
                var response = await _httpClient.SendAsync(requestMsg);
                response.EnsureSuccessStatusCode();
                //if(saveNotification)
                //    SaveNotification(email, "Email", "Sent", body, refereeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending Email: {ex.Message}");
            }
        }

        public async Task SendComms(LReminderModel reminderData )
        {
            try
            {
                    string token = await GetAuthToken();

                if (reminderData.ReferenceType == 0)   //Reminder Type
                {

                    string smsMessage = StringConstants.REMINDER_SMSMSG;
                    smsMessage = smsMessage.Replace("REFEREE_NAME", reminderData.RefereeName);
                    smsMessage = smsMessage.Replace("REFERENCE_NAME", reminderData.ReferenceName);
                    smsMessage = smsMessage.Replace("REFERENCE_LINK", reminderData.LinkURL);


                    string emailMessage = StringConstants.REMINDER_EMAILMSG;
                    emailMessage = emailMessage.Replace("REFEREE_NAME", reminderData.RefereeName);
                    emailMessage = emailMessage.Replace("REFERENCE_NAME", reminderData.ReferenceName);
                    emailMessage = emailMessage.Replace("REFERENCE_LINK", reminderData.LinkURL);

                    await SendSmsWithToken(reminderData.RefereePhone, smsMessage, token, reminderData.RefereeId, reminderData.SaveNotification);
                    await SendEmailWithToken(reminderData.RefereeEmail, "Reference Check Reminder", emailMessage, token, reminderData.RefereeId, reminderData.SaveNotification);
                }
                else   //Referenec Alert Type
                {


                    string smsMessage = StringConstants.NEWREFERENCE_SMSMSG;
                    smsMessage = smsMessage.Replace("REFEREE_NAME", reminderData.RefereeName);
                    smsMessage = smsMessage.Replace("REFERENCE_NAME", reminderData.ReferenceName);
                    smsMessage = smsMessage.Replace("REFERENCE_LINK", reminderData.LinkURL);


                    string emailMessage = StringConstants.NEWREFERENCE_EMAILMSG;
                    emailMessage = emailMessage.Replace("REFEREE_NAME", reminderData.RefereeName);
                    emailMessage = emailMessage.Replace("REFERENCE_NAME", reminderData.ReferenceName);
                    emailMessage = emailMessage.Replace("REFERENCE_LINK", reminderData.LinkURL);


                    await SendSmsWithToken(reminderData.RefereePhone, smsMessage, token, reminderData.RefereeId, reminderData.SaveNotification);  ;
                    await SendEmailWithToken(reminderData.RefereeEmail, "New Reference Alert!!!", emailMessage, token, reminderData.RefereeId, reminderData.SaveNotification);


                }
                Console.WriteLine($"Reminders sent successfully to Referee ID {0}", reminderData.RefereeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending reminders: {ex.Message}");
            }
        }


        private async Task<string> GetAuthToken()
        {

            string _authApiUrl = _configmngr.GetConnectionString("_authApiUrl").ToString();

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_authApiUrl, new StringContent(""));
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                AuthResponse? authResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);
                return authResponse.token;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obtaining authentication token: {ex.Message}");
            }
        }

        private void SaveNotification(string recipient, string type, string status, string message, int refereeId)
        {
            _dbaction.SaveNotification(recipient, type, status, message, refereeId);
        }
        private int GetRefereeIdByRecipient(string recipient)
        {
            return _dbaction.GetRefereeIdByRecipient(recipient);
        }

        private class AuthResponse
        {
            public bool success { get; set; }
            public string? token { get; set; }
        }
    }
}