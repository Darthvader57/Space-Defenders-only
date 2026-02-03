using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Defenders_Version_2
{
    public partial class Form1 : Form
    {
        
        bool shooting;
        int score = 0;
        int shootingSpeed = 20;
        PictureBox bullet = new PictureBox();
        Random rand = new Random();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            gameTimer.Start();
            Player.BringToFront();
            this.MouseMove += Form1_MouseMove;
            this.MouseMove += Form1_MouseClick;


            // Optional: Display starting score
            lblScore.Text = "Score: 0";
        }
        //moves player from left to right and right to left
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
          Player.Left =e.X - (Player.Width) / 2;
        }

        //shoot the bullet using the left mouse click
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && !shooting)
            {
                shooting = true;
                bullet = new PictureBox();
                bullet.Size = new Size(5, 20);
                bullet.BackColor = Color.Red;
                bullet.Left = Player.Left + Player.Width / 2 - 2;
                bullet.Top = Player.Top - 20;
                this.Controls.Add(bullet);
                bullet.BringToFront();
            }
        }
        private void gameTimer_Tick(object sender, EventArgs e)
        {

            // Move bullet
            if (shooting && bullet != null)
            {
                bullet.Top -= shootingSpeed;
                if (bullet.Top < 0)
                {
                    shooting = false;
                    this.Controls.Remove(bullet);
                }
            }

            // Check collision with aliens
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "Alien")
                {
                    x.Top += 1;
                    //Bullet hits Alien
                    if (shooting && bullet != null && bullet.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        this.Controls.Remove(bullet);
                        shooting = false;
                        score += 10;
                        lblScore.Text = "Score: " + score;
                    }
                    //Alien hits player
                    if (x.Bounds.IntersectsWith(Player.Bounds))
                    {
                        gameTimer.Stop();
                        lblScore.Text = "Game Over!";
                    }


                }
            }
        }
    }
}
