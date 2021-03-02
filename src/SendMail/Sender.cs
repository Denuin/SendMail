using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SendMail
{
    internal class Sender
    {
        private readonly ISmtpClient _smtp;

        public Sender()
        {
            _smtp = new MailKit.Net.Smtp.SmtpClient();
        }

        public async Task<bool> Send(Dictionary<string, string> argsMap)
        {
            bool result = false;

            string[] to = argsMap["-to"].Split(';');
            string[] from = argsMap["-from"].Split(',');
            string subject = argsMap["-subject"];
            string body = argsMap["-body"];
            string[] smtp = argsMap["-smtp"].Split(':');
            string[] user = argsMap["-user"].Split('/');

            string[] att = null;
            if (argsMap.TryGetValue("-att", out string valueAtt))
            {
                att = valueAtt.Split(';');
            }

            if (smtp?.Length != 2)
            {
                Console.WriteLine("Send Error: -smtp格式有误！");
                return result;
            }
            if (user?.Length != 2)
            {
                Console.WriteLine("Send Error: -user格式有误！");
                return result;
            }

            #region 发邮件

            _smtp.ConnectAsync(smtp[0], Convert.ToInt32(smtp[1]), SecureSocketOptions.StartTls).Wait();
            _smtp.AuthenticateAsync(user[0], user[1]).Wait();

            _smtp.MessageSent += (sender, args) =>
            {
                Console.WriteLine($"Send Message: {args.Response}");
            };

            var message = new MimeMessage();
            InternetAddressList list = new InternetAddressList();
            foreach (var p in to)
            {
                list.Add(new MailboxAddress(p, p));
            }
            message.To.AddRange(list);
            if (from.Length > 1)
            {
                message.From.Add(new MailboxAddress(from[0], from[1]));
            }
            else
            {
                message.From.Add(new MailboxAddress(from[0], from[0]));
            }
            message.Subject = subject;

            var builder = new BodyBuilder();
            if (att != null)
            {
                foreach (var p in att)
                {
                    if (File.Exists(p))
                    {
                        builder.Attachments.Add(p);
                    }
                    else
                    {
                        Console.WriteLine($"提示: 附件文件 {p} 不存在。");
                    }
                }
            }
            builder.TextBody = body;
            message.Body = builder.ToMessageBody();
            await _smtp.SendAsync(message);

            #endregion 发邮件

            return result;
        }
    }
}