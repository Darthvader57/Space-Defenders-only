using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Defenders_FINAL
{
    public partial class Form1 : Form
    {
        //Boolean Variables
        bool isMouseHold = false;
        bool isShooting = true;

        int score = 0;
        int highScore = 0;
        int alienSpd = 2;
        int lastDifficultyScore = 0;
        int difficultyLevel = 1;

        //milliseconds btween shooting
        int fireCooldown = 5;

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
            cooldownTimer.Tick += CooldownTimer_tick;

            //Progress bar setup
            cooldownBar.Maximum = 50;
            cooldownBar.Value = 50;

            highScore = Properties.Settings.Default.HighScore;
            //Questionables
            lblScore.Text = "Score: " + score + "High Score:" + highScore;

            gameTimer.Start();
            Player.BringToFront();

        }

        private void Form1_FormedClosed(object sender, FormClosedEventArgs e)
        {
            //Release Mouse
            Cursor.Clip = Rectangle.Empty;
        }

        //Mouse Movement -> Keep Player inside Window/Client Form
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int leftMovement = e.X - (Player.Width / 2);
            if (leftMovement < 0)
                leftMovement = 0;
            if (leftMovement > this.ClientSize.Width - Player.Width)
                leftMovement = this.ClientSize.Width - Player.Width;
            Player.Left = leftMovement;
        }

        //Allows mouse shooting -> hold to rapid-fire
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

        private void CooldownTimer_tick(object sender, EventArgs e)
        {
            if (cooldownBar.Value < 50)
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

            isShooting = false;
            cooldownBar.Value = 0;
            this.Cursor = Cursors.WaitCursor;
            cooldownTimer.Start();

            //Bullet Creation
            PictureBox bullet = new PictureBox();
            bullet.Size = new Size(5, 20);
            bullet.BackColor = Color.YellowGreen;
            bullet.Left = Player.Left + (Player.Width / 2) - (bullet.Width / 2);
            bullet.Top = Player.Top - bullet.Height;
            bullet.Tag = "bullet";
            this.Controls.Add(bullet);
            bullets.Add(bullet);
            bullet.BringToFront();

        }

        private void GameTimer_tick(object sender, EventArgs e)
        {
            //Auto Fire while held
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

            //Movement of Alients and Hitting Aliens
            foreach (PictureBox alien in this.Controls.OfType<PictureBox>())
            {
                if ((string)alien.Tag == "alien")
                {
                    alien.Top += alienSpd;

                    if (alien.Bounds.IntersectsWith(Player.Bounds))
                    {
                        GameOver();
                        return;
                    }

                    foreach (PictureBox pb in bullets.ToList())
                    {
                        if (pb.Bounds.IntersectsWith(alien.Bounds))
                        {
                            //Reset Alients instead removing them
                            alien.Top = -alien.Height;
                            alien.Left = rand.Next(20, this.ClientSize.Width - alien.Width - 20);


                            //Remove bullets
                            this.Controls.Remove(pb);
                            bullets.Remove(pb);

                            score += 10;
                            lblScore.Text = "Score: " + score + "High Score:" + highScore;
                        }
                    }

                    //Respawn
                    if (alien.Top > this.ClientSize.Height)
                    {
                        alien.Top = rand.Next(0, 60);
                        alien.Left = rand.Next(20, this.ClientSize.Width - alien.Width - 20);
                    }
                }
            }

            if (score % 50 == 0 && score != 0 && score != lastDifficultyScore)
            {
                lastDifficultyScore = score;
                alienSpd = 2 + (score / 50);
            }

            //difficulty
            if (score % 50 == 0 && score != 0)
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
            "\n\nPlay Again?",
            "Game Over",
            MessageBoxButtons.YesNo,
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
                if ((string)c.Tag == "bullet")
                    this.Controls.Remove(c);
            }
            bullets.Clear();

            //Reset Variables
            score = 0;
            alienSpd = 2;
            isShooting = true;
            cooldownBar.Value = 50;
            lblScore.Text = "Score: " + score + "High Score:" + highScore;

            //reset our alien Position
            foreach (PictureBox alien in this.Controls.OfType<PictureBox>().ToList())
            {
                if ((string)alien.Tag == "alien")
                {
                    alien.Top = rand.Next(0, 20);
                    alien.Left = rand.Next(20, this.ClientSize.Width - alien.Width - 20);
                }
            }


        }

        private void btnEasy_Click(object sender, EventArgs e)
        {
            this.difficultyLevel = 1;
            this.alienSpd = 3;
            Start_Game();
        }

        private void btnMedium_Click(object sender, EventArgs e)
        {
            this.difficultyLevel = 2;
            this.alienSpd = 4;
            Start_Game();
        }

        private void btnHard_Click(object sender, EventArgs e)
        {
            this.difficultyLevel = 3;
            this.alienSpd = 5;
            Start_Game();
        }

        private void btnExtreme_Click(object sender, EventArgs e)
        {
            this.difficultyLevel = 4;
            this.alienSpd = 6;
            Start_Game();
        }

        void Start_Game()
        {
            menuPanel.Visible = false;

            gameTimer.Start();
            Player.BringToFront();
        }
    }
}
