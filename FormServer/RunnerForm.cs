﻿using Backend;
using DBLayer;
using FormServer.Controllers;
using HTTPBackend;
using HTTPBackend.Middlewares;
using System;
using System.Windows.Forms;

namespace FormServer
{
    public partial class RunnerForm : Form
    {
        private readonly HttpListener httpListener;
        private readonly BoardDrawer boardDrawer;
        private readonly TreeLogWriter mainConsoleWriter;
        private Guid SelectedPlayer;
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
            HTTPService.RegisterMiddleware<ErrorHandling>();
            HTTPService.RegisterMiddleware<Authentication>();
            HTTPService.RegisterMiddleware<DBMiddleware>();
            var controllers = new BaseController[]
            {
                new AccountController(mainConsoleWriter),
                new CodeController(mainConsoleWriter, richTextBoxConsole)
            };
            httpListener = new HttpListener(controllers, mainConsoleWriter);
        }

        private void RebindList()
        {
            var selectedItem = listBoxUsers.SelectedItem;
            listBoxUsers.DataSource = PlayerController.PlayerIds;
            listBoxUsers.SelectedItem = selectedItem;
        }

        private void ListBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listBoxUsers.SelectedItem as Guid?;
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
                SelectedPlayer = chosenData.PlayerData.PlayerId;
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
            pictureBox1.Invalidate();
        }

        private void TabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            timerBoard.Enabled = (tabControl1.TabIndex == 2);
        }

        private void RunnerForm_Load(object sender, EventArgs e)
        {
            httpListener.StartListener();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            boardDrawer.Paint(e.Graphics, e.ClipRectangle.Size);
        }
    }
}
