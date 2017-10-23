using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;

namespace pacman
{
    class Client
    {
        Form1 f = Program.GetForm();
        int player;


        TcpChannel channel = null;
        IPacmanPlatform obj;
        int key;
        string nickname;

        public void ConnectToServer()
        {
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);

            obj = (IPacmanPlatform)Activator.GetObject(
                typeof(IPacmanPlatform),
                "tcp://localhost:8086/PacmanRemoteObject");

            player = obj.NamePlayer();
            //we need number of players to create them in form
        }

        public void SendInput()
        {
            obj.GetKeyboardInput(f.GetKeyInput(),player);
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

        
    }



}
