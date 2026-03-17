﻿using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

public class MailService : IMailService
{
    private readonly IConfiguration _config;

    public MailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendMailAsync(string to, string subject, string body, byte[] attachment, string fileName)
    {
        var message = new MimeMessage();

        message.From.Add(
            MailboxAddress.Parse(_config["Mail:Username"]));

        var recipients = to.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var recipient in recipients)
        {
            message.To.Add(MailboxAddress.Parse(recipient.Trim()));
        }

        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = body;

        if (attachment != null && attachment.Length > 0)
        {
            builder.Attachments.Add(fileName, attachment);
        }

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync("smtp.office365.com", 587, false);

        await smtp.AuthenticateAsync(
            _config["Mail:Username"],
            _config["Mail:Password"]);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
    }
}