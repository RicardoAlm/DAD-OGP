using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace pacman
{
    public class PacmanRemoteObject : MarshalByRefObject, IPacmanPlatform
    {

        int MSEC_PER_ROUND;

        Hashtable playermoves = new Hashtable();
        int player = 0;

        public PacmanRemoteObject()
        {
            
        }

        public void GetKeyboardInput(string key,int player)
        {
            playermoves.Add(player, key);

            
        }

        public int NamePlayer()
        {
            
            player++;
            // check if player < NUM_PLAYERS
            return player;
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
}
