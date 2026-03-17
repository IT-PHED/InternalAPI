public interface IMailService
{
    Task SendMailAsync(string to, string subject, string body, byte[] attachment, string fileName);
}