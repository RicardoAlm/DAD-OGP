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
            //DifFiles(); -> para testar
           // Console.ReadLine();
            _puppetMaster.KillAllProcess();
        }

        private static void DifFiles()
        {
            string[] s1 = System.IO.File.ReadAllLines("C:\\Users\\jp_s\\Documents\\Dad\\DAD-OGP\\scripts\\C2.csv");
            string[] s2 = System.IO.File.ReadAllLines("C:\\Users\\jp_s\\Documents\\Dad\\DAD-OGP\\scripts\\debug.csv");
            bool dif = false;

            for (int i = 0; i < s1.Length; i++)
            {
                if (!s2[i].Equals(s1[i]))
                {
                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("MyFile:" + s2[i]);
                    Console.WriteLine("ScriptFile:" + s1[i]);
                    Console.WriteLine("-------------------------------------------");
                    dif = true;
                }
            }
            if (!dif)
            {
                Console.WriteLine("No Diference!");
            }
        }
    }
}
