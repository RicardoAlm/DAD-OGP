using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using pacman;


namespace ProcessCreationService
{
    class PCS
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(11000);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(PcsRemote),
                "PCS",
            WellKnownObjectMode.Singleton);
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
