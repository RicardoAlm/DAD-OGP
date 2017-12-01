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

        public Server(int port)
        {
            TcpChannel channel;
            if (port == 0)
            {
               channel = new TcpChannel(8086);
            }
            else
            {
                channel = new TcpChannel(port);
            }
            Debug.WriteLine("Starting Server...");
            ChannelServices.RegisterChannel(channel, false);

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
