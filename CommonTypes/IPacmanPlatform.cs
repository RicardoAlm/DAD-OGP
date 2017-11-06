using System;
using System.Collections.Generic;
using System.Text;

namespace pacman
{
    public interface IPacmanPlatform
    {
        void GetKeyboardInput(int player, string key, int RoundId);
        void Register(string nick, string url);
    }

}