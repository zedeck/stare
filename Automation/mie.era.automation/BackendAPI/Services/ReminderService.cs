using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Numerics;
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
    public class ReminderService
    {
        private readonly ERADBContext _context;
        private readonly HttpClient _httpClient;
        private readonly ICandidate _candidate;
        private readonly IConfiguration _configuration;
        //private readonly string _authApiUrl = "http://192.168.0.24/internal/services/APIAuth/api/Authentication/authenticateDomain?applicationName=era";
        //private readonly string _smsApiUrl = "http://192.168.0.24/internal/services/APIAuth/api/SMS/SendSMS";
        //private readonly string _emailApiUrl = "http://192.168.0.24/internal/services/APIAuth/api/Email/SendEmail";

        public ReminderService(ERADBContext context, ICandidate candidate, IConfiguration config)
        {
            _context = context;
            _candidate = candidate;
            _configuration = config;

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.Credentials = CredentialCache.DefaultCredentials;
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

            _httpClient = new HttpClient(handler);
        }

        public async Task<List<Request>> GetUpcomingRequests()
        {
            var currentDate = DateTime.Now;
            return await _context.Requests
                .AsNoTracking()
                .Where(r => r.RequestDate > currentDate && r.Status == "Pending")
                .ToListAsync();
        }

        public async Task<string> GetRefereePhoneNumber(int refereeID)
        {
            var referee = await _context.Referees
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Refereeid == refereeID);
            return referee?.PhoneNumber ?? string.Empty;
        }

        public async Task SendSms(string phoneNumber, string message, int refereeId)
        {
            try
            {
                string token = await GetAuthToken();
                await SendSmsWithToken(phoneNumber, message, token, refereeId);
                await SaveNotification(phoneNumber, "SMS", "Sent", message, refereeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending SMS: {ex.Message}");
            }
        }

        private async Task SendSmsWithToken(string phoneNumber, string message, string token, int refereeId)
        {
            try
            {
                string smsRequestUrl = $"{_configuration.GetConnectionString("_smsApiUrl")}?number={phoneNumber}&message={Uri.EscapeDataString(message)}";
                var requestMsg = new HttpRequestMessage(HttpMethod.Post, smsRequestUrl);
                requestMsg.Headers.Add("MIEAuthorization", "Bearer " + token);
                var response = await _httpClient.SendAsync(requestMsg);
                response.EnsureSuccessStatusCode();
                await SaveNotification(phoneNumber, "SMS", "Sent", message, refereeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending SMS: {ex.Message}");
            }
        }

        public async Task SendEmail(string email, string subject, string body, int refereeId)
        {
            try
            {
                string token = await GetAuthToken();
                await SendEmailWithToken(email, subject, body, token, refereeId, null);
                Console.WriteLine($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending Email: {ex.Message}");
            }
        }

        private async Task SendEmailWithToken(string email, string subject, string body, string token, int? refereeId, int? requestId )
        {
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
                    readReceipt = true,
                    deliveryReceipt = false
                };

                var jsonPayload = JsonSerializer.Serialize(emailPayload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var requestMsg = new HttpRequestMessage(HttpMethod.Post, _configuration.GetConnectionString("_emailApiUrl"));
                requestMsg.Headers.Add("MIEAuthorization", "Bearer " + token);
                requestMsg.Content = content;
                var response = await _httpClient.SendAsync(requestMsg);
                response.EnsureSuccessStatusCode();

                if (refereeId.HasValue)
                    await SaveNotification(email, "Email", "Sent", body, refereeId.Value);

                if (requestId.HasValue)
                    await SaveInternalNotification(email, email, requestId.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending Email: {ex.Message}");
            }
        }

        public async Task SendReminders(int requestId)
        {
            try
            {
                string token = await GetAuthToken();

                var request = await _context.Requests
                   .AsNoTracking()
                   .FirstOrDefaultAsync(r => r.RequestId == requestId);

                if (request == null)
                {
                    Console.WriteLine($"Request with ID {requestId} not found.");
                    return;
                }

                var referee = await _context.Referees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Refereeid == request.RefereeId);

               

                if (referee == null)
                {
                    Console.WriteLine($"Referee with ID {request.RefereeId} not found.");
                    return;
                }

               

                var email = referee.Email;
                var phoneNumber = referee.PhoneNumber;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phoneNumber))
                {
                    Console.WriteLine($"Referee with ID {request.RefereeId} is missing email or phone number.");
                    return;
                }

                var candidate = _candidate.GetCandidateInfoByReqID(request.RequestId);
                var reminderUrl = _candidate.GetReferenceLinkByRequestID(request.RequestId);

                var smsMessage = StringConstants.REMINDER_SMSMSG.Replace("#REFNAME", referee.Name);
                smsMessage = smsMessage.Replace("#CANNAME", candidate.CandidateName);
                smsMessage = smsMessage.Replace("#LINK", reminderUrl);
                smsMessage = smsMessage.Replace("#NEWLINE", System.Environment.NewLine);

                var emailMessage = StringConstants.REMINDER_EMAILMSG.Replace("#REFNAME", referee.Name);
                emailMessage = emailMessage.Replace("#CANNAME", candidate.CandidateName);
                emailMessage = emailMessage.Replace("#LINK", reminderUrl);

                await SendSmsWithToken(phoneNumber, smsMessage, token, referee.Refereeid);
                await SendEmailWithToken(email, "Reference Check Reminder", emailMessage, token, referee.Refereeid, null);

                Console.WriteLine($"Reminders sent successfully to Referee ID {referee.Refereeid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending reminders: {ex.Message}");
            }
        }


        private async Task<string> GetAuthToken()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_configuration.GetConnectionString("_authApiUrl"), new StringContent(""));
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                AuthResponse authResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);
                return authResponse.token;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error obtaining authentication token: {ex.Message}");
            }
        }

        private async Task SaveNotification(string recipient, string type, string status, string message, int refereeId)
        {
            try
            {
                //int refereeId = await GetRefereeIdByRecipient(recipient);
                int refereeID = refereeId;
                var existingNotification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.RefereeId == refereeID && n.NotificationType == type);

                if (existingNotification != null)
                {
                    _context.Entry(existingNotification).State = EntityState.Detached;
                }

                var notification = new Notification
                {
                    RefereeId = refereeID,
                    NotificationType = type,
                    NotificationStatus = status,
                    NotificationDateTime = DateTime.Now,
                    Message = message
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving notification: {ex.Message}");
            }
        }


        private async Task SaveInternalNotification(string email, string username, int requestId)
        {
            try
            {
                var notification = new InternalNotification
                {
                    RequestId = requestId,
                    SigEmail= email,
                    SigUser= username,
                    NotifyDate = DateTime.Now
                    
                };

                _context.InternalNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving notification: {ex.Message}");
            }
        }

        private async Task<int> GetRefereeIdByRecipient(string recipient)
        {
            var referee = await _context.Referees
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.PhoneNumber == recipient || r.Email == recipient);

            return referee?.Refereeid ?? 0;
        }


        private class AuthResponse
        {
            public bool success { get; set; }
            public string token { get; set; }
        }

        public async Task<bool> WasSentWithinLastXHoursForRequest(int requestId)
        {
            try
            {
                var Request = _context.Requests
                    .Where(n => n.RequestId == requestId)
                    .Single();

                if (Request.RefereeId == null)
                    return true;

                return await WasSentWithinLastXHours(Request.RefereeId.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if reminder was sent within last 24 hours: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> WasSentWithinLastXHours(int refereeId)
        {
            try
            {
                var lastReminderTimestamp = await _context.Notifications
                    .Where(n => n.RefereeId == refereeId)
                    .MaxAsync(n => (DateTime?)n.NotificationDateTime);

                if (!lastReminderTimestamp.HasValue)
                {
                    return false;
                }

                TimeSpan timeDifference = DateTime.Now - lastReminderTimestamp.Value;

                var interval = await _context.Parameters
                    .Where(c => c.Code == "REMINDER_INT")
                    .SingleAsync();


                return timeDifference <= TimeSpan.FromHours(Convert.ToInt32(interval));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if reminder was sent within last 24 hours: {ex.Message}");
                throw;
            }
        }

        public async Task SendAllReminders()
        {
            var Referees = _context.Referees.FromSqlRaw("exec ERADB.dbo.usp_GetAllRemindersDue").ToList();

            List<int> Ids = Referees.Select(x => x.Refereeid).ToList();

            foreach (var item in Ids)
            {
                await SendReminders(item);
            }
        }

        public async Task SendAllInternalNotifications(string token)
        {
            var Referees = _context.RefereesInfo.FromSqlRaw("exec ERADB.dbo.usp_GetRequestsReadyForApproval").ToList();

            foreach (var item in Referees)
            {
                await SendInternalNotifications(item, token);
            }
        }

        private  async Task SendInternalNotifications(RefereesService.RefereeInfo referee, string token)
        {
            try
            {
                
                var email = referee.SigEmail;
                var a = $"{_configuration.GetConnectionString("FrontEndEDITUrl")}{referee.RemoteKey}/1";

                if (string.IsNullOrEmpty(email) )
                {
                    throw new Exception($"Referee with request ID {referee.RequestID} is missing an email .");
                }

                var emailMessage = StringConstants.INTERNAL_EMAIL_NOTICE.Replace("#SIG_NAME", referee.SigName);
                emailMessage = emailMessage.Replace("#CANNAME", referee.CandidateName);
                emailMessage = emailMessage.Replace("#REMOTE_KEY", referee.RemoteKey);
                emailMessage = emailMessage.Replace("#REFERENCE_LINK", a);


                await SendEmailWithToken(email, "Reference check complete, Please QA response", emailMessage, token, null, referee.RequestID);

                //Console.WriteLine($"Reminders sent successfully to Referee ID {referee.Refereeid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending reminders: {ex.Message}");
            }
        }

    }
}