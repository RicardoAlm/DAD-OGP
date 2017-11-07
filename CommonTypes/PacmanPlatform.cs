using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Linq;


namespace pacman
{
    public class PacmanServerObject : MarshalByRefObject, IPacmanPlatform
    {
 
        private int MSEC_PER_ROUND = 10000;
        private int MAX_PLAYERS;
        private Dictionary<int, string> playermoves;
        private Dictionary<string, PacmanClientObject> clients;
        private int players;
        private bool game = false;
        private int round;

        public PacmanServerObject()
        {
            playermoves = new Dictionary<int, string>();
            clients = new Dictionary<string, PacmanClientObject>();
            players = 0;
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer(); 
        
            t.Interval = MSEC_PER_ROUND;
            t.Tick += new EventHandler(timer_Tick);
            t.Start();
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
                while (players != MAX_PLAYERS) { }
                foreach (string client_nick in clients.Keys)
                {
                    clients[client_nick].GetServerClients(client_nick, clients);
                }
            }).Start();
        }



        public void GetKeyboardInput(int player, string key)
        {
            playermoves.Add(player, key);
                    
        }

        public Dictionary<int, string> PlayerMovements()
        {
            return playermoves;
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            if (game == false)
            {
                if (clients.Count == MAX_PLAYERS)
                {
                    game = true;
                    round = 0;
                    playermoves.Clear();
                }
            }
            else
            {

                round += 1;
            }
        }

        public int GetRound()
        {
            return round;
        }

        public bool StartGame()
        {
            return game;
        }
    }

    public class PacmanClientObject : MarshalByRefObject
    {
        Form _form;
        Delegate displaydelegate;
        private int _id;
        private List<int> msg_seq_vector;
        private Dictionary<string, PacmanClientObject> clients;
        private Dictionary<Dictionary <List<int>, string>, int> message_queue;

        public PacmanClientObject(Form form, Delegate d)
        {
            _form = form;
            displaydelegate = d;
        }

        //---------------------Server Side-----------------------------------------------
        public void GetServerClients(string nick, Dictionary<string, PacmanClientObject> cs)
        {
            int i=0;
            clients = cs;
            foreach (string n in cs.Keys)
            {
                if (nick.Equals(n))
                    _id = i;
                i++;
            }
            clients.Remove(nick);
            Debug.WriteLine("List Updated");
        }


        /// <summary>
        /// Receives a message from a machine with his correspondent id message vector.
        /// Verifies each position of the id vector received with its own. If there is a 
        /// positive diference in other position that its not the sending one, then we have
        /// pending messages and we add to our message queue.
        /// Else we display right away to the user.
        /// After this process we check the queue to see if we can display new messages to the user.
        /// </summary>
        /// <param name="msg">Message to display</param>
        /// <param name="id_vector">Id vector of the sending machine </param>
        /// <param name="id">Sending machine id</param>
        public void DisplayMessage(string msg, List<int> id_vector, int id)
        {
            new Thread(() =>
           {
               bool message_added_queue = false;
               for (int i = 0; i <= id_vector.Count; i++)
               {
                   if ((id_vector[i] - msg_seq_vector[i]) > 0 && i != id)
                   {
                       message_queue.Add(new Dictionary<List<int>, string> { { id_vector, msg } }, id);
                       message_added_queue = true;
                   }
               }
               if (!message_added_queue)
               {
                   msg_seq_vector[id]++;
                   _form.Invoke(displaydelegate, new object[] { msg });
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
            foreach (Dictionary<List<int>, string> d in message_queue.Keys)
            {
                foreach (List<int> l in d.Keys)
                    changes = VerifyMessage_aux(d[l], l, message_queue[d]);
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
        /// <param name="id_vector"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool VerifyMessage_aux(string msg, List<int> id_vector, int id)
        {
            bool  no_changes = false;
            for (int i = 0; i <= id_vector.Count; i++)
            {
                if ((id_vector[i] - msg_seq_vector[i]) > 0 && i != id)
                {
                    no_changes = true;
                }
            }
            if (!no_changes)
            {
                msg_seq_vector[id]++;
                _form.Invoke(displaydelegate, new object[] { msg });
                message_queue.Remove(new Dictionary<List<int>, string> { { id_vector, msg } });
                return true;
            }
            return false;
        }

        //---------------------Peer Side-----------------------------------------------


    public void SendMessage(string msg)
        {
            new Thread(() =>
            {
                foreach (PacmanClientObject c in clients.Values)
                {
                    msg_seq_vector[_id]++;
                    c.DisplayMessage(msg, msg_seq_vector, _id);
                }
            }).Start();
        }


    }
}
