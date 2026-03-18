﻿public interface IMailService
{
    Task SendMailAsync(string to, string subject, string body, byte[] attachment = null, string fileName = null, string cc = null);
}