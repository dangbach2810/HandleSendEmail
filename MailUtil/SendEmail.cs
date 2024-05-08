using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailUtil
{
    public class SendEmail
    {
       
        public async Task SendMail(string _from, string _to,
            string _subject, string _body
        )
        {
            string _password = "bach1111@";
            string _gmail = "locdangbach@gmail.com";
            MailMessage message = new MailMessage(_from, _to, _subject, _body);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Sender = new MailAddress(_from);
            using var smtpClient = new SmtpClient("smpt.gmail.com");
            smtpClient.Port = 578;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_gmail, _password);
            try { 
                await smtpClient.SendMailAsync(message);
            }catch(Exception ex)
            { 
                Console.WriteLine(ex.ToString());
            }
        
        
        }
    }
    
}
