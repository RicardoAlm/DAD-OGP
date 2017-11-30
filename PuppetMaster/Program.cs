using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class Program
    {
        private static PuppetMaster _puppetMaster;

        static void Main(string[] args)
        {
            _puppetMaster = new PuppetMaster();
            string path = "C:\\Users\\jp_s\\Documents\\Dad\\DAD-OGP\\scripts\\";
            Console.WriteLine("Enter the name of the script: ");
            string fileName = Console.ReadLine();
            string[] script = System.IO.File.ReadAllLines(path + fileName);
            foreach (string line in script)
            {
                _puppetMaster.Execute(line);
            }
            Console.ReadLine();
            _puppetMaster.KillAllProcess();
        }
    }
}
