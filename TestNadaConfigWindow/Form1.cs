using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NadaConfigClient;

namespace TestNadaConfigWindow
{
    public partial class Form1 : Form
    {
        NadaClient myClient;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            groupBox1.Enabled = false;
            TO.Visible = false;
            conn.Visible = false;
            server.Text = string.Empty;
            service.Text = string.Empty;
            if (String.IsNullOrEmpty(servicename.Text))
            {
                myClient = new NadaClient(Env.Text, String.IsNullOrEmpty(srvname.Text)?null:srvname.Text, 20000);
            }
            else
            {
                myClient = new NadaClient(Env.Text, servicename.Text);
            }

            button1.Enabled = true;
            if (myClient.isConnected)
            {
                conn.Visible = true;
                groupBox1.Enabled = true;
                server.Text = myClient.ServerName;
                service.Text = myClient.ServiceName;
            }
            else
            {
                TO.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            val.Text = string.Empty;

            if (myClient != null)
            {
                val.Text = myClient.GetConfigItem(section.Text, item.Text);
            }
        }

        private void servicename_Leave(object sender, EventArgs e)
        {
            if (servicename.Text.Length > 0)
            {
                srvname.Enabled = false;
            }
            else
            {
                srvname.Enabled = true;
            }
        }
    }
}
