using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;


namespace pacman
{
    public class Server
    {
        private readonly ServerObject server;

        public Server(int port, string leaderUrl)
        {
            if (port == 0)
            {
                if (leaderUrl.Equals("leader"))
                {
                    port = 8086;
                    leaderUrl = "tcp://localhost:" + port + "/ServerObject";
                }
                else
                {
                    Random rnd = new Random();
                    port = rnd.Next(49152, 65535);

                    if (!CheckAvailableServerPort(port))
                    {
                        //TODO -> throw new Exception
                    }
                    leaderUrl = "tcp://localhost:" + leaderUrl + "/ServerObject";
                }
            }
            else if (port != 0 && !leaderUrl.Equals("leader"))
            {
                leaderUrl = "tcp://localhost:" + leaderUrl + "/ServerObject";
            }


            Debug.WriteLine("Starting Server...");
            ChannelServices.RegisterChannel(new TcpChannel(port), false);

            server = new ServerObject();
            RemotingServices.Marshal(server, "ServerObject",
                typeof(ServerObject));
            string myUrl = "tcp://localhost:" + port + "/ServerObject";
            server.ConnectServer(myUrl,leaderUrl);
            Debug.WriteLine("Server Up");
        }


        public void SetMAXPLAYERS(int max)
        {
            server.Start(max);
        }

        private bool CheckAvailableServerPort(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }
    }
}
