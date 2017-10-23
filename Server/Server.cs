using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace pacman
{
    
    public class Server
    {
        private void StartServer()
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);
            List<string> lista = new List<string>();

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(PacmanRemoteObject),
                "PacmanRemoteObject",
                WellKnownObjectMode.Singleton);
            
        }

        


    }
}
