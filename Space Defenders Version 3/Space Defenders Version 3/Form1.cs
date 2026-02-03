using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Defenders_Version_3
{
    public partial class Form1 : Form
    {
        bool isMouseHold = false;
        bool isShooting = true;

        int score = 0;
        int highScore = 0;
        int alienSpd = 2;
        int difficultyLevel = 1;
        int shootingSpeed = 20;

        //Milliseconds between shooting
        int fireCooldown = 150;

        Random rand = new Random();
        Timer cooldownTimer = new Timer();

        List<PictureBox> bullets = new List<PictureBox>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            //Create Cursor for CoolDown
            Cursor.Clip = this.RectangleToScreen(this.ClientRectangle);
            cooldownTimer.Interval = fireCooldown;
            cooldownTimer.Tick += CooldownTimer_Tick;

            //setup Progress Bar
            cooldownBar.Maximum = 100;
            cooldownBar.Value = 100;

            highScore = Properties.Settings.Default.HighScore;
            lblScore.Text = "Score: " + score + "High Score: " + highScore;
           gameTimer.Start();
            Player.BringToFront();


        }

        private void Form1_Closed(object sender, FormClosedEventArgs e)
        {
            //Release Mouse
            Cursor.Clip = Rectangle.Empty;

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int leftMovement = e.X - (Player.Width / 2);
            if (leftMovement < 0)
                leftMovement = 0;
            if (leftMovement > this.ClientSize.Width - Player.Width)
                leftMovement = this.ClientSize.Width - Player.Width;
            Player.Left = leftMovement;

        }
        // allow mouse shooting to Rapid Fire
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isMouseHold = true;
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                isMouseHold = false;
        }

        private void CooldownTimer_Tick(object sender, EventArgs e)
        {
            if(cooldownBar.Value < 100)
            {
                cooldownBar.Value += 10;
            }
            else
            {
                cooldownTimer.Stop();
                isShooting = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void ShootBullet()
        {
            if (!isShooting)
                return;
            isShooting=false;
            cooldownBar.Value = 0;
            this.Cursor = Cursors.WaitCursor;
            cooldownTimer.Start();

            //Bullet Creation
            PictureBox bullet = new PictureBox();
            bullet.Size = new Size(5, 20);
            bullet.BackColor = Color.Red;
            bullet.Left = Player.Left + (Player.Width / 2) - (bullet.Width / 2);
            bullet.Top = Player.Top - bullet.Height;
            bullet.Tag = "bullet";
            this.Controls.Add(bullet);
            bullets.Add(bullet);
            bullet.BringToFront();
            //Continue to add bullets
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {     //auto fire
            if (isMouseHold && isShooting)
                ShootBullet();

            //Move Bullets
            foreach (PictureBox pb in bullets.ToList())
            {
                pb.Top -= 20;

                if (pb.Top < 0)
                {
                    bullets.Remove(pb);
                    this.Controls.Remove(pb);
                }

            }
            //movement for aliens and hitting them
            foreach (PictureBox Alien in this.Controls.OfType<PictureBox>())
            {
                if ((string)Alien.Tag == "Alien")
                {
                    Alien.Top += alienSpd;

                    if (Alien.Bounds.IntersectsWith(this.Bounds))
                    {
                        GameOver();
                        return;
                    }

                    foreach (PictureBox pb in bullets.ToList())
                    {
                        if (pb.Bounds.IntersectsWith(Alien.Bounds))
                        {   //resets aliens instead of removing them
                            Alien.Top = rand.Next(0, 60);
                            Alien.Left = rand.Next(20, this.ClientSize.Width - Alien.Width - 20);

                            //Remove Bullet
                            this.Controls.Remove(pb);
                            bullets.Remove(pb);

                            score += 10;
                            lblScore.Text = "Score: " + score + "High Score: " + highScore;
                        }
                    }
                }

                //Respawn
                if (Alien.Top > this.ClientSize.Height)
                {
                    Alien.Top = rand.Next(0, 60);
                    Alien.Left = rand.Next(20, this.ClientSize.Width - Alien.Width - 20);
                }
            }

            //Difficulty
            if(score % 50 == 0 && score != 0)
                alienSpd = 2 + (score / 50);
        }




        private void GameOver()
        {
            gameTimer.Stop();
            cooldownTimer.Stop();

            if (score > highScore)
            {
                highScore = score;
                Properties.Settings.Default.HighScore = highScore;
                Properties.Settings.Default.Save();
            }




            DialogResult results = MessageBox.Show(
            "Game Over!\n\nScore: " + score +
            "\nHigh Score: " + highScore +
             "\n\nPlay Again?" +
             MessageBoxButtons.YesNo +
             MessageBoxIcon.Information);

            if (results == DialogResult.Yes)
                GameRestart();
            else
            this.Close();

        }

        private void GameRestart()
        {
            foreach (Control c in this.Controls.OfType<PictureBox>().ToList())
            {
                if((string)c.Tag == "bullet")
                    this.Controls.Remove(c);
            }
            bullets.Clear();

            //Reset Variables
            score = 0;
            alienSpd = 2;
            isShooting = true;
            cooldownBar.Value = 100;
            lblScore.Text = "Score: " + score + "High Score: " + highScore;

            //reset alien positions
            foreach (PictureBox alien in this.Controls.OfType<PictureBox>().ToList())
            {
                if((string)alien.Tag == "alien")
                {
                    alien.Top = rand.Next(0, 60);
                    alien.Left = rand.Next(20, this.ClientSize.Width - alien.Width - 20);

                }
            }
        }
    }
}


