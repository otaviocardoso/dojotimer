﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;
using System.IO;
using System.Collections;

namespace DojoTimer
{
    public partial class MainForm : Form
    {
        Options options;
        Stopwatch stopwatch = new Stopwatch();
        KeyboardHook hook = new KeyboardHook();

        public MainForm() : this(Options.Load()) { }
        public MainForm(Options options)
        {
            InitializeComponent();
            this.options = options;
            BindHotKey();

            Stop();
            this.Left = Screen.PrimaryScreen.WorkingArea.Right - Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Top + 2 * SystemInformation.CaptionHeight;
            this.Icon = Icons.Green;

        }


        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            RunButton_Click(this, e);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (stopwatch.IsRunning)
                Stop();
            else
                Start();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            SetTransparency();
            if (stopwatch.Elapsed < options.Period)
            {
                SetTime();
            }
            else
            {
                Stop();
                this.Activate();
                using (var alarm = Sounds.alarm)
                    new SoundPlayer(alarm).Play();
                HandleFinish(false);
            }
        }

        private void HandleFinish(bool force)
        {
            if (force || options.KeepTrack)
                new WhoWasHereForm(options).ShowDialog(this);
        }

        private void SetTransparency()
        {
            foreach (var button in this.Controls)
            {
                if (button is ButtonBase)
                {
                    (button as ButtonBase).FlatAppearance.BorderColor = this.BackColor;
                    (button as ButtonBase).FlatAppearance.CheckedBackColor = Color.FromArgb(125, Color.White);
                    (button as ButtonBase).FlatAppearance.MouseOverBackColor = Color.FromArgb(150, Color.White);
                    (button as ButtonBase).FlatAppearance.MouseDownBackColor = Color.FromArgb(200, Color.White);
                }
            }

            var control = TimeLabel;
            control.FlatAppearance.MouseOverBackColor = Color.FromArgb(inside ? 150 : 225, Color.White);
            control.ForeColor = inside || !MainTimer.Enabled ? Color.White : this.BackColor;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Reset();
        }


        private void TimeLabel_Click(object sender, EventArgs e)
        {
            hook.Dispose();
            ShowOptionsForm();
            options.Save();
            BindHotKey();
            SetTime();
        }

        private void ShowOptionsForm()
        {
            var form = new OptionsForm(options);
            bool topmost = this.TopMost;
            this.TopMost = false;
            form.ShowDialog();
            this.TopMost = topmost;
        }

        private void BindHotKey()
        {
            hook = new KeyboardHook();
            hook.KeyPressed += hook_KeyPressed;
            hook.RegisterHotKey(options.ShortcutModifiers, options.ShortcutKey);
        }
        private void RunButton_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void SetTime()
        {
            var remaining = options.Period - stopwatch.Elapsed;
            if (remaining.Ticks < 0) remaining = TimeSpan.Zero;
            TimeLabel.Text = string.Format("{0:00}:{1:00}", (int)remaining.TotalMinutes, remaining.Seconds);
        }

        private void Start()
        {
            if (stopwatch.Elapsed >= options.Period)
                stopwatch.Reset();
            stopwatch.Start();
            MainTimer.Enabled = true;
            StartButton.Text = "<";
        }

        private void Stop()
        {
            stopwatch.Stop();
            MainTimer.Enabled = false;
            StartButton.Text = "4";
            SetTransparency();
            SetTime();
        }

        private void Reset()
        {
            stopwatch.Reset();
            Stop();
        }

        private void Run()
        {
            var list = new ArrayList(Application.OpenForms);
            foreach (Form form in list)
                if (form is OutputWindow && !form.IsDisposed)
                    form.Close();

            var output = new OutputWindow();
            options.Write += s => output.Write(s);
            RunAndShow(output);
        }

        private void RunAndShow(OutputWindow output)
        {
            var run = options.Run();

            output.Show();
            output.Activate();
            this.BackColor = run ? Color.Green : Color.Red;
            this.Icon = run ? Icons.Green : Icons.Red;
            this.SetTransparency();
            output.ShowText(run);
        }

        bool inside = true;
        private void MainForm_Activated(object sender, EventArgs e)
        {
            inside = true;
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            inside = false;
        }

        private void TopMostCheck_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = TopMostCheck.Checked;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void CommitButton_Click(object sender, EventArgs e)
        {
            HandleFinish(true);
        }

    }
}
