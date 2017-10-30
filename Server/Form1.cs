using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        private pacman.Server server;

        public Form1()
        {
            InitializeComponent();
            server = new pacman.Server();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
