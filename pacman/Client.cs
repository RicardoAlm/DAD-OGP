﻿using System;
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

namespace pacman
{
    class Client
    {
        private Form1 f; //=  Program.GetForm();

        public delegate void DelDisplay(string msg);
        private TcpChannel channel = null;
        private IPacmanPlatform server;
        private PacmanClientObject client;
        private int port;

        //private string nickname;
        private int player;

        public Client(Form1 form)
        {
            f = form;
            Debug.WriteLine("Connecting to server...");
            ConnectToServer();
            Debug.WriteLine("Connected to server");
            Debug.WriteLine("Updating Clients list...");
            server.GetClients(port.ToString());
            Debug.WriteLine("Clients list Updated");

        }

        public void ConnectToServer()
        {

            Random rnd = new Random();
            port = rnd.Next(49152, 65535);

            if (CheckAvailableServerPort(port))
            { 
                channel = new TcpChannel(port);
                ChannelServices.RegisterChannel(channel, true);
                server = (PacmanServerObject)Activator.GetObject(
                    typeof(PacmanServerObject),
                    "tcp://localhost:8086/PacmanServerObject");

                client = new PacmanClientObject(f, new DelDisplay(this.Display));
                RemotingServices.Marshal(client, "PacmanClientObject",
                    typeof(PacmanClientObject));

                if (server == null)
                {
                    throw new SocketException();
                }
                else
                {
                    server.Register(port.ToString(), "tcp://localhost:" + port + "/PacmanClientObject");
                }
            }

            
            //player = server.NamePlayer();
            //we need number of players to create them in form
        }

        public void SendInput()
        {
            server.GetKeyboardInput(player, f.GetKeyInput());
        }


        public void ReceiveState()
        {

        }

        public void MoveTheGame()
        {
            /*obj.sendInput
             * 
             * thread to obj.moveplayer() 
             * send to foRm what to do 
             * 

            */
        }

        public void BroadcastChatMsg(string ChatMsg)
        {

        }

        public void Display(string msg)
        {
            
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