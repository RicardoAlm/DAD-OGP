using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace pacman
{
    class Client 
    {
        private TcpChannel channel = null;
        private IPacmanPlatform server;
        private ClientObject client;
        private int port;
        private int player;

        public Client(Form1 form, Delegate d, Delegate p)
        {
            Debug.WriteLine("Connecting to server...");
            ConnectToServer(form, d, p);
            Debug.WriteLine("Connected to server");
            /* Debug.WriteLine("Updating Clients list...");
             server.GetClients(port.ToString());
             Debug.WriteLine("Clients list Updated");*/
            //form.
        }

        public void ConnectToServer(Form f, Delegate d, Delegate p)
        {
            Random rnd = new Random();
            port = rnd.Next(49152, 65535);

            if (CheckAvailableServerPort(port))
            { 
                channel = new TcpChannel(port);
                ChannelServices.RegisterChannel(channel, true);
                server = (IPacmanPlatform)Activator.GetObject(
                    typeof(IPacmanPlatform),
                    "tcp://localhost:8086/ServerObject");

                client = new ClientObject(f, d, p);
                RemotingServices.Marshal(client, "ClientObject",
                    typeof(ClientObject));

                if (server == null)
                {
                    throw new SocketException();
                }
                else
                {
                    server.Register(port.ToString(), "tcp://localhost:" + port + "/ClientObject");
                }
            }
        }

        public void SendStateServer(State s)
        {
            server.GetKeyboardInput(s);
        }

        public int NumPlayers()
        {
            return client.NumPlayers();
        }


    public string GetPort() { return port.ToString(); }

        public void BroadcastChatMsg(string ChatMsg)
        {
            client.SendMessage(ChatMsg);
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
