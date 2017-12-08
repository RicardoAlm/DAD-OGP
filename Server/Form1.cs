using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {

        private pacman.Server server;
        private bool _leaderMissing = true;

        public Form1(int port)
        {
            InitializeComponent();
            new Thread(() =>
            {
                while (_leaderMissing){}
                server = new pacman.Server(port, portTextBox.Text);
            }).Start();
        }

        public Form1(int port, string leaderUrl)
        {
            InitializeComponent();
            server = new pacman.Server(port,leaderUrl);
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (portTextBox != null)
            {
                _leaderMissing = false;
                button1.Enabled = false;
            }
        }
    }
}
