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
        /// <summary>
        /// Describes the state of the connection with the server.
        /// *Note: notLoggedIn and lostConnection are similar, if not the same thing.
        /// </summary>
        public enum state { notLoggedIn, connecting, connected, lostConnection };
        private state _lState = state.notLoggedIn;

        int smallFormHeight = 180;
        int largeFormHeight;

        /// <summary>
        /// Determines the current connection the Login window has. When a new spreadsheet is opened up this connection is passed to the 
        /// spreadsheet GUI and a new connection is made.
        /// </summary>
        private ConnectionLiaison connection;
      
        /// <summary>
        /// Runs when the form is first created. Try putting stuff in Login_Load first before putting it here.
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
            connection = null;
            largeFormHeight = this.Height;
            this.Height = smallFormHeight;
        }


        //------------------------------  Form Helper Functions ------------------------------------

        /// <summary>
        /// Connect to the server at the given hostname and port and with the give name.
        /// </summary>
        private void EstablishConnection(string hostname, int port, Action onConnect, Action<string> onFail)
        {
            //Check if we have a current connection, if we do then disconnect the previous one and make the new connection
            if (connection != null && connection.isConnected())
            {
                //Disconnect
                connection.CloseSocketConnection();
            }

            //Update GUI status
            SetConnectionState(state.connecting);

            connection = new ConnectionLiaison(null, ReceivedSomething);
            connection.tryToConnect(hostname, onConnect, onFail);
        }


        private void SafeGuiChange(Action toInvoke)
        {
            this.Invoke((MethodInvoker)delegate { toInvoke(); });
        }


        //-------------------------------------- Call Backs for other threads. ----------------------------------------
        //------  If you change the GUI directly use "SafeGuiChange(Action a)", otherwise, you may get a "Cross-thread operation not valid" --------------


        public void connected()
        {
            //Send password
            connection.sendPassword(PW_textbox.Text);

            //Get spreadsheets from server
            //TODO


            //ViewState already uses SafeGuiChange, so we don't need to do it here
            SetConnectionState(state.connected);
        }


        public void FailedConnection(string s)
        {
            SetConnectionState(state.lostConnection);

            //MessageBox does not rely on having access to the main Form thread so SafeGuiChange is not needed.
            MessageBox.Show("Error: " + s);
        }

        /// <summary>
        /// Called when a live connection is lost
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="e"></param>
        public void Disconnected(SocketConnection sc, Exception e)
        {
            SetConnectionState(state.lostConnection);
            MessageBox.Show("Lost connection");
        }

        /// <summary>
        /// Method called when the server sends us something
        /// </summary>
        /// <param name="messenger"></param>
        public void ReceivedSomething(MessageReceivedFrom messenger)
        {
            //Split the message via the special delimiter
            string[] split = messenger.message.Split(ConnectionLiaison.ESC);

            //Decide what to do with the message received
            if (split[0] == "FILELIST")
            {
                SafeGuiChange(() =>
                    {
                        ssListBox.Items.Clear();
                        for (int i = 1; i < split.Length; i++)
                            ssListBox.Items.Add(split[i]);
                    });
            }
            else if (split[0] == "INVALID")
            {
                MessageBox.Show("Invalid password");
                SetConnectionState(state.notLoggedIn);
                return;
            }
            else
                MessageBox.Show("Unknown Message from server: \n" + messenger.message);

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

                    ServerButton.Text = "Please wait";
                    StatusLabel.Text = "Attempting to connect to server...";
                }
                else if (s == state.connected)
                {
                    ServerButton.Text = "Refresh";
                    StatusLabel.Text = "Connected";

                    ssListBox.Enabled = true;
                    this.Height = largeFormHeight;

                }
                else if (s == state.lostConnection || s == state.notLoggedIn)
                {
                    //this.Height = smallFormHeight;
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
                    EstablishConnection(IP_textbox.Text, 2500, connected, FailedConnection);
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

            // Grab the selected item
            string item = list.SelectedItem.ToString();

            //Do not show the GUI form! It will come out when it's ready.
            new SpreadsheetGUIForm(connection, item).loadSpreadsheet();
            //Reconnect to the server
            ServerButton_Click(null, null);
        }



    }
}
