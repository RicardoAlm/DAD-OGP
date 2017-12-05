using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net;

namespace pacman
{
    public class Server 
    {
        private readonly ServerObject server;

        public Server(int port , string leaderPort)
        {
            new TcpChannel(port);
            string url="";

            if (port == 0)
            {
                if (leaderPort.Equals(""))
                    port = 8086;
                else
                {
                    Random rnd = new Random();
                    port = rnd.Next(49152, 65535);

                    if (!CheckAvailableServerPort(port))
                    {
                        //TODO -> throw new Exception
                    }
                    url = "tcp://localhost:" + port + "/ServerObject";
                }
            }
            else
            {
                url = leaderPort;
            }
            TcpChannel channel = new TcpChannel(port);
            Debug.WriteLine("Starting Server...");
            ChannelServices.RegisterChannel(channel, false);

            server = new ServerObject();
            RemotingServices.Marshal(server, "ServerObject",
                typeof(ServerObject));

            Debug.WriteLine("Server Up");
            Debug.WriteLine("PORT: " + leaderPort + "this.port: " + port );
            server.ConnectServer(url, "tcp://localhost:" + port + "/ServerObject");
            
        }

        public void SetMAXPLAYERS(int max) { 
            server.SetMaxplayers(max); 
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
