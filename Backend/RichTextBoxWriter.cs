using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Backend
{
    public class RichTextBoxWriter : TextWriter, IClearableTextWriter
    {
        private readonly StringBuilder stream;
        private readonly RichTextBox richTextBox;

        private bool open = false;
        public bool Open
        {
            get => open;
            set
            {
                open = value;
                if (open)
                {
                    Action workFunction = () =>
                    {
                        richTextBox.Text = stream.ToString();
                    };
                    if (richTextBox.InvokeRequired)
                    {
                        richTextBox.Invoke(workFunction);
                    }
                    else workFunction();
                }
            }
        }

        public RichTextBoxWriter(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
            stream = new StringBuilder(1000);
        }

        public override Encoding Encoding => Encoding.Default;

        public void Clear()
        {
            stream.Clear();
            if (Open)
            {
                Action workFunction = () =>
                {
                    richTextBox.ResetText();
                };
                if (richTextBox.InvokeRequired)
                {
                    richTextBox.Invoke(workFunction);
                }
                else workFunction();
            }
        }

        public override void Write(string value)
        {
            stream.Append(value);
            if (Open)
            {
                Action workFunction = () =>
                {
                    richTextBox.AppendText(value);
                };
                if (richTextBox.InvokeRequired)
                {
                    richTextBox.Invoke(workFunction);
                }
                else workFunction();
            }
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