using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Threading;

namespace pacman
{
    public class PacmanServerObject : MarshalByRefObject, IPacmanPlatform
    {

        private int MSEC_PER_ROUND;
        private int MAX_PLAYERS;
        private Dictionary<int, string> playermoves;
        private Dictionary<string, PacmanClientObject> clients;
        private int players;
        private int player;

        public PacmanServerObject()
        {
            playermoves = new Dictionary<int, string>();
            clients = new Dictionary<string, PacmanClientObject>();
            players = 0;
        }

        public void Register(string nick, string url)
        {
            Debug.WriteLine("Registing Client...");
            if (!clients.ContainsKey(nick))
            {
                Debug.WriteLine("Client name available");
                PacmanClientObject remoteObj = (PacmanClientObject)Activator.GetObject(
                    typeof(PacmanClientObject),
                    url);
                if (remoteObj == null)
                    System.Console.WriteLine("Could not locate server");
                else
                {
                    Debug.WriteLine("Adding Client to List...");
                    lock (clients)
                    {
                        clients.Add(nick, remoteObj);
                        players++;
                    }
                    Debug.WriteLine("Client Added");
                }
            }
            else
            {
                //TODO: Exception->nick already exists
            }
        }

        public void SetMAXPLAYERS(int max)
        {
            MAX_PLAYERS = max;
            new Thread(() => 
                {
                    while (players != MAX_PLAYERS)
                    {

                    }
                });
        }



        public void GetKeyboardInput(int player, string key)
        {
            playermoves.Add(player, key);            
        }

        public int NamePlayer()
        {
            
            player++;
            // check if player < NUM_PLAYERS
            return player;
        }


        //alterar para thread pool? 
        public void GetClients(string nick)
        {
            new Thread(() =>
            {
                clients[nick].GetServerClients(clients, nick);
            });
        }

        /*public hashtable MovePlayer()
         * {
         *      MSEC_PER_ROUND after this time
         *      send to every client the moves of each player (the client will be constantly trying to get this information)
         *      each client will move all players based on playermoves
                
         * 
         * }
         * */
    }

    public class PacmanClientObject : MarshalByRefObject
    {
        Form _form;
        Delegate displaydelegate;
        private Dictionary<string, PacmanClientObject> clients;

        public PacmanClientObject(Form form, Delegate d)
        {
            _form = form;
            displaydelegate = d;
        }

        //---------------------Server Side-----------------------------------------------
        //TODO -> Remove Myself From The List
        public void GetServerClients(Dictionary<string, PacmanClientObject> cs, string nick)
        {
            clients = cs;
            clients.Remove(nick);
        }

        public void DisplayMessage(string msg)
        {
            _form.Invoke(displaydelegate, new object[] { msg });
        }
        //---------------------Peer Side-----------------------------------------------
        

        public void SendMessage(string msg)
        {
            new Thread(() =>
            {
                foreach (PacmanClientObject c in clients.Values)
                {
                    c.DisplayMessage(msg);
                }
            });
        }


    }
}
