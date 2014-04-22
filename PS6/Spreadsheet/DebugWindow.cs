using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS
{

    /// <summary>
    /// Represents the type of debug message which will determine the color it displays in the debug window.
    /// </summary>
    public enum type { send, receive, error, other };

    /// <summary>
    /// A window for displaying log messages
    /// </summary>
    public partial class DebugWindow : Form
    {

        Color[] typecolors = {Color.Green,Color.Teal ,Color.Red , Color.White};

        bool ScrollLock = true;

        int oldW;
        int oldH;

        /// <summary>
        /// 
        /// </summary>
        public DebugWindow()
        {
            InitializeComponent();
            oldW = this.Width;
            oldH = this.Height;
        }

        /// <summary>
        /// Writes to the debug window
        /// </summary>
        /// <param name="messageType">The message type determines the highligh color the message will be logged with</param>
        /// <param name="message">The message to log in the debug window.</param>
        public void write(type messageType,string message)
        {
            string timeStamp = DateTime.Now.ToString("h:mm:ss tt ");
            message = message.Replace("\u001b", "[ESC]");

            SafeGuiChange(() =>
                {
                    Color c = (((int)messageType) < typecolors.Length ? typecolors[((int)messageType)] : typecolors[typecolors.Length - 1]);
                    richTextBox1.SelectionBackColor = Color.White;
                    richTextBox1.AppendText(timeStamp);
                    richTextBox1.SelectionBackColor = c;
                    richTextBox1.AppendText(ToLiteral(message)+"\n");

                });
        }


        private void buttonClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }


        private void SafeGuiChange(Action toInvoke)
        {
            if (this.IsHandleCreated)
                this.Invoke((MethodInvoker)delegate { toInvoke(); });
        }


        private void DebugWindow_ClientSizeChanged(object sender, EventArgs e)
        {
            int deltaW = this.Width - oldW;
            int deltaH = this.Height - oldH;
            oldW = this.Width;
            oldH = this.Height;
            richTextBox1.Width += deltaW;
            richTextBox1.Height += deltaH;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ScrollLock)
                buttonScrollLock.Text = "Scroll-Lock";
            else
                buttonScrollLock.Text = "Stop Scroll-Lock";
            ScrollLock = !ScrollLock;
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

            if (ScrollLock)
            {
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }


        /// <summary>
        /// Converts any escaped characters in the input into a seeable format like "\n"
        /// Borrowed from http://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        private void DebugWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

    /// <summary>
    /// Used to send log messages to the debug window.
    /// </summary>
    public static class debug
    {
        static DebugWindow d = new DebugWindow();

        /// <summary>
        /// Shows the debug window
        /// </summary>
        public static void show()
        {
            d.Show();
        }

        /// <summary>
        /// Hides the debug window
        /// </summary>
        public static void hide()
        {
            d.Hide();
        }

        public static void toggleDebug()
        {
            if (d.Visible)
                hide();
            else
                show();
        }

        /// <summary>
        /// Writes to the debug window
        /// </summary>
        /// <param name="messageType">The message type determines the highligh color the message will be logged with</param>
        /// <param name="message">The message to log in the debug window.</param>
        public static void write(type messageType, string message)
        {
            d.write(messageType, message);
        }
    }
}
