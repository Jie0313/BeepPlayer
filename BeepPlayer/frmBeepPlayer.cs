using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace BeepPlayer
{
    public partial class frmBeepPlayer : Form
    {
        [DllImport("kernel32.dll")]
        public static extern bool Beep(int frequency, int duration);

        private readonly int[]    baseFreq = { 523, 587, 659, 698, 784, 880, 988, 1046 };
        private readonly string[] notes    = { "Do", "Re", "Mi", "Fa", "Sol", "La", "Si", "Do'" };
        private int[] freq;
        private int   octave = 0;

        private int initWidth  = 0;
        private int initHeight = 0;
        private Dictionary<string, Rect> initControl = new Dictionary<string, Rect>();

        public frmBeepPlayer()
        {
            InitializeComponent();
            freq = (int[])baseFreq.Clone();
            InitializeButton();
        }

        private void InitializeButton()
        {
            btn2.Click += btn1_Click;
            btn3.Click += btn1_Click;
            btn4.Click += btn1_Click;
            btn5.Click += btn1_Click;
            btn6.Click += btn1_Click;
            btn7.Click += btn1_Click;
            btn8.Click += btn1_Click;
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.Enabled = false;
            Beep(freq[btn.TabIndex], 300);
            btn.Enabled = true;
        }

        private void btnOctaveUp_Click(object sender, EventArgs e)
        {
            if (octave >= 2) return;
            octave++;
            UpdateFreq();
        }

        private void btnOctaveDown_Click(object sender, EventArgs e)
        {
            if (octave <= -2) return;
            octave--;
            UpdateFreq();
        }

        private void UpdateFreq()
        {
            double mult = Math.Pow(2, octave);
            Button[] btns = { btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8 };
            for (int i = 0; i < 8; i++)
            {
                freq[i]      = (int)(baseFreq[i] * mult);
                btns[i].Text = $"{notes[i]}\n{freq[i]}Hz";
            }
            string mark = octave > 0 ? new string('↑', octave)
                        : octave < 0 ? new string('↓', Math.Abs(octave)) : "";
            this.Text = octave == 0 ? "簡易電子琴" : $"簡易電子琴  {mark}";
        }

        private void frmBeepPlayer_Load(object sender, EventArgs e)
        {
            this.initWidth  = this.palMain.Width;
            this.initHeight = this.palMain.Height;

            foreach (Control ctl in this.palMain.Controls)
                this.initControl.Add(ctl.Name, new Rect(ctl.Left, ctl.Top, ctl.Width, ctl.Height));
        }

        private void frmBeepPlayer_SizeChanged(object sender, EventArgs e)
        {
            if (this.initWidth == 0 || this.initHeight == 0 || this.initControl.Count == 0)
                return;

            double iRatioWith   = this.palMain.Width  / (double)this.initWidth;
            double iRatioHeight = this.palMain.Height / (double)this.initHeight;

            foreach (Control ctl in this.palMain.Controls)
            {
                if (!initControl.ContainsKey(ctl.Name)) continue;
                ctl.Left   = (int)(initControl[ctl.Name].Left   * iRatioWith);
                ctl.Top    = (int)(initControl[ctl.Name].Top    * iRatioHeight);
                ctl.Width  = (int)(initControl[ctl.Name].Width  * iRatioWith);
                ctl.Height = (int)(initControl[ctl.Name].Height * iRatioHeight);
            }
        }

        private void frmBeepPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("確定要關閉應用程式嗎？", "關閉確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                e.Cancel = true;
        }
    }
}
