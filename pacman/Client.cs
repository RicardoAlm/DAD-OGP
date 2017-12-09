using System;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace pacman
{
    class Client 
    {   
        private TcpChannel channel = null;
        private Tuple<string, IPacmanPlatform> server;
        private ClientObject client;
        public int player { get; private set; }

        public Client(Form1 form, Delegate d, Delegate p, int portServer, int portClient)
        {
            Debug.WriteLine("Connecting to server...");
            ConnectToServer(form, d, p, portServer, portClient);
            Debug.WriteLine("Connected to server");
        }

        public void ConnectToServer(Form f, Delegate d, Delegate p, int portServer, int portClient)
        {
            if (portClient == 0)
            {
                Random rnd = new Random();
                portClient = rnd.Next(49152, 65535);

                if (!CheckAvailableServerPort(portClient))
                {
                    //TODO -> throw new Exception
                }
            }
            client = new ClientObject(f, d, p);
            channel = new TcpChannel(portClient);
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(client, "ClientObject",
                typeof(ClientObject));

            String serverUrl = "tcp://localhost:" + portServer + "/ServerObject";
            server = new Tuple<string, IPacmanPlatform> (serverUrl,(IPacmanPlatform)Activator.GetObject(
                typeof(IPacmanPlatform),serverUrl));

            if (server == null)
            {
                throw new SocketException();
            }
            server.Item2.Register(portClient.ToString(), "tcp://localhost:" + portClient + "/ClientObject");
        }

        public void newLeader(string serverUrl)
        {
            server = new Tuple<string, IPacmanPlatform>(serverUrl, (IPacmanPlatform)Activator.GetObject(
                typeof(IPacmanPlatform), serverUrl));
        }

        public string GetServerUrl()
        {
            return server.Item1;
        }

        public void SendStateServer(State s)
        {
         //   string urlServer = GetServerUrl();
            try
            {
                Debug.WriteLine("Tentar enviar");
                server.Item2.GetKeyboardInput(s);
                Debug.WriteLine("Consegui");
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.StackTrace);
             //   while (urlServer.Equals(server.Item1)){ }
            //    new server.Item2.GetKeyboardInput(s);
            }
        }

        public int NumPlayers()
        {
            return client.NumPlayers();
        }

        public void BroadcastChatMsg(string ChatMsg)
        {
            client.SendMessage(ChatMsg);
        }

        public bool GetFreeze()
        {
            return client._freeze;
        }

        public string GetMove()
        {
            if (client._script.Length !=0)
            {
                return client.GetScriptMove();
            }
            return "";
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
