using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Space_Defender_Version_1
{
    public partial class Form1 : Form
    {
        int playSpd = 10;
        bool goLeft;
        bool goRight;
        bool shooting;
        int score = 0;
        int shootingSpeed = 20;
        PictureBox bullet = new PictureBox();
        Random rand = new Random();
        
        
        public Form1()
        {
            InitializeComponent();
        }

        private void lblScore_Click(object sender, EventArgs e)
        {

        }
  

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Left)
                goLeft = true;
            if(e.KeyCode == Keys.Right)
                goRight = true;
            if (e.KeyCode == Keys.Space && shooting)
            {
                shooting = true;
                bullet = new PictureBox();
                bullet.Size = new Size(5, 20);
                bullet.BackColor = Color.White;
                bullet.Left = Player.Left + Player.Width / 2 - 2;
                bullet.Top = Player.Top - 20;
                this.Controls.Add(bullet);

            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) 
                goLeft = true;
            if (e.KeyCode == Keys.Right)
                goRight = false;
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {

        }

    }
}
