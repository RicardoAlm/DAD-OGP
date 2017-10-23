using System;
using System.Collections.Generic;
using System.Text;

namespace pacman
{
    public interface IPacmanPlatform
    {
        void GetKeyboardInput(string key, int player);

        int NamePlayer();


    }


}