using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Stock_Pie.Application.Interfaces;

namespace Stock_Pie.Application.Services
{
    public class SendResendEmailService : IEmailService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly string _apiKey;
        private readonly string _from;

        public SendResendEmailService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _http = httpClient;
            _apiKey = _config["Resend:ApiKey"] ?? throw new InvalidOperationException("Resend API key missing");
            _from = _config["Resend:From"] ?? "no-reply@example.com";
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task SendOtpEmailAsync(string toEmail, string subject, string body)
        {
            var payload = new
            {
                from = _from,
                to = toEmail,
                subject,
                html = body
            };
            Console.WriteLine(_http.BaseAddress);
            
            var json = JsonSerializer.Serialize(payload);

            var content = new StringContent(json, Encoding.UTF8, "application/json");


            var resp = await _http.PostAsync("emails", content);

            if (!resp.IsSuccessStatusCode)
            {
                var txt = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Resend email failed: {txt}", null, resp.StatusCode);
            }
        }
    }
}