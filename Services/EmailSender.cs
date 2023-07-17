using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace JV_Imprest_Payment.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.From = new MailAddress("appsnotification@oandoplc.com", "JV Payment");
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = htmlMessage;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("appsnotification@oandoplc.com", "2BiGTfXjgmI%3D");
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}