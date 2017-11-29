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

        public Server()
        {
            Debug.WriteLine("Starting Server...");
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            server = new ServerObject();
            RemotingServices.Marshal(server, "ServerObject",
                typeof(ServerObject));

            Debug.WriteLine("Server Up");

            server.WaitForClientsInput();
        }

        public Server(int port)
        {
            Debug.WriteLine("Starting Server...");
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            server = new ServerObject();
            RemotingServices.Marshal(server, "ServerObject",
                typeof(ServerObject));

            Debug.WriteLine("Server Up");

            server.WaitForClientsInput();
        }

        public void SetMAXPLAYERS(int max) { 
            server.SetMaxplayers(max); 
        }


    }
}
