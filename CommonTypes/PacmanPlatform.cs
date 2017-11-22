using System;
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

        private int MSEC_PER_ROUND = 50;
        private int MAX_PLAYERS;
        private readonly List<string> _urls;
        private readonly List<State> _queueStates;
        private readonly Dictionary<string, PacmanClientObject> _clients;
        private int _players;
        private readonly State _board;
        private bool _gameStart;
        private bool movementRed;
        private bool movementYellow;
        private bool movementPinkX;
        private bool movementPinkY;

        public PacmanServerObject()
        {
            _gameStart = false;
            _queueStates = new List<State>();
            _clients = new Dictionary<string, PacmanClientObject>();
            _urls = new List<string>();
            _players = 0;
            _board = new State();
            _board.CoordX = new List<int>();
            _board.CoordY = new List<int>();
            _board.GhostX = new List<int>();
            _board.GhostY = new List<int>();
            _board.Alive = new List<bool>();
            movementRed = true;
            movementYellow = true;
            movementPinkX = true;
            movementPinkY = true;
        }

        public void Register(string nick, string url)
        {
            Debug.WriteLine("Registing Client...");
            if (!_clients.ContainsKey(nick))
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
                    lock (_clients)
                    {
                        _clients.Add(nick, remoteObj);
                        _urls.Add(url);
                        _players++;
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
                while (_players != MAX_PLAYERS) { Thread.Sleep(1); } 
                foreach(string clientNick in _clients.Keys)
                {
                    _board.Id = id;
                    _board.Alive.Insert(id,true);
                    _board.CoordX.Insert(id, 8);
                    _board.CoordY.Insert(id, (id + 1) * 40);
                    _clients[clientNick].GetServerClients(clientNick, _urls, id);
                    id++;
                }
                _gameStart = true;
                _board.GhostX.Insert(0,180);
                _board.GhostY.Insert(0,73);
                _board.GhostX.Insert(1,221);
                _board.GhostY.Insert(1,273);
                _board.GhostX.Insert(2,301);
                _board.GhostY.Insert(2,72);
                
            }).Start();
        }


        //--- Server methods ---
        
        public void GetKeyboardInput(State s) {
            _queueStates.Add(s);
        }

   

        public void WaitForClientsInput()
        {
            _board.Round = 0;
            new Thread(() =>
            {
                while (!_gameStart){ Thread.Sleep(1); }
                while (true)
                    {
                        Thread.Sleep(MSEC_PER_ROUND); //+delay?
                        IncrementePosition();
                        _queueStates.Clear();
                        foreach (PacmanClientObject c in _clients.Values)
                        {
                            c.SendState(_board);
                            c.MoveTheGame();
                        }
                    }
            }).Start();
        }

        public bool Walls_collision_pacman(int x,int y)
        {
            //wall1 x=88 Y=40 w15 h95
            //wall2 x=248 y=40 w15 h95
            //wall3 x=128 y=240 w15 h95
            //wall4 x=288 y=240 w15 h95
            //redghost x=180 y=73 w30 h30
            //yellowghost x=221 y=273 w30 h30
            //pinkghost x=301,y=72 w30 h30
            //pacman x=8 Y=40 w25 h25
            //-25 + 15
            //hit first wall
            if (x > 63 & x < 103 & y < 135)
            {
                return true;
            }

            //hit second wall
            if (x > 223 & x < 263 & y < 135)
            {
                return true;
            }

            //hit third wall
            if (x > 103 & x < 143 & y > 215)
            {
                return true;
            }

            //hit fourth wall
            if (x > 263 & x < 303 & y < 215)
            {
                return true;
            }            
            return false;
        }

        public bool Ghosts_collision_pacman(int x, int y)
        {            
            //hit redGhost
            if (x > _board.GhostX[0] - 25 & x < _board.GhostX[0] + 30 & y < _board.GhostY[0] + 30 & y > _board.GhostY[0])
            {
                return true;
            }

            //hit yellowGhost
            if (x > _board.GhostX[1] - 25 & x < _board.GhostX[1] + 30 & y < _board.GhostY[1] + 30 & y > _board.GhostY[1])
            {
                return true;
            }

            //hit pinkGhost
            if (x > _board.GhostX[2] - 25 & x < _board.GhostX[2] + 30 & y < _board.GhostY[2] + 30 & y > _board.GhostY[2])
            {
                return true;
            }
            return false;
        }

        public bool Walls_collision_redghost(int x)
        {            
            //hit first wall
            if (x < 103)
            {
                movementRed=true;
            }

            //hit second wall
            if (x > 218)
            {
                movementRed=false;                
            }
            return movementRed;
        }

        public bool Walls_collision_yellowghost(int x)
        {            
            //hit third wall
            if (x < 143)
            {
                movementYellow = true;
            }

            //hit fourth wall
            if (x > 258)
            {
                movementYellow = false;
            }
            return movementYellow;
        }
        
        public bool Walls_collision_pinkghostX(int x, int y)
        {
            int boardRight = 320;
            int boardLeft = 0;
            
            if (x > boardRight )//- 15)
            {
                movementPinkX = !movementPinkX;
            }
            if (x < boardLeft + 15)
            {
                movementPinkX = !movementPinkX;
            }

            //hit first wall
            if (x > 58 & x < 103 & y < 135)
            {
                movementPinkX = !movementPinkX;
            }

            //hit second wall
            if (x > 218 & x < 263 & y < 135)
            {
                movementPinkX = !movementPinkX;
            }

            //hit third wall
            if (x > 98 & x < 143 & y > 210)
            {
                movementPinkX = !movementPinkX;
            }

            //hit fourth wall
            if (x > 258 & x < 303 & y > 210)
            {
                movementPinkX = !movementPinkX;
            }
            return movementPinkX;
        }

        public bool Walls_collision_pinkghostY(int y)
        {
            int boardTop = 40;
            int boardBottom = 320;
           
            if (y < boardTop)
            {
                movementPinkY = !movementPinkY;
            }
            if (y + 30 > boardBottom - 2)
            {
                movementPinkY = !movementPinkY;
            }
            return movementPinkY;
        }

        public void IncrementePosition()
        {
            int speed = 5;
            int boardRight = 320;
            int boardBottom = 320;
            int boardLeft = 0;
            int boardTop = 40;
            foreach (State s in _queueStates)
            {
                // Debug.WriteLine("iD:" + s.Id + "Key:" + s.Key);
                // Debug.WriteLine("BoardCount:"+_board.CoordY.Count);

                //checking first and after cause ghost can hit player too
                if (Ghosts_collision_pacman(_board.CoordX[s.Id], _board.CoordY[s.Id]))
                {
                    _board.Alive[s.Id] = false;
                }


                if (_board.Alive[s.Id] == true)
                {
                    if (s.Key.Equals("up"))
                    {
                        if (!(_board.CoordY[s.Id] - speed < boardTop)) { _board.CoordY[s.Id] -= speed; }
                    }
                    if (s.Key.Equals("down"))
                    {
                        if (!(_board.CoordY[s.Id] + speed > boardBottom)) { _board.CoordY[s.Id] += speed; }
                    }
                    if (s.Key.Equals("left"))
                    {
                        if (!(_board.CoordX[s.Id] - speed < boardLeft)) { _board.CoordX[s.Id] -= speed; }

                    }
                    if (s.Key.Equals("right"))
                    {
                        if (!(_board.CoordX[s.Id] + speed > boardRight)) { _board.CoordX[s.Id] += speed; }

                    }
                    if (s.Key.Equals("")) { }

                    //check if hits wall and ghost if does kill it
                    if (Walls_collision_pacman(_board.CoordX[s.Id], _board.CoordY[s.Id]) || Ghosts_collision_pacman(_board.CoordX[s.Id], _board.CoordY[s.Id]))
                    {
                        _board.Alive[s.Id] = false;
                    }
                }

                
            //////////////GHOSTS//////////////    
            }//redghost
            if (Walls_collision_redghost(_board.GhostX[0]))
            {
                _board.GhostX[0] = _board.GhostX[0] + speed;                
            }
            else
            {
                _board.GhostX[0] = _board.GhostX[0] - speed;
            }//yellowghost
            if (Walls_collision_yellowghost(_board.GhostX[1]))
            {
                _board.GhostX[1] = _board.GhostX[1] + speed;
            }else
            {
                _board.GhostX[1] = _board.GhostX[1] - speed;
            }
            //pinkghostX
            if (Walls_collision_pinkghostX(_board.GhostX[2], _board.GhostY[2]))
            {
                _board.GhostX[2] = _board.GhostX[2] + speed;
            }
            else
            {
                _board.GhostX[2] = _board.GhostX[2] - speed;
            }//pinkghostY
            if (Walls_collision_pinkghostY(_board.GhostY[2]))
            {
                _board.GhostY[2] = _board.GhostY[2] + speed;
            }
            else
            {
                _board.GhostY[2] = _board.GhostY[2] - speed;
            }

            _board.Round++;
        }

        

    }


    public class PacmanClientObject : MarshalByRefObject
    {
        readonly Form _form;
        readonly Delegate _displaydelegate;
        readonly Delegate _drawpDelegate;
        private int _id;
        private bool _gameReady;
        private List<int> _msgSeqVector;
        private readonly List <PacmanClientObject> _clients;
        private readonly Dictionary<Dictionary <List<int>, string>, int> _messageQueue;



        public PacmanClientObject(Form form, Delegate d, Delegate p)
        {
            _form = form;
            _displaydelegate = d;
            _drawpDelegate = p;
            _clients = new List<PacmanClientObject>();
            _messageQueue = new Dictionary<Dictionary<List<int>, string>, int>();
            _gameReady = false;
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

        public void SendState(State s)
        {
            s.Id = _id;
            _form.Invoke(_drawpDelegate, new object[] { s });
        }

        public void MoveTheGame()
        {
            _gameReady = true;
        }

        public bool IsGameReady()
        {
            return _gameReady;
        }



        public int NumPlayers()
        {
            return _clients.Count + 1;
        }
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

    [Serializable]
    public class State
    {

        public State() { }

        public int Id { get; set; }
        public int Round { get; set; }
        public List<int> CoordX { get; set; }
        public List<int> CoordY { get; set; }
        public List<int> GhostX { get; set; }
        public List<int> GhostY { get; set; }
        public string Key { get; set; }
        public List<bool> Alive { get; set; }
    }

}
