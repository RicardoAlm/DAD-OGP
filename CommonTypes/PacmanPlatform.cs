﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace pacman
{
    public class PacmanServerObject : MarshalByRefObject, IPacmanPlatform
    {
 
        private int MSEC_PER_ROUND;
        private int MAX_PLAYERS;
        private int ROUND;
        private List<string> urls;
        private Dictionary<int, string> playermoves;
        private Dictionary<string, PacmanClientObject> clients;
        private int players;
        private System.Windows.Forms.Timer timer1;

        public PacmanServerObject()
        {
            playermoves = new Dictionary<int, string>();
            clients = new Dictionary<string, PacmanClientObject>();
            urls = new List<string>();
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
                        urls.Add(url);
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

        public void SetMaxplayers(int max)
        {
            MAX_PLAYERS = max;
            new Thread(() =>
            {
                int id = 0;
                while (players != MAX_PLAYERS) { }
                foreach(string clientNick in clients.Keys)
                {
                    clients[clientNick].GetServerClients(clientNick, urls, id);
                    id++;
                }
            }).Start();
        }


        //--- Server methods ---
       
        public void GetKeyboardInput(int player, string key) {
            playermoves.Add(player, key);
        }

        public void WaitForClientsInput()
        {
   
        }

        public void ComputeUpdates() //calcula state
        {
            NextRound();
        }

        public void SendStateUpdate() { }

        public void NextRound() //So para o server o envia para clientes tambem?
        {
            this.ROUND++;
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
        readonly Form _form;
        readonly Delegate _displaydelegate;
        private int _id;
        private List<int> _msgSeqVector;
        private readonly List <PacmanClientObject> _clients;
        private readonly Dictionary<Dictionary <List<int>, string>, int> _messageQueue;
        public string playermove;
        public int ROUND;

        public PacmanClientObject(Form form, Delegate d)
        {
            _form = form;
            _displaydelegate = d;
            _clients = new List<PacmanClientObject>();
            _messageQueue = new Dictionary<Dictionary<List<int>, string>, int>();
        }

        //---------------------Server Side-----------------------------------------------
        public void GetServerClients(string nick, List<string> urls, int id)
        {
            _id = id;
            _msgSeqVector = new List<int>(urls.Count);
            for (int i = 0; i < urls.Count ; i++ )
            {
                _msgSeqVector.Insert(i,0);
                if (i != id)
                {
                    PacmanClientObject remoteObj = (PacmanClientObject)Activator.GetObject(
                        typeof(PacmanClientObject),
                        urls[i]);
                    _clients.Add(remoteObj);
                }
            }

            Debug.WriteLine(_clients.Count);
            Debug.WriteLine("List Updated");
        }

        //---Client Methods ---


        public void WaitForStateUpdate() { }
        //---------------------Peer Side-----------------------------------------------
        /// <summary>
        /// Receives a message from a machine with his correspondent id message vector.
        /// Verifies each position of the id vector received with its own. If there is a 
        /// positive diference in other position that its not the sending one, then we have
        /// pending messages and we add to our message queue.
        /// Else we display right away to the user.
        /// After this process we check the queue to see if we can display new messages to the user.
        /// </summary>
        /// <param name="msg">Message to display</param>
        /// <param name="idVector">Id vector of the sending machine </param>
        /// <param name="id">Sending machine id</param>
        public void DisplayMessage(string msg, List<int> idVector, int id)
        {
            new Thread(() =>
           {
               bool messageAddedQueue = false;
               for (int i = 0; i < idVector.Count; i++)
               {
                   if ((idVector[i] - _msgSeqVector[i]) > 0 && i != id)
                   {
                       _messageQueue.Add(new Dictionary<List<int>, string> { { idVector, msg } }, id);
                       messageAddedQueue = true;
                   }
               }
               if (!messageAddedQueue)
               {
                   _msgSeqVector[id]++;
                   _form.Invoke(_displaydelegate, new object[] { msg });
                   VerifyMessage();
               }
           }).Start();
        }

        /// <summary>
        /// Iterates the message queue. Sends each message with the sender id and vector id to the 
        /// VerifyMessage_aux to check if the message can be displayed to the user.
        /// If it was displayed we keep checking other messages and after we end this iteration, we 
        /// do a new one to check if we can display new messages that were missed in the priviouse iteration.
        /// </summary>
        public void VerifyMessage()
        {
            bool changes = false;
            foreach (Dictionary<List<int>, string> d in _messageQueue.Keys)
            {
                foreach (List<int> l in d.Keys)
                    changes = VerifyMessage_aux(d[l], l, _messageQueue[d]);
            }
            if (changes)
                VerifyMessage();
        }

        /// <summary>
        /// Verifies each position of the id vector  of the machine id with its own. If there is a 
        /// positive diference in other position that its not the sending one, then we 
        /// skip this message, because we still have pending messages.
        /// Else we display right away to the user.
        /// Return true if we displayed to the user.
        /// Return false if we didnt.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="idVector"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool VerifyMessage_aux(string msg, List<int> idVector, int id)
        {
            bool  noChanges = false;
            for (int i = 0; i <= idVector.Count; i++)
            {
                if ((idVector[i] - _msgSeqVector[i]) > 0 && i != id)
                {
                    noChanges = true;
                }
            }
            if (!noChanges)
            {
                _msgSeqVector[id]++;
                _form.Invoke(_displaydelegate, new object[] { msg });
                _messageQueue.Remove(new Dictionary<List<int>, string> { { idVector, msg } });
                return true;
            }
            return false;
        }




        public void SendMessage(string msg)
        {
            new Thread(() =>
            {
                foreach (PacmanClientObject c in _clients)
                {
                    _msgSeqVector[_id]++;
                    c.DisplayMessage(msg, _msgSeqVector, _id);
                }
            }).Start();
        }


    }
}
