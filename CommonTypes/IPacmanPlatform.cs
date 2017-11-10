using System;
using System.Collections.Generic;
using System.Text;

namespace pacman
{
    public interface IPacmanPlatform
    {
        void Register(string nick, string url);
        void GetKeyboardInput(State s);
    }

}