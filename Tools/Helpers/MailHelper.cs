using System;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace HeroServer
{
    public static class MailHelper
    {
        static String smtpServer, smtpName, smtpMail, smtpPwd;
        static int smtpPort;

        public static async void Initialize()
        {
            smtpServer = await new SystemParamDB().GetValue("SmtpServer");
            smtpPort = Convert.ToInt32(await new SystemParamDB().GetValue("SmtpPort"));
            smtpName = await new SystemParamDB().GetValue("SmtpName");
            smtpMail = await new SystemParamDB().GetValue("SmtpMail");
            smtpPwd = await new SystemParamDB().GetValue("SmtpPwd");
        }

        public static async Task SendMail(String receiverMail, String receiverName, String subject, String message, bool bHtml = false)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(smtpName, smtpMail));
            mimeMessage.To.Add(new MailboxAddress(receiverName, receiverMail));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(bHtml ? TextFormat.Html : TextFormat.Plain) { Text = message };

            await SendAsync(mimeMessage);
        }

        public static async Task SendMailAttach(String receiverMail, String receiverName, String subject, byte[] documentPdf, String documentHtml)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(smtpName, smtpMail));
            mimeMessage.To.Add(new MailboxAddress(receiverName, receiverMail));
            mimeMessage.Subject = subject;
            //mimeMessage.Body = new TextPart(bHtml ? TextFormat.Html : TextFormat.Plain) { Text = message };

            BodyBuilder emailBody = new BodyBuilder
            {
                HtmlBody = documentHtml
            };
            emailBody.Attachments.Add("Agreement.pdf", documentPdf);

            // If you find that MimeKit does not properly auto-detect the mime-type based on the
            // filename, you can specify a mime-type like this:
            //emailBody.Attachments.Add ("Receipt.pdf", bytes, ContentType.Parse (MediaTypeNames.Application.Pdf));

            mimeMessage.Body = emailBody.ToMessageBody();


            await SendAsync(mimeMessage);
        }

        private static async Task SendAsync(MimeMessage mimeMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(smtpMail, smtpPwd);
                    await client.SendAsync(mimeMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
