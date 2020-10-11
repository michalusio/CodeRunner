using Backend;
using ColorMine.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using SystemDll;

namespace FormFront
{
    internal class BoardDrawer
    {
        private const int CELL_SIZE = 32;
        private const int CELL_SIZE_HALF = CELL_SIZE / 2;

        private static readonly Dictionary<Guid, (Rgb, SolidBrush, Pen)> PlayerColorData = new Dictionary<Guid, (Rgb, SolidBrush, Pen)>();

        private static SolidBrush GetPlayerBrush(Guid id)
        {
            if (PlayerColorData.TryGetValue(id, out var playerColorData))
            {
                return playerColorData.Item2;
            }
            else
            {
                var playerColor = new Hsv { H = id.GetHashCode() % 360, S = 1, V = 1 }.To<Rgb>();
                var playerBrush = new SolidBrush(ToColor(playerColor));
                var playerPen = new Pen(playerBrush, 1f / CELL_SIZE);
                PlayerColorData[id] = (playerColor, playerBrush, playerPen);
                return playerBrush;
            }
        }

        public void Paint(Graphics g, Size size)
        {
            g.Clear(Color.AntiqueWhite);
            var matrix = g.Transform;
            PrepareDrawGrid(g, size);

            var ids = PlayerController.PlayerIds;
            foreach (var id in ids)
            {
                DrawPlayer(g, id);
            }
            g.Transform = matrix;
        }

        private static void DrawPlayer(Graphics g, Guid id)
        {
            var playerData = PlayerController.Get(id);
            var playerBrush = GetPlayerBrush(id);
            foreach (var robot in playerData.PlayerData.Robots)
            {
                DrawRobot(g, playerBrush, robot);
            }
        }

        private static void DrawRobot(Graphics g, SolidBrush playerBrush, Robot robot)
        {
            g.FillEllipse(playerBrush, robot.X - 0.5f, robot.Y - 0.5f, 1, 1);
        }

        private static void PrepareDrawGrid(Graphics g, Size size)
        {
            g.TranslateTransform(size.Width / 2 - CELL_SIZE_HALF, size.Height / 2 - CELL_SIZE_HALF);
            for (int i = -32; i <= 32; i++)
            {
                g.DrawLine(Pens.Black, i * CELL_SIZE, -CELL_SIZE * 100, i * CELL_SIZE, CELL_SIZE * 100);
                g.DrawLine(Pens.Black, -CELL_SIZE * 100, i * CELL_SIZE, CELL_SIZE * 100, i * CELL_SIZE);
            }
            //g.FillEllipse(Brushes.Black, 0, 0, CELL_SIZE, CELL_SIZE);
            g.TranslateTransform(CELL_SIZE_HALF, CELL_SIZE_HALF);
            g.ScaleTransform(CELL_SIZE, CELL_SIZE);
        }

        private static Color ToColor(Rgb playerColor)
        {
            return Color.FromArgb(
                (int)playerColor.R,
                (int)playerColor.G,
                (int)playerColor.B
            );
        }
    }
}
