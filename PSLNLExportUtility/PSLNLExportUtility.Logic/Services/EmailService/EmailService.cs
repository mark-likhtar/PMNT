using PSLNLExportUtility.Infrastructure.Logging;
using PSLNLExportUtility.Logic.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace PSLNLExportUtility.Logic.Services.EmailService
{
    public class EmailService
    {
        private readonly SmtpClient _smtpClient;

        public static readonly Logger _logger = new Logger();

        public EmailService()
        {
            _smtpClient = new SmtpClient();
        }

        public void SendReport(string to, string subject, IEnumerable<string> report)
        {
            try
            {
                var message = new MailMessage();

                message.To.Add(to);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = string.Join("<br>", report);

                _smtpClient.Send(message);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}
