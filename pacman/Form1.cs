﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace pacman {

    public partial class Form1 : Form
    {

        private List<PictureBox> pacmans;
        private bool sendMessage = false;
        private int _id;

        // direction player is moving in. Only one will be true
        bool goup;
        bool godown;
        bool goleft;
        bool goright;
        string keypressed;
        int dead ;
        bool ready =false;

        int boardRight = 320;
        int boardBottom = 320;
        int boardLeft = 0;
        int boardTop = 40;
        //player speed
        int speed = 5;

        int score = 0; int total_coins = 61;
        List<int> scores;
        //ghost speed for the one direction ghosts
        int ghost1 = 5;
        int ghost2 = 5;
        
        //x and y directions for the bi-direccional pink ghost
        int ghost3x = 5;
        int ghost3y = 5;


        Client client = null;
        public delegate void UpdateChat(string msg);
        public delegate void ReDrawPacman(State s);
        State newState;

        public Form1() {
            InitializeComponent();
            client = new Client(this, new UpdateChat(this.ChangeChat), new ReDrawPacman(this.Redraw));
            pacmans = new List<PictureBox>();
            label2.Visible = false;
            _id = 0;
            newState = new State();
            newState.GhostInvert = new List<bool>();
        }

        public void Redraw(State s)
        {
            if (s.Round == 1)
            {
                scores = new List<int>(Enumerable.Repeat(0, client.NumPlayers()));
                ready = true;
                _id = s.Id;

                
                for (int i = 0; i < client.NumPlayers(); i++)
                {
                    PictureBox pacman = new PictureBox();
                    pacman.BackColor = System.Drawing.Color.Transparent;
                    pacman.Image = Properties.Resources.Left;
                    pacman.Location = new System.Drawing.Point(s.CoordX[i], s.CoordY[i]);
                    pacman.Margin = new System.Windows.Forms.Padding(0);
                    pacman.Name = "pacman" + i.ToString();
                    pacman.Tag = "pacman";
                    pacman.Size = new System.Drawing.Size(25, 25);
                    pacman.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    pacman.TabIndex = 4;
                    pacman.TabStop = false;
                    this.Controls.Add(pacman);
                    pacmans.Insert(i, pacman);
                }
                for (int i = 0; i < 4; i++)
                {
                    newState.GhostInvert.Insert(i,s.GhostInvert[i]);
                }
            }
            else {
                DrawBoardPacmans(s);
                MoveGhost(s);

            }
            sendMessage = true;
        }

        public void MoveGhost(State s)
        {
            redGhost.Location = new System.Drawing.Point(s.GhostX[0], s.GhostY[0]);
            yellowGhost.Location = new System.Drawing.Point(s.GhostX[1], s.GhostY[1]);
            pinkGhost.Location = new System.Drawing.Point(s.GhostX[2], s.GhostY[2]);
        }

        //get input ***************
        public string GetKeyInput()
        {
            if (goup)
            {
                return "up";
            }
            if (godown)
            {
                return "down";
            }
            if (goleft)
            {
                return "left";
            }
            if (goright)
            {
                return "right";
            }

            return "";
        }

        private void keyisdown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Left) {
                goleft = true;
                //pacman.Image = Properties.Resources.Left;
            }
            if (e.KeyCode == Keys.Right) {
                goright = true;
                //pacman.Image = Properties.Resources.Right;
            }
            if (e.KeyCode == Keys.Up) {
                goup = true;
                //pacman.Image = Properties.Resources.Up;
            }
            if (e.KeyCode == Keys.Down) {
                godown = true;
                //pacman.Image = Properties.Resources.down;
                //DrawPacmans(); //here
            }
            if (e.KeyCode == Keys.Enter) {
                    tbMsg.Enabled = true; tbMsg.Focus();
               }
        }

        private void keyisup(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Left) {
                goleft = false;
            }
            if (e.KeyCode == Keys.Right) {
                goright = false;
            }
            if (e.KeyCode == Keys.Up) {
                goup = false;
            }
            if (e.KeyCode == Keys.Down) {
                godown = false;
            }
        }

        private void DrawBoardPacmans(State s) 
        {
            for (int i = 0; i < client.NumPlayers(); i++ )
            {
                PictureBox pacman = pacmans[i];
                pacman.Location = new System.Drawing.Point(s.CoordX[i], s.CoordY[i]);
                this.Controls.Add(pacman);
            }           
        }






        private void timer1_Tick(object sender, EventArgs e)
        {
            
            label1.Text = "Score: " + score;
            
            if (sendMessage)
            {
                
                newState.Id = _id;
                newState.Key = GetKeyInput();

                //check if phantoms hit wall////////////////////////////////////////////////////////
                if (redGhost.Bounds.IntersectsWith(pictureBox1.Bounds))
                {
                    newState.GhostInvert[0] = true;
                }
                else if (redGhost.Bounds.IntersectsWith(pictureBox2.Bounds))
                {
                    newState.GhostInvert[0] = false;
                }
                if (yellowGhost.Bounds.IntersectsWith(pictureBox3.Bounds))
                {
                    newState.GhostInvert[1] = true;
                }

                else if (yellowGhost.Bounds.IntersectsWith(pictureBox4.Bounds))
                {
                    newState.GhostInvert[1] = false;
                }

                if (pinkGhost.Left < boardLeft ||
                        pinkGhost.Left > boardRight ||
                        (pinkGhost.Bounds.IntersectsWith(pictureBox1.Bounds)) ||
                        (pinkGhost.Bounds.IntersectsWith(pictureBox2.Bounds)) ||
                        (pinkGhost.Bounds.IntersectsWith(pictureBox3.Bounds)) ||
                        (pinkGhost.Bounds.IntersectsWith(pictureBox4.Bounds)))
                {
                    newState.GhostInvert[2] = !newState.GhostInvert[2];
                    //ghost3x = -ghost3x;
                }
                if (pinkGhost.Top < boardTop || pinkGhost.Top + pinkGhost.Height > boardBottom - 2)
                {                    
                    newState.GhostInvert[3] = !newState.GhostInvert[3];
                    //ghost3y = -ghost3y;
                }
                ///////////////////////////////////////////////////////////////////////////////////

                client.SendStateServer(newState);
                sendMessage=false;
            }

            keypressed = "";
            


            if (ready)
            {

                

                /*redGhost.Left += ghost1;
                yellowGhost.Left += ghost2;
                // if the red ghost hits the picture box 4 then wereverse the speed
                if (redGhost.Bounds.IntersectsWith(pictureBox1.Bounds))
                    ghost1 = -ghost1;
                // if the red ghost hits the picture box 3 we reverse the speed
                else if (redGhost.Bounds.IntersectsWith(pictureBox2.Bounds))
                    ghost1 = -ghost1;
                // if the yellow ghost hits the picture box 1 then wereverse the speed
                if (yellowGhost.Bounds.IntersectsWith(pictureBox3.Bounds))
                    ghost2 = -ghost2;
                // if the yellow chost hits the picture box 2 then wereverse the speed
                else if (yellowGhost.Bounds.IntersectsWith(pictureBox4.Bounds))
                    ghost2 = -ghost2;
                //moving ghosts and bumping with the walls end
                //for loop to check walls, ghosts and points*/
                foreach (Control x in this.Controls)
                {
                    if (x is PictureBox && x.Tag == "dead" && dead == client.NumPlayers())
                    {
                        label2.Text = "GAME OVER";
                        label2.Visible = true;
                        timer1.Stop();
                    }
                    if (x is PictureBox && x.Tag == "pacman")
                    {
                        if (((PictureBox)x).Bounds.IntersectsWith(redGhost.Bounds) ||//ghosts:
                            ((PictureBox)x).Bounds.IntersectsWith(pinkGhost.Bounds) ||
                             ((PictureBox)x).Bounds.IntersectsWith(yellowGhost.Bounds) ||
                              ((PictureBox)x).Bounds.IntersectsWith(pictureBox1.Bounds) || //walls:
                               ((PictureBox)x).Bounds.IntersectsWith(pictureBox2.Bounds) ||
                                ((PictureBox)x).Bounds.IntersectsWith(pictureBox3.Bounds) ||
                                 ((PictureBox)x).Bounds.IntersectsWith(pictureBox4.Bounds))
                        {
                            x.Visible = false;
                            dead++;
                            x.Tag = "dead";
                        }
                        foreach (Control y in this.Controls)
                        {
                            if (y is PictureBox && y.Tag == "coin")
                            {
                                if (((PictureBox)x).Bounds.IntersectsWith(((PictureBox)y).Bounds))
                                {
                                    this.Controls.Remove(y);
                                    score++;
                                    int i = (int)Char.GetNumericValue(x.Name[6]);
                                    scores.Insert(i,scores[i]++);
                                    if (score == total_coins)
                                    {
                                     
                                        int winner = scores.IndexOf(scores.Max());
                                        label2.Text ="PLAYER" + winner.ToString() + " WON!";
                                        label2.Visible = true;
                                        timer1.Stop();
                                    }
                                }
                            }
                        }
                    }
                }
                /*
                pinkGhost.Left += ghost3x;
                pinkGhost.Top += ghost3y;

                if (pinkGhost.Left < boardLeft ||
                    pinkGhost.Left > boardRight ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox1.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox2.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox3.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox4.Bounds)))
                {
                    ghost3x = -ghost3x;
                }
                if (pinkGhost.Top < boardTop || pinkGhost.Top + pinkGhost.Height > boardBottom - 2)
                {
                    ghost3y = -ghost3y;
                }
                */
            }
            
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                tbChat.Text += "\r\n" + client.GetPort() + ":" + tbMsg.Text;
                client.BroadcastChatMsg("\r\n" + client.GetPort() + ":" + tbMsg.Text);
                tbMsg.Clear(); tbMsg.Enabled = false; this.Focus();
                
            }
        }


        public void ChangeChat(string msg)
        {
            tbChat.Text += msg;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tbMsg_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbChat_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
