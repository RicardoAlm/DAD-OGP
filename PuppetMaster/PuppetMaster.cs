using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using pacman;

namespace PuppetMaster
{
    class PuppetMaster
    {
        private ServerObject _server;
        private Dictionary<string, ClientObject> _clients = new Dictionary<string, ClientObject>();
        private Dictionary<string, PcsRemote> _kill = new Dictionary<string, PcsRemote>();

        public PuppetMaster()
        {
        }

        public void Execute(string s)
        {
            string[] function = s.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            if (function[0].Equals("StartServer"))
            {
                StartServer(function[1], function[2], function[3], Int32.Parse(function[4]), Int32.Parse(function[5]));
            }
            else if (function[0].Equals("StartClient"))
            {
                if (function.Length == 6)
                {
                    StartClient(function[1], function[2], function[3], Int32.Parse(function[4]),
                        Int32.Parse(function[5]), "");
                }
                if (function.Length == 7)
                {
                    StartClient(function[1], function[2], function[3], Int32.Parse(function[4]),
                        Int32.Parse(function[5]), function[7]);
                }
            }
        }

        public void StartServer(string pid, string pcsUrl, string serverUrl, int msecPerRound, int numPlayer)
        {
            ConnectPcsServer(pcsUrl, pid);
            _server = (ServerObject)Activator.GetObject(
                typeof(ServerObject),
                serverUrl);
        }

        public void StartClient(string pid, string pcsUrl, string clientUrl, int msecPerRound, int numPlayer,
            string fileName)
        {
            ConnectPcsClient(pcsUrl, pid);
            _clients.Add(pid, (ClientObject)Activator.GetObject(
                typeof(ClientObject),
                clientUrl));
        }

        public void ConnectPcsServer(string url, string id)
        {
            PcsRemote serverPcs = (PcsRemote) Activator.GetObject(
                typeof(PcsRemote),
                url);
            if (serverPcs == null)
            {
                throw new SocketException();
            }
            else
            {
                _kill.Add(id,serverPcs);
                serverPcs.LaunchServer();
            }
        }
        public void ConnectPcsClient(string url, string id)
        {
            PcsRemote clientPcs = (PcsRemote)Activator.GetObject(
                typeof(PcsRemote),
                url);
            if (clientPcs == null)
            {
                throw new SocketException();
            }
            else
            {
                _kill.Add(id, clientPcs);
                clientPcs.LaunchClient();
            }
        }

        public void KillAllProcess()
        {
            foreach (PcsRemote pcs in _kill.Values)
            {
                pcs.KillProcess();
            }
        }
    }
}
