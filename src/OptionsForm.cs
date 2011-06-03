﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.IO;
using DojoTimer.Helpers;

namespace DojoTimer
{
    public partial class OptionsForm : Form
    {
        Options options;
        public Options Options { get { return options; } }
        public OptionsForm(Options options)
        {
            InitializeComponent();
            SetOptions(options);
            formTitleBar1.BindHandleTo(this);
        }

        private void SetOptions(Options options)
        {
            this.options = options;
            MinutesInput.Text = ((int)options.Period.TotalMinutes).ToString("00");
            SecondsInput.Text = options.Period.Seconds.ToString("00");
            ShortcutInput.Text = (ShortcutInput.Tag = options.Shortcut).ToString();
            ScriptInput.Text = options.Script;
            WorkingDirectoryInput.Text = options.WorkingDirectory;

            ParticipantsInput.Lines = options.Participants;
            KeepTrackInput.Checked = options.KeepTrack;
            CommitScript.Text = options.CommitScript;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShortcutInput_KeyDown(object sender, KeyEventArgs e)
        {
            ShortcutInput.Text = (ShortcutInput.Tag = e.KeyData).ToString();

            e.SuppressKeyPress = true;
        }

        private void ShortcutInput_Leave(object sender, EventArgs e)
        {
            if ((((Keys)ShortcutInput.Tag) & Keys.Modifiers) == 0)
                ShortcutInput.Text = (ShortcutInput.Tag = options.Shortcut).ToString();
        }

        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            options.Period = TimeSpan.FromSeconds(int.Parse(MinutesInput.Text) * 60 + int.Parse(SecondsInput.Text));
            options.Shortcut = (Keys)ShortcutInput.Tag;
            options.Script = ScriptInput.Text;

            options.Participants = ParticipantsInput.Lines;
            options.KeepTrack = KeepTrackInput.Checked;
            options.CommitScript = CommitScript.Text;
            options.WorkingDirectory = WorkingDirectoryInput.Text;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                SetOptions(new Options());
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (BrowseFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WorkingDirectoryInput.Text = BrowseFolder.SelectedPath;
            }
        }

    }
}
