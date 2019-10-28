using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace ShutDown
{
    public partial class AFKForm : Form
    {
        public AFKForm()
        {
            InitializeComponent();
        }

        private static Image ControlToBitmap(Control cntrl)
        {
            var bmp = new Bitmap(cntrl.Width, cntrl.Height);
            cntrl.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
            return bmp;
        }


        private static void ControlBrigthness(Control cntrl, Control to, bool Light = true, float brightness = (float)0.1)
        {
            var image = ControlToBitmap(cntrl);

            var imageAttributes = new ImageAttributes();
            var width = image.Width;
            var height = image.Height;
            var rnd1 = new Random();
            switch (Light)
            {
                case true: { var rnd = rnd1.Next(4, 9); brightness = brightness * rnd; break; }
                case false: { var rnd = rnd1.Next(1, 3); brightness = -brightness * rnd; break; }
            }
            float[][] colorMatrixElements = {
                                                new float[] {1, 0, 0, 0, 0},
                                                new float[] {0, 1, 0, 0, 0},
                                                new float[] {0, 0, 1, 0, 0},
                                                new float[] {0, 0, 0, 1, 0},
                                                new[] {brightness, brightness, brightness, 0, 1}
                                            };

            var colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            var graphics = Graphics.FromImage(image);
            graphics.DrawImage(image, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, imageAttributes);

            to.BackgroundImage = image;
        }

        private void AFKForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized; 
        }

        private void AFKForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        Color[] backColors = { Color.Black, Color.Lime, Color.Red, Color.Orange, Color.White, Color.Yellow, Color.Blue, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black };
        Color[] foreColors = { Color.Yellow, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Black, Color.Lime, Color.Red, Color.Orange, Color.White, Color.Yellow, Color.Blue};
        private void ChangeTime()
        {
            t = TimeSpan.FromSeconds(seconds);
            string answer;
            if (t.Hours > 0) answer = string.Format("{0:D2}h:{01:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
            else if (t.Minutes > 0) 
            {
                answer = $"{t.Minutes:D2}m:{t.Seconds:D2}s";
            }
            else answer = $"{t.Seconds:D2}s";
            Invoke(new Action(() =>
            {
                label1.Text = answer;
                if (t.Hours == 0 && t.Minutes < 5)
                {
                    this.Show();
                    this.TopMost = false;
                    this.TopMost = true;
                    this.WindowState = FormWindowState.Maximized;
                    button2.Visible = true;
                    button2.BackColor = backColors[seconds % backColors.Length];
                    button2.ForeColor = foreColors[seconds % foreColors.Length];
                }
            }));
        }


        TimeSpan t;
        uint seconds = 60*60;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    seconds = 60 * 60; break;
                case 1:
                    seconds = 2 * 60 * 60; break;
                case 2:
                    seconds = 30 * 60; break;
                default:
                    seconds = 10*60;
                    break;
            }
            ChangeTime();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
            switch (checkBox1.Checked)
            {
                case true:
                    checkBox1.Text = "Остановить таймер";
                    break;
                case false:
                    checkBox1.Text = "Включить таймер";
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            seconds--;
            if(seconds > 60*60)
                WinAPI.TaskbarProgress.SetValue(this.Handle, 2 * 60 * 60 - seconds, 4 * 60 * 60);
            else if (seconds > 30 * 60)
                WinAPI.TaskbarProgress.SetValue(this.Handle, 60 * 60 - seconds, 2*60 * 60);
            else
                WinAPI.TaskbarProgress.SetValue(this.Handle, 30 * 60 - seconds, 30 * 60);
            ChangeTime();
            if(seconds == 0)
            {
                ProcessStartInfo Info = new ProcessStartInfo("shutdown");
                Info.Arguments = "-s -f -t 1";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Process.Start(Info);
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            seconds = 30 * 60;
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.Red;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.White;
        }

        private void checkBox1_MouseEnter(object sender, EventArgs e)
        {
            checkBox1.ForeColor = Color.Lime;
            checkBox1.Focus();
        }

        private void checkBox1_MouseLeave(object sender, EventArgs e)
        {
            checkBox1.ForeColor = Color.White;
            panel1.Focus();
        }

        private void checkBox1_Paint(object sender, PaintEventArgs e)
        {
            if (checkBox1.Focused)
            {
                var pen = new Pen(Color.Lime, 2);
                uint p = 5;
                e.Graphics.DrawRectangle(pen, checkBox1.ClientRectangle.X, checkBox1.ClientRectangle.Y, checkBox1.ClientRectangle.Width - p, checkBox1.ClientRectangle.Height - p);
            }
        }
    }
}
