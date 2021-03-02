using System;
using System.Collections.Generic;

namespace SendMail
{
    internal class Program
    {
        private static readonly Dictionary<string, string> argsMap = new Dictionary<string, string>();

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">-user "邮件帐号/密码" -smtp "smtp.office365.com:587" -to "1@abc.com;2@abc.com" -from "123@abc.com" -subject "Test Email" -body "The test is working!" [-att "D:\1.txt;D:\2.txt"] </param>
        private static void Main(string[] args)
        {
            if (args != null)
            {
                int argsLength = args.Length;

                for (int i = 0; i < argsLength; i++)
                {
                    if (args[i][0] == '-')
                    {
                        string key = args[i].ToLower();
                        string value = args[i + 1];
                        argsMap[key] = value.Trim();
                    }
                }
            }

            if (CheckArgsMap(argsMap))
            {
                Sender sender = new Sender();
                sender.Send(argsMap).Wait();
            }
        }

        private static bool CheckArgsMap(Dictionary<string, string> dic)
        {
            bool result = true;
            HashSet<string> hsCheck = new HashSet<string>
            {
                "-to",
                "-from",
                "-subject",
                "-body",
                "-smtp",
                "-user"
            };

            HashSet<string> hsKey = new HashSet<string>(dic.Keys);
            hsCheck.ExceptWith(hsKey);
            foreach (var p in hsCheck)
            {
                Console.WriteLine($"Error: 缺少参数 {p} !");
                result = false;
            }

            return result;
        }
    }
}