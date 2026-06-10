namespace Stock_Pie.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string subject, string body);
    }
}