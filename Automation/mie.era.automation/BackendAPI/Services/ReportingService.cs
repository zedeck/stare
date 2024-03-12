using BackendAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using BackendAPI.Database;
using Common.Authentication;

namespace BackendAPI.Services
{
    public class ReportingService
    {
        private readonly IConfiguration _configuration;
        private readonly ERADBContext _context;
        private readonly string _ssrsIntegrationPath;
        private readonly string _reportFilePath;
        private readonly string _eraReportPath;

        public ReportingService(ERADBContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
            _eraReportPath = _configuration.GetConnectionString("EraSSRSReportPath");
            _ssrsIntegrationPath = _configuration.GetConnectionString("ReportIntegrationURL");
            _reportFilePath = _configuration.GetConnectionString("ReportTempPath");
        }

        private HttpClient GetHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.Credentials = CredentialCache.DefaultCredentials;
            handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
            return new HttpClient(handler);

        }

        private byte[] GetReport(string remoteKey, string token)
        {
            
            HttpClient httpClient = GetHttpClient();
            //httpClient.BaseAddress = new Uri(_ssrsIntegrationPath);
            var reportPayload = new
            {
                reportPath = _eraReportPath,
                reportFormat = 0,
                parameters = new[] { new { name = "RemoteKey", value = remoteKey } },
            };

            var jsonPayload = JsonSerializer.Serialize(reportPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var requestMsg = new HttpRequestMessage(HttpMethod.Post, _ssrsIntegrationPath);
            requestMsg.Headers.Add("MIEAuthorization", "Bearer " + token);
            requestMsg.Content = content;
            var response = httpClient.SendAsync(requestMsg).Result;
            response.EnsureSuccessStatusCode();
            string tmp = response.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
            return Convert.FromBase64String(tmp);
        }

        public void SubmitToPCV(string token)
        {
            var Requests = _context.Requests.FromSqlRaw("exec ERADB.dbo.usp_GetRequestsDue").ToList();

            foreach (var item in Requests)
            {
                var reportData = GetReport(item.RemoteKey, token);

                //Save the report locally
                var filename = $"EAR_{item.RequestId.ToString()}_{item.RemoteKey}.pdf";
                filename = Path.Combine(_reportFilePath, filename);
                File.WriteAllBytes(filename, reportData);

                _context.Database.ExecuteSqlRaw("EXEC ERADB.dbo.usp_resultMIERequest @RemoteKey, @FileName",
                    new Microsoft.Data.SqlClient.SqlParameter("@RemoteKey", item.RemoteKey),
                    new Microsoft.Data.SqlClient.SqlParameter("@FileName", filename));
            }
        }


    }
}
