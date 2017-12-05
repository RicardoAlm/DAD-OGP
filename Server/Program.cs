using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
        
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Debug.WriteLine("Launch!!!");
            if (args.Length==0)
                {
                    Application.Run(new Form1(0));
                }
            else
                {
                if (args[1].Equals("1"))
                {
                    Console.WriteLine("Launch!!!");
                    Application.Run(new Form1(Int32.Parse(args[0]), ""));
                }
                else
                {
                    Application.Run(new Form1(Int32.Parse(args[0]), args[1]));
                }
            }
        }
    }
}
