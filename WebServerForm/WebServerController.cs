using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;

namespace WebServerForm
{
    public class WebServerController
    {
        private Form1 view;
        private Socket httpServer;
        private int serverPort = 80;
        private Thread thread;

        public WebServerController(Form1 form)
        {
            view = form;
        }

        public void StartServer(int serverPort)
        {
            try
            {
                httpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    if (serverPort > 65535 || serverPort <= 0)
                    {
                        PrintLog("Server port not within acceptable range");
                    }
                }
                catch (Exception ex)
                {
                    serverPort = 80;
                    PrintLog("Failed to start server on specified port (" + ex.Message + ")");
                }

                thread = new Thread(new ThreadStart(this.connectionThreadMethod));
                thread.Start();
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error while starting the server");
                PrintLog("Failed to start server (" + ex.Message + ")");
            }
        }

        public void StopServer()
        {
            try
            {
                httpServer.Close();
                thread.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to stop the server", ex.Message);
            }
        }

        private void connectionThreadMethod()
        {
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, serverPort);
                httpServer.Bind(endpoint);
                httpServer.Listen(1);
                startListeningForConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("I could not start (" + ex.Message + ")");
            }
        }

        private void startListeningForConnection()
        {
            while (true)
            {
                DateTime time = DateTime.Now;
                string data = "";
                byte[] bytes = new byte[2048];

                Socket client = httpServer.Accept();

                while (true)
                {
                    int numBytes = client.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numBytes);

                    if (data.IndexOf("\r\n") > -1)
                    {
                        break;
                    }
                }

                PrintLog("\r\n" + data + "\n\n------ End of request ------");

                string resHeaer = "HTTP/1.1 200 Everything is fine\nServer: My c#_Server\nContent-Type: text/html; charset: UTF-8\n\n";
                string resBody = "<!DOCTYPE html>" +
                    "<html>" +
                    "<head>" +
                    "<title>My Server</title>" +
                    "</head>" +
                    "<body><h4>Server Time is: " + time.ToString() + "</h4>" +
                    "<br><h4>Hello World</h4>" +
                    "</body>" +
                    "</html>";

                string resStr = resHeaer + resBody;

                byte[] resData = Encoding.ASCII.GetBytes(resStr);

                client.SendTo(resData, client.RemoteEndPoint);

                PrintLog(resStr);

                client.Close();
            }
        }

        public void PrintLog(string text)
        {
            view.UpdateLogs(text);
        }
    }
}
