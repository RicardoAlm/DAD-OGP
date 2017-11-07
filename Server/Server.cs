using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;


namespace pacman
{
    public class Server 
    {
        private PacmanServerObject server;

        public Server()
        {
            Debug.WriteLine("Starting Server...");
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            server = new PacmanServerObject();
            RemotingServices.Marshal(server, "PacmanServerObject",
                typeof(PacmanServerObject));



           /* RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PacmanServerObject),
                "PacmanServerObject",
                WellKnownObjectMode.Singleton);*/
            Debug.WriteLine("Server Up");
        }

        public void SetMAXPLAYERS(int max) { 
            server.SetMaxplayers(max); 
        }


    }
}
