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
    public class ServerObject : MarshalByRefObject, IPacmanPlatform
    {
        public int MsecPerRound { get; set; } = 50;
        private int MAX_PLAYERS;
        private readonly List<string> _urls;
        private readonly List<State> _queueStates;
        private readonly Dictionary<string, ClientObject> _clients;
        private int _players;
        private readonly State _board;
        private bool _gameStart;
        private bool movementRed;
        private bool movementYellow;
        private bool movementPinkX;
        private bool movementPinkY;

        public ServerObject()
        {
            _gameStart = false;
            _queueStates = new List<State>();
            _clients = new Dictionary<string, ClientObject>();
            _urls = new List<string>();
            _players = 0;
            _board = new State();
            _board.CoordX = new List<int>();
            _board.CoordY = new List<int>();
            _board.GhostX = new List<int>();
            _board.GhostY = new List<int>();
            _board.Score = new List<int>();
            _board.Alive = new List<bool>();
            _board.CoinsEaten = new List<bool>();
            _board.GameRunning = true;
            movementRed = true;
            movementYellow = true;
            movementPinkX = true;
            movementPinkY = true;
        }

        public void Register(string nick, string url)
        {
            if (!_clients.ContainsKey(nick))
            {
                Debug.WriteLine("Client name available");
                Debug.WriteLine(url);
                ClientObject remoteObj = (ClientObject)Activator.GetObject(
                    typeof(ClientObject),
                    url);
                Debug.WriteLine("Adding Client to List...");
                lock (_clients)
                {
                    _clients.Add(nick, remoteObj);
                    _urls.Add(url);
                    _players++;
                }
                Debug.WriteLine("Client Added");
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
                    _board.Score.Insert(id, 0);
                    _board.CoordX.Insert(id, 8);
                    _board.CoordY.Insert(id, (id + 1) * 40);
                    _clients[clientNick].GetServerClients(clientNick, _urls, id);
                    id++;
                }
                for (int i = 0; i < 60; i++)
                {
                    _board.CoinsEaten.Insert(i, true);
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
            bool running = true;
            new Thread(() =>
            {
                while (!_gameStart){ Thread.Sleep(1); }
                while (running)
                    {
                        Thread.Sleep(MsecPerRound); //+delay?
                        GameFinish();
                        IncrementePosition();
                        _queueStates.Clear();
                        foreach (ClientObject c in _clients.Values)
                        {
                            c.SendState(_board);
                            c.MoveTheGame();
                        }
                        if (_board.GameRunning == false)
                        {
                            running = false;
                        }
                    }
            }).Start();
        }

        public void Coins_collision_pacman(int x,int y,int id)
        {
            for (int i = 1; i < 9; i++)
            {
                if (_board.CoinsEaten[i - 1] == true && x > 0 && x < 23 && y < 40 * i + 15 && y > 40 * i - 25)
                {
                        _board.CoinsEaten[i - 1] = false;
                        _board.Score[id] += 1;
                }

                else if (_board.CoinsEaten[i + 7] == true && x > 23 && x < 63 && y < 40 * i + 15 && y > 40 * i - 25)
                {
                        _board.CoinsEaten[i + 7] = false;
                        _board.Score[id] += 1;
                }

                else if (_board.CoinsEaten[i + 12] == true && i > 3 && x > 63 && x < 103 && y < 40 * i + 15 && y > 40 * i - 25)
                {
                        _board.CoinsEaten[i + 12] = false;
                        _board.Score[id] += 1;
                }
                
                else if (_board.CoinsEaten[i + 20] == true && i < 6 && x > 103 && x < 143 && y < 40 * i + 15 && y > 40 * i - 25)
                {  
                            _board.CoinsEaten[i + 20] = false;
                            _board.Score[id] += 1; 
                }

                else if (_board.CoinsEaten[i + 25] == true && x > 143 && x < 183 && y < 40 * i + 15 && y > 40 * i - 25)
                {
                        _board.CoinsEaten[i + 25] = false;
                        _board.Score[id] += 1;
                }

                else if (_board.CoinsEaten[i + 33] == true && x > 183 && x < 223 && y < 40 * i + 15 && y > 40 * i - 25)
                {
                        _board.CoinsEaten[i + 33] = false;
                        _board.Score[id] += 1;
                }

                else if (_board.CoinsEaten[i + 38] == true && i > 3 && x > 223 && x < 263 && y < 40 * i + 15 && y > 40 * i - 25)
                {
                            _board.CoinsEaten[i + 38] = false;
                            _board.Score[id] += 1;
                }

                else if (_board.CoinsEaten[i + 46] == true && i < 6 && x > 263 && x < 303 && y < 40 * i + 15 && y > 40 * i - 25)
                {
                            _board.CoinsEaten[i + 46] = false;
                            _board.Score[id] += 1;
                }

                else if (_board.CoinsEaten[i + 51] == true && x > 303 && x < 343 && y < 40 * i + 15 && y > 40 * i - 25)
                {
                         _board.CoinsEaten[i + 51] = false;
                        _board.Score[id] += 1;
                }
            }
        }

        public void GameFinish()
        {
            if(!_board.CoinsEaten.Any(c => c == true) || !_board.Alive.Any(c => c == true))
            {

                int indexMax = !_board.Score.Any() ? -1 : _board.Score.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;

                _board.Winner = indexMax.ToString();
                _board.GameRunning = false;
            }
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
            if (x > 63 && x < 103 && y < 135)
            {
                return true;
            }

            //hit second wall
            else if (x > 223 && x < 263 && y < 135)
            {
                return true;
            }

            //hit third wall
            else if (x > 103 && x < 143 && y > 215)
            {
                return true;
            }

            //hit fourth wall
            else if (x > 263 && x < 303 && y > 215)
            {
                return true;
            }            
            return false;
        }

        public bool Ghosts_collision_pacman(int x, int y)
        {            
            //hit redGhost
            if (x > _board.GhostX[0] - 25 && x < _board.GhostX[0] + 30 && y < _board.GhostY[0] + 30 && y > _board.GhostY[0] - 25)
            {
                return true;
            }

            //hit yellowGhost
            else if (x > _board.GhostX[1] - 25 && x < _board.GhostX[1] + 30 && y < _board.GhostY[1] + 30 && y > _board.GhostY[1] - 25)
            {
                return true;
            }

            //hit pinkGhost
            else if (x > _board.GhostX[2] - 25 && x < _board.GhostX[2] + 30 && y < _board.GhostY[2] + 30 && y > _board.GhostY[2] - 25)
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
            else if (x > 218)
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
            else if (x > 258)
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
            else if (x < boardLeft + 15)
            {
                movementPinkX = !movementPinkX;
            }

            //hit first wall
            else if (x > 58 && x < 103 && y < 135)
            {
                movementPinkX = !movementPinkX;
            }

            //hit second wall
            else if (x > 218 && x < 263 && y < 135)
            {
                movementPinkX = !movementPinkX;
            }

            //hit third wall
            else if (x > 98 && x < 143 && y > 210)
            {
                movementPinkX = !movementPinkX;
            }

            //hit fourth wall
            else if (x > 258 && x < 303 && y > 210)
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
            else if (y + 30 > boardBottom - 2)
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
                    else if (s.Key.Equals("down"))
                    {
                        if (!(_board.CoordY[s.Id] + speed > boardBottom)) { _board.CoordY[s.Id] += speed; }
                    }
                    else if (s.Key.Equals("left"))
                    {
                        if (!(_board.CoordX[s.Id] - speed < boardLeft)) { _board.CoordX[s.Id] -= speed; }

                    }
                    else if (s.Key.Equals("right"))
                    {
                        if (!(_board.CoordX[s.Id] + speed > boardRight)) { _board.CoordX[s.Id] += speed; }

                    }
                    else if (s.Key.Equals("")) { }

                    //check if hits wall and ghost if does kill it
                    if (Walls_collision_pacman(_board.CoordX[s.Id], _board.CoordY[s.Id]) || Ghosts_collision_pacman(_board.CoordX[s.Id], _board.CoordY[s.Id]))
                    {
                        _board.Alive[s.Id] = false;
                    }
                    //
                    if (!s.Key.Equals(""))
                    {
                        Coins_collision_pacman(_board.CoordX[s.Id], _board.CoordY[s.Id],s.Id);
                    }
                }
            }   
            //////////////GHOSTS//////////////    
            //redghost
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

        public bool GetGameStart()
        {
            return _gameStart;
        }

    }


    public class ClientObject : MarshalByRefObject
    {
        readonly Form _form;
        readonly Delegate _displaydelegate;
        readonly Delegate _drawpDelegate;
        public int _id { get; set; }
        private bool _gameReady;
        private List<int> _msgSeqVector;
        private readonly List <ClientObject> _clients;
        private readonly Dictionary<Dictionary <List<int>, string>, int> _messageQueue;
        private readonly Dictionary<int, State> _allRoundsStates;
        public string[] _script { get; set; }
        public bool _freeze { get; set; }


        public ClientObject(Form form, Delegate d, Delegate p)
        {
            _form = form;
            _displaydelegate = d;
            _drawpDelegate = p;
            _clients = new List<ClientObject>();
            _messageQueue = new Dictionary<Dictionary<List<int>, string>, int>();
            _gameReady = false;
            _allRoundsStates = new Dictionary<int, State>();
            _script= new string[]{};
            _freeze = false;
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
                    ClientObject remoteObj = (ClientObject)Activator.GetObject(
                        typeof(ClientObject),
                        urls[i]);
                    _clients.Add(remoteObj);
                }

            }
            Debug.WriteLine("List Updated");
        }

        //---Client Methods ---

        public void SendState(State s)
        {
            _allRoundsStates.Add(s.Round,s);
            s.Id = _id;
            if(!_freeze)
                _form.Invoke(_drawpDelegate, new object[] { s });
        }

        public void MoveTheGame()
        {
            _gameReady = true;
        }

        //-----------------------PuppetMaster-------------------------------------

        public string LocalState(int round)
        {
            while(!_allRoundsStates.ContainsKey(round))
            {
                Thread.Sleep(100); 
            }
            State s = _allRoundsStates[round];
            string localstate = "";
            for (int i = 0; i < s.GhostX.Count; i++)
            {
                localstate += "M, " + s.GhostX[i] + ", " + s.GhostY[i] + "\r\n";
            }

            for (int i = 0; i < s.CoordX.Count; i++)
            {
                if(s.Alive[i])
                    localstate += "P"+ i + ", P " + s.CoordX[i] + ", " + s.CoordY[i] + "\r\n";
                else
                    localstate += "P" + i + ", L " + s.CoordX[i] + ", " + s.CoordY[i] + "\r\n";
            }

            int coinX = 40;
            int coinY = 40;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (s.CoinsEaten[i])
                        localstate += "C, " + coinX + ", " + coinY + "\r\n";
                    coinY += 40;
                }
                coinY = 40;
                coinX += 40;
            }
            return localstate;
        }

        public void SendScript(string scriptName)
        {
            string path = "C:\\Users\\jp_s\\Documents\\Dad\\DAD-OGP\\scripts\\";
            _script = System.IO.File.ReadAllLines(path + scriptName);
        }

        public string GetScriptMove()
        {
            char[] del = { ',' };
            string[] c = _script[0].Split(del);
            _script = _script.Skip(1).ToArray(); //Eliminar 1 posição do array
            return c[1];
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
                            _messageQueue.Add(new Dictionary<List<int>, string> {{idVector, msg}}, id);
                            messageAddedQueue = true;
                        }
                    }
                    if (!messageAddedQueue)
                    {
                        if (!_freeze)
                        {
                            _msgSeqVector[id]++;
                            _form.Invoke(_displaydelegate, new object[] {msg});
                            VerifyMessage();
                        }
                        else
                        {
                            _messageQueue.Add(new Dictionary<List<int>, string> { { idVector, msg } }, id);
                        }
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
                foreach (ClientObject c in _clients)
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
        public int Id { get; set; }
        public int Round { get; set; }
        public List<int> CoordX { get; set; }
        public List<int> CoordY { get; set; }
        public List<int> GhostX { get; set; }
        public List<int> GhostY { get; set; }
        public List<int> Score { get; set; }
        public string Key { get; set; }
        public bool GameRunning { get; set; }
        public string Winner { get; set; }
        public List<bool> Alive { get; set; }
        public List<bool> CoinsEaten { get; set; }
    }

    public class PcsRemote: MarshalByRefObject
    {
        private Process _process;

        public void LaunchServer(string port)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "C:\\Users\\jp_s\\Documents\\Dad\\DAD-OGP\\Server\\bin\\Debug\\Server.exe",
                Arguments = port
            };
            _process = Process.Start(startInfo);
        }
        public void LaunchClient(string portServer, string portClient)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "C:\\Users\\jp_s\\Documents\\Dad\\DAD-OGP\\pacman\\bin\\Debug\\pacman.exe",
                Arguments = portServer + " " + portClient
            };
            _process = Process.Start(startInfo);
        }

        public void KillProcess()
        {
            _process.Kill();
        }

    }

}
