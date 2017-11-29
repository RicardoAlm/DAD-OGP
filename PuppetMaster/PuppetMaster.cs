using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class PuppetMaster
    {
        public PuppetMaster() { }

        public void Execute(string s)
        {
            string[] function = s.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            if (function[0].Equals("StartServer"))
            {
                StartServer(function[1], function[2], function[3], Int32.Parse(function[4]), Int32.Parse(function[5]));
            }
            else if (function[0].Equals("StartClient"))
            {
                if (function.Length == 6)
                {
                    StartClient(function[1], function[2], function[3], Int32.Parse(function[4]),
                        Int32.Parse(function[5]), "");
                }
                if (function.Length == 7)
                {
                    StartClient(function[1], function[2], function[3], Int32.Parse(function[4]),
                        Int32.Parse(function[5]), function[7]);
                }
            }
        }

        public void StartServer(string pid, string pcsUrl, string serverUrl, int msecPerRound, int numPlayer)
        {
            Console.WriteLine("ServerMagic");
        }

        public void StartClient(string pid, string pcsUrl, string clientUrl, int msecPerRound, int numPlayer,
            string fileName)
        {
            
        }


    }
}
