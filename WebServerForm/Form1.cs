using System;
using System.Windows.Forms;

namespace WebServerForm
{
    public partial class Form1 : Form
    {
        private WebServerController controller;

        public Form1()
        {
            InitializeComponent();
            controller = new WebServerController(this);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            textBoxServerLogs.Text = "";
            int serverPort = 80;
            if (!String.IsNullOrEmpty(textBoxPort.Text))
            {
                serverPort = int.Parse(textBoxPort.Text.ToString());
            }
            controller.StartServer(serverPort);
            textBoxServerLogs.Text = "Server started";
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            controller.StopServer();
            textBoxServerLogs.Text = "Server stopped";
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
        }

        public void UpdateLogs(string log)
        {
            textBoxServerLogs.BeginInvoke((Action)(() =>
            {
                textBoxServerLogs.AppendText(log + Environment.NewLine);
            }));
        }
    }
}
