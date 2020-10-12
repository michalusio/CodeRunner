using Backend;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FormServer
{
    internal class TreeLogWriter : TextWriter, ILogger
    {
        private readonly TreeView console;
        private TreeNode currentNode;
        private TreeNode parentNode;

        public TreeLogWriter(TreeView view)
        {
            console = view;
            Clear();
        }

        public override Encoding Encoding => Encoding.Default;

        public void Clear()
        {
            Action workFunction = () =>
            {
                console.Nodes.Clear();
                currentNode = new TreeNode("Program");
                parentNode = currentNode;
                parentNode.Expand();
                console.Nodes.Add(currentNode);
            };
            if (console.InvokeRequired)
            {
                console.Invoke(workFunction);
            }
            else workFunction();
        }

        public void InnerLevelWrite(string label, Action actions)
        {
            var tempNode = currentNode;
            currentNode = currentNode.Nodes.Find(label, false).FirstOrDefault();
            if (currentNode == null)
            {
                currentNode = tempNode.Nodes.Add(label, label);
            }
            actions();
            currentNode = tempNode;
        }

        public void Log(object obj) => WriteLine(obj);

        public void OuterLevelWrite(string label, Action actions)
        {
            var tempNode = currentNode;
            currentNode = parentNode.Nodes.Find(label, false).FirstOrDefault();
            if (currentNode == null)
            {
                currentNode = parentNode.Nodes.Add(label, label);
            }
            actions();
            currentNode = tempNode;
        }

        public override void Write(string value)
        {
            string[] lines = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Action workFunction = () =>
            {
                foreach (string str in lines)
                {
                    currentNode.Nodes.Add(str);
                }
            };
            if (console.InvokeRequired)
            {
                console.Invoke(workFunction);
            }
            else workFunction();
        }
        public override void Write(object value) => Write(value?.ToString() ?? "null");
        public override void Write(bool value) => Write((object)value);
        public override void Write(int value) => Write((object)value);
        public override void Write(long value) => Write((object)value);
        public override void WriteLine() => Write(Environment.NewLine);
        public override void WriteLine(string value)
        {
            Write(value);
            WriteLine();
        }
        public override void WriteLine(object value) => WriteLine(value?.ToString() ?? "null");
        public override void WriteLine(bool value) => WriteLine((object)value);
        public override void WriteLine(int value) => WriteLine((object)value);
        public override void WriteLine(long value) => WriteLine((object)value);
    }
}
