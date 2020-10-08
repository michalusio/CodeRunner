﻿using Backend;
using HTTPBackend;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormFront
{
    public partial class RunnerForm : Form
    {
        private readonly HttpListener httpListener;
        private readonly BoardDrawer boardDrawer;
        private readonly TreeLogWriter mainConsoleWriter;
        private ulong SelectedPlayer;
        private int runId;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public RunnerForm()
        {
            InitializeComponent();
            runId = 0;
            RebindList();
            mainConsoleWriter = new TreeLogWriter(treeView1);
            boardDrawer = new BoardDrawer();
            Console.SetOut(mainConsoleWriter);
            httpListener = new HttpListener(new HttpController(mainConsoleWriter, richTextBoxConsole), mainConsoleWriter);
        }

        private void RebindList()
        {
            var selectedItem = listBoxUsers.SelectedItem;
            listBoxUsers.DataSource = PlayerController.PlayerIds;
            listBoxUsers.SelectedItem = selectedItem;
        }

        private void ListBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listBoxUsers.SelectedItem as ulong?;
            if (!item.HasValue) return;
            var chosenData = PlayerController.Get(item.Value);
            if (chosenData == null) return;
            if (PlayerController.Get(SelectedPlayer)?.ConsoleStream is RichTextBoxWriter previousWriter)
            {
                previousWriter.Open = false;
            }
            if (chosenData.ConsoleStream is RichTextBoxWriter currentWriter)
            {
                currentWriter.Open = true;
                SelectedPlayer = chosenData.PlayerData.PlayerIdULong;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (checkBoxAutoRun.Checked)
            {
                mainConsoleWriter.InnerLevelWrite("Runs", () =>
                mainConsoleWriter.InnerLevelWrite($"Run {runId}", () => PlayerController.PerformAllPlayers()));
                runId++;
            }
            RebindList();
        }

        private void TimerBoard_Tick(object sender, EventArgs e)
        {
            panel3.Invalidate();
        }

        private void TabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            timerBoard.Enabled = (tabControl1.TabIndex == 2);
        }

        private void Panel3_Paint(object sender, PaintEventArgs e)
        {
            boardDrawer.Paint(e.Graphics, e.ClipRectangle.Size);
        }

        private void RunnerForm_Load(object sender, EventArgs e)
        {
            httpListener.StartListener();
        }
    }
}
