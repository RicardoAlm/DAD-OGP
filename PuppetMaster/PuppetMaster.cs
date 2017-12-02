using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using pacman;

namespace PuppetMaster
{
    class PuppetMaster
    {
        private Tuple<string, ServerObject> _server;
        private readonly Dictionary<string, ClientObject> _clients = new Dictionary<string, ClientObject>();
        private readonly Dictionary<string, PcsRemote> _kill = new Dictionary<string, PcsRemote>();

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
                        Int32.Parse(function[5]), function[6]);
                }
            }
            else if (function[0].Equals("LocalState"))
                LocalState(function[1], Int32.Parse(function[2]));
        }

        public void StartServer(string pid, string pcsUrl, string serverUrl, int msecPerRound, int numPlayer)
        {
            string url = GetPort(serverUrl);
            ConnectPcsServer(pcsUrl, pid, url);
            _server = Tuple.Create(url, (ServerObject) Activator.GetObject(
                typeof(ServerObject),
                serverUrl+"Object"));
            try
            {
                _server.Item2.SetMaxplayers(numPlayer);
                _server.Item2.MsecPerRound = msecPerRound;
            }catch(Exception e) { Debug.WriteLine(e.ToString()) ;KillAllProcess();}
        }

        public void StartClient(string pid, string pcsUrl, string clientUrl, int msecPerRound, int numPlayer,
            string fileName)
        {
            Debug.WriteLine("StartClientPID:" + pid);
            ConnectPcsClient(pcsUrl, pid, _server.Item1, GetPort(clientUrl));
            ClientObject client = (ClientObject) Activator.GetObject(
                typeof(ClientObject),
                clientUrl + "Object");
            if (!fileName.Equals(""))
                client.SendScript(fileName);
            _clients.Add(pid, client);
        }

        public void ConnectPcsServer(string url, string id, string port)
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
                serverPcs.LaunchServer(port);
            }
        }
        public void ConnectPcsClient(string url, string id, string portServer, string portClient)
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
                clientPcs.LaunchClient(portServer, portClient);
            }
        }

        public void LocalState(string pid, int round)
        {
            string localState = _clients[pid].LocalState(round);
            Console.WriteLine(localState);
            System.IO.File.WriteAllText(@"C:\Users\jp_s\Documents\Dad\DAD-OGP\PuppetMaster\bin\Debug\LocalState" + pid + "_" + round+".txt",localState);
        }

        public string GetPort(string s)
        {
            char[] del = { ':', '/' };
            string[] str = s.Split(del);
            return str[4];
        }

        public void KillAllProcess()
        {
            foreach (PcsRemote pcs in _kill.Values.Reverse())
            {
                pcs.KillProcess();
            }
        }
    }
}
