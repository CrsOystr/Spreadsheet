using CustomNetworking;
using SS;
using SpreadsheetGUI;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerConnGUI
{
    public partial class Login : Form
    {

        public enum state { notLoggedIn, connecting, connected, lostConnection };
        private state _lState = state.notLoggedIn;

        int smallFormHeight = 180;
        int largeFormHeight;

        /*variables to be used
        private bool authenticated = false;
        private bool connected = false;
        private string PW = "";
        private string IP = "";
        private StringSocket socket;
        //*/

        private ConnectionLiaison heldConnection;
      
        /// <summary>
        /// Runs when the form is first created.
        /// </summary>
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Runs when the form is loaded (before it shows).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Load(object sender, EventArgs e)
        {
            SetConnectionState(state.notLoggedIn);
            heldConnection = null;
            largeFormHeight = this.Height;
            this.Height = smallFormHeight;
        }


        //------------------------------  Form Helper Functions ------------------------------------

        /// <summary>
        /// Connect to the server at the given hostname and port and with the give name.
        /// </summary>
        private void EstablishConnection(string hostname, int port, String password, Action<string> onConnect, Action<string> onFail)
        {
            if (heldConnection != null && heldConnection.Connected)
            {

            }


            //Update GUI status
            SetConnectionState(state.connecting);


            ConnectionLiaison cl = new ConnectionLiaison(hostname, null);
            cl.tryToConnect(onConnect, onFail);


            /*
            TcpClient client = null;
            if (socket == null)
            {
                try
                {
                    client = new TcpClient(hostname, port);
                    connected = true;
                }
                catch
                {
                    connected = false;
                }
                if (connected)
                {
                    socket = new StringSocket(client.Client, UTF8Encoding.Default);
                    socket.BeginReceive(LineReceived, null);
                    socket.BeginSend("PASSWORD\u001Bjames\n", (e, p) => { }, null);
                }
            }
            return connected;
            //*/
        }


        private void SafeGuiChange(Action toInvoke)
        {
            this.Invoke((MethodInvoker)delegate { toInvoke(); });
        }

        /*
        private void LineReceived(string s, Exception e, object payload)
        {
            MessageBox.Show(s);
        }
        //*/



        //-------------------------------------- Call Backs for other threads. ----------------------------------------
        //------  If you change the GUI directly use "SafeGuiChange(Action a)", otherwise, you may get a "Cross-thread operation not valid" --------------


        public void LoggedIn(string s)
        {
            //ViewState already uses SafeGuiChange, so we don't need to do it here
            SetConnectionState(state.connected);

            //Get spreadsheets from server
            //TODO
        }

        public void FailedConnection(string s)
        {
            SetConnectionState(state.lostConnection);

            //MessageBox does not rely on having access to the main Form thread so SafeGuiChange is not needed.
            MessageBox.Show("Error: " + s);
        }






        private void SetConnectionState(state s)
        {
            //Safely Change the GUI as other threads may have set this property
            SafeGuiChange(() =>
            {
                if (s == state.connecting)
                {
                    ssListBox.Enabled = false;
                    ServerButton.Enabled = true;

                    StatusLabel.Text = "Attempting to connect to server...";
                    ServerButton.Text = "Press to abort";
                }
                else if (s == state.connected)
                {
                    StatusLabel.Text = "Connected";
                    ServerButton.Text = "Refresh";

                    ssListBox.Enabled = true;
                    this.Height = largeFormHeight;

                }
                else if (s == state.lostConnection || s == state.notLoggedIn)
                {
                    StatusLabel.Text = "Not connected";
                    ServerButton.Enabled = true;
                    ServerButton.Text = "Connect";
                }

            });
        }

        private state GetConnectionState()
        {
            return _lState;
        }


        // --------------------------------------  Form Events ----------------------------------------------------

        private void ServerButton_Click(object sender, EventArgs e)
        {
            if (ServerButton.Enabled == true)
            {


                //Verify the validity of the server info

                //if the IP isn't entered error box
                if (IP_textbox.Text.Length < 1)
                {
                    MessageBox.Show("Please enter a valid server IP address");
                }
                //if PW text box has no value error box
                else if (PW_textbox.Text == "")
                {
                    MessageBox.Show("Please enter a password to connect to the server");
                }
                //if both tb.text field have values, save them as variables
                else
                {
                    //Try to connect and login to the server. This method shouldn't hold on to the GUI thread for very long.
                    EstablishConnection(IP_textbox.Text, 2500, PW_textbox.Text, LoggedIn, FailedConnection);
                }
            }
        }

        private void waitForEnter(object sender, KeyEventArgs e)
        {
            //only activate if the enter key was pressed
            if (e.KeyCode == Keys.Enter)
            {
                //enter key is down, Do what we do when we click the ServerButton
                ServerButton_Click(sender, e);

                //Suppress the annoying ding
                e.Handled = true;
                e.SuppressKeyPress = true;

            }
        }

        private void ssListBox_DoubleClick(object sender, EventArgs e)
        {
            var list = (ListBox)sender;

            // This is your selected item
            string item = list.SelectedItem.ToString();

            new SpreadsheetGUIForm(heldConnection, item).Show();
        }



    }
}
