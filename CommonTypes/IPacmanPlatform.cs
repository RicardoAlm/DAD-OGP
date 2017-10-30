using System;
using System.Collections.Generic;
using System.Text;

namespace pacman
{
    public interface IPacmanPlatform
    {
        void GetKeyboardInput(int player, string key);
        void Register(string nick, string url);
        void GetClients(string nick);
        int NamePlayer();


    }


}