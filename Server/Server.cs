using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;
using System.Threading;


namespace pacman
{
    public class Server 
    {
        private readonly ServerObject server;
        private List<int> serverPorts;

        public Server(int port)
        {
            TcpChannel channel = port == 0 ? new TcpChannel(8086) : new TcpChannel(port);
            Debug.WriteLine("Starting Server...");
            ChannelServices.RegisterChannel(channel, false);

            server = new ServerObject();
            RemotingServices.Marshal(server, "ServerObject",
                typeof(ServerObject));

            Debug.WriteLine("Server Up");
            serverPorts.Add(port);
            server.WaitForClientsInput();
        }

        public void SetMAXPLAYERS(int max) { 
            server.SetMaxplayers(max); 
        }

        public void SetServers() { server.GetServerPorts(serverPorts); }

    }
}
