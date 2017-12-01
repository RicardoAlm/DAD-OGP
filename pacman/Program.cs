using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace pacman {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //string contents = "arg0:" + args[0] + "\nagr1:" + args[1];
            //DEBUG-> System.IO.File.WriteAllText(@"C:\Users\jp_s\Documents\Dad\DAD-OGP\scripts\debug.txt", contents);
            Application.Run(args.Length == 0
                ? new Form1(8086, 0)
                : new Form1(Int32.Parse(args[0]), Int32.Parse(args[1])));
        }
    }
}
