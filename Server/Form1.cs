using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Server
{
    public partial class Form1 : Form
    {
        private bool leaderMissing=true;
        private pacman.Server server;

        public Form1(int port,string leaderPort)
        {
            InitializeComponent();
            server = new pacman.Server(port, leaderPort);
        }
        public Form1(int port)
        {
            InitializeComponent();
            new Thread(() =>
            {
                while (leaderMissing)
                {

                }
                if (textBox1.Text != null)
                    server = new pacman.Server(port, textBox1.Text);
            }).Start();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void MaxPlayers_TextChanged(object sender, EventArgs e)
        {

        }

        private void Register_Click(object sender, EventArgs e)
        {
            if(MaxPlayers.Text != null)
                server.SetMAXPLAYERS(Int32.Parse(MaxPlayers.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            leaderMissing = false;
        }
    }
}
