using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DonkeyKong
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }

        // speed
        int speed = 3;

        // create the sequence
        int sequence = 0;
        int sr = 1;
        int walkcycle = 1;
        // 7 mod 3 = 1 
        // 10 mod 3 = 1
        // 11 mod 3 = 2
        // 13 mod 3 = 1

        // player movement
        bool r = false;
        bool u = false;
        bool l = false;
        bool isJumping = false;

        // jumping stuff
        int jumpHeight = 15;
        int jumpSpeed = 2;
        int gravity = 3;
        int floorLevel;
        int jumpUp = 0;
        int jumpDown = 26;
        bool fall = false;

        // donkey kong barrel movement
        bool left = false;
        bool right = true;
        int barrelSpeed = 3;
        List<PictureBox> barrels = new List<PictureBox>();

        // make list for ladders
        List<PictureBox> ladders = new List<PictureBox>();

        // create random event for dk's barrel
        Random gen = new Random();
        #region Mouse Move
        // allow form to be dragged with mouse
        private Point mouseDownLocation;
        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownLocation = e.Location;
                Cursor = Cursors.SizeAll;
            }
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
    {
                Point newLocation = PointToClient(Cursor.Position);
                Point offset = new Point(newLocation.X - mouseDownLocation.X, newLocation.Y - mouseDownLocation.Y);
                Location = new Point(Location.X + offset.X, Location.Y + offset.Y);
                Invalidate();
            }
        }

        private void Form2_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }
        #endregion

        private void tmrMovement_Tick(object sender, EventArgs e)
        {
            
            #region Player Movement
            pbPlayer.BringToFront();
            sequence += 1;
            sr = sequence % 10;
            if (l)
            {
                pbPlayer.Left -= speed;
                if (sr == 0)
                {
                    walkcycle++;
                    pbPlayer.Image = (Image)Properties.Resources.ResourceManager.GetObject("fixrl" + walkcycle);
                    pbPlayer.Refresh();
                    if (walkcycle == 3) { walkcycle = 0; }
                }

            }



            if (r)
            {
                pbPlayer.Left += speed;

                if (sr == 0)
                {
                    walkcycle++;
                    pbPlayer.Image = (Image)Properties.Resources.ResourceManager.GetObject("fixrr" + walkcycle);
                    pbPlayer.Refresh();
                    if (walkcycle == 3) { walkcycle = 0; }
                }

            }

           
                for (int i = 0; i < ladders.Count; i++)
                {
                if (u && pbPlayer.Bounds.IntersectsWith(ladders[i].Bounds))
                {
                    pbPlayer.Top -= speed;
                    if (sr == 0)
                    {
                        walkcycle++;
                        pbPlayer.Image = (Image)Properties.Resources.ResourceManager.GetObject("fixu" + walkcycle);
                        pbPlayer.Refresh();
                        if (walkcycle == 2) { walkcycle = 0; }

                    }
                }

                }

            #endregion

            

            #region Jumping
            if (isJumping == true)
            {
                Jump();
            }
            else
            {
                jumpUp = 0;
                jumpDown = 26;
            }
            #endregion

            #region Donkey Kong
            for (int j = 0; j < barrels.Count; j++)
            {

                if (barrels[j].Tag.ToString()=="right")
                {
                    barrels[j].Left += barrelSpeed;

                }
                if (barrels[j].Tag.ToString() == "left")
                {
                    barrels[j].Left -= barrelSpeed;

                }
                #region Wall Collision
                if (barrels[j].Bounds.IntersectsWith(wall5.Bounds))
                {
                    barrels[j].Tag = "down";
                    barrels[j].Top += 5;
                  
                }
                if (barrels[j].Bounds.IntersectsWith(wall4.Bounds))
                {
                    barrels[j].Tag = "down";
                    barrels[j].Top += 5;

                }
                if (barrels[j].Bounds.IntersectsWith(wall3.Bounds))
                {
                    barrels[j].Tag = "down";
                    barrels[j].Top += 5;

                }
                if (barrels[j].Bounds.IntersectsWith(wall2.Bounds))
                {
                    barrels[j].Tag = "down";
                    barrels[j].Top += 5;

                }
                if (barrels[j].Bounds.IntersectsWith(wall1.Bounds))
                {
                    barrels[j].Tag = "down";
                    barrels[j].Top += 5;

                }
                if (barrels[j].Bounds.IntersectsWith(wall0.Bounds))
                {
                    Controls.Remove(barrels[j]);
                   barrels[j].Dispose();
                    barrels.RemoveAt(j);
                    

                }
                #endregion
                #region Floor Collision
                if (barrels[j].Bounds.IntersectsWith(floor5.Bounds))
                {
                    barrels[j].Tag = "left";

                }
                if (barrels[j].Bounds.IntersectsWith(floor4.Bounds))
                {
                    barrels[j].Tag = "right";

                }
                if (barrels[j].Bounds.IntersectsWith(floor3.Bounds))
                {
                    barrels[j].Tag = "left";

                }
                if (barrels[j].Bounds.IntersectsWith(floor2.Bounds))
                {
                    barrels[j].Tag = "right";

                }
                if (barrels[j].Bounds.IntersectsWith(floor1.Bounds))
                {
                    barrels[j].Tag = "left";

                }
                #endregion
                if (pbPlayer.Bounds.IntersectsWith(barrels[j].Bounds)) {
                    tmrBarrel.Stop();
                    tmrMovement.Stop();
                    DialogResult dialogResult = MessageBox.Show("Would you like to play again?", "You lose",  MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Application.Restart();
                        //this.Close();
                        //this.Refresh();
                        this.InitializeComponent();

                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        this.Close();
                    }
                }
                if (pbPlayer.Bounds.IntersectsWith(peach.Bounds))
                {
                    tmrBarrel.Stop();
                    tmrMovement.Stop();
                    DialogResult dialogResult2 = MessageBox.Show("YOU WIN");
                    if (dialogResult2 == DialogResult.OK)
                    {
                        this.Close();

                    }
                    
                }
            }
            #endregion
        }
        private void CreateBarrel()
        {
            PictureBox barrel = new PictureBox();
        
            barrel.BringToFront();
            barrel.Size = new Size(40, 40);
            barrel.Location = new Point(197, 207);
            barrel.Image = Properties.Resources.b1;
            barrel.SizeMode = PictureBoxSizeMode.Zoom;
            barrel.BackColor = Color.Transparent;
            barrel.Tag = "right";
            Controls.Add(barrel);
            barrels.Add(barrel);
        }
        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D) { r = true; }
            if (e.KeyCode == Keys.A) { l = true; }
            if (e.KeyCode == Keys.W) { u = true; }
            if (e.KeyCode == Keys.Space && isJumping == false) { isJumping = true; }
        }
        private void Jump()
        {
            if(jumpUp<=26)
            {
                jumpUp++;
                pbPlayer.Top-=2;
            }else if(jumpUp >=2) 
            {
                jumpDown--;
                pbPlayer.Top+=2;
                if (jumpDown == 0)
                {
                    isJumping = false;
                }
            }





           // pbPlayer.Top -= jumpSpeed;
          //  jumpHeight--;

           // if (jumpHeight < 0)
          //  {
           //     isJumping = false;
            //    jumpHeight = 10;
          //  }
        }
        private void Fall()
        {
            if (fall == true) { 
                if (pbPlayer.Bottom + gravity < floorLevel)
                {
                    pbPlayer.Top += gravity;
                }
                else
                {
                    pbPlayer.Top = floorLevel - pbPlayer.Height;
                 
                }
            }
        
        }
        private void Form2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                r = false;
                pbPlayer.Image = Properties.Resources.fixrr1;
            }
            if (e.KeyCode == Keys.A)
            {
                l = false;
                pbPlayer.Image = Properties.Resources.fixrl1;
            }
            if (e.KeyCode == Keys.W)
            {
                u = false;
                pbPlayer.Image = Properties.Resources.fixu1;
            }
           
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            pbPlayer.BackColor = Color.Transparent;
            for (int i = 1; i <= 6; i++)
            {
                ladders.Add((PictureBox)this.Controls["ladder" + i.ToString()]);
            }
            pbPlayer.BringToFront();
        }

        private void tmrBarrel_Tick(object sender, EventArgs e)
        {
            
               
            
            
        }

        private void tmrBarrel_Tick_1(object sender, EventArgs e)
        {
            int r = gen.Next(50);
            if (r == 20)
            {
                CreateBarrel();
            }
            
            }
            
    }
}
