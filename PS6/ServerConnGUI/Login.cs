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

        int smallFormHeight = 220;
        int largeFormHeight;

        bool expectingDisconnect = false;

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
        private void EstablishConnection(string serverAddress, Action onConnect, Action<string> onFail)
        {
            //Check if we have a current connection, if we do then disconnect the previous one and make the new connection
            if (connection != null && connection.isConnected())
                manualDisconnect();

            //Update GUI status
            SetConnectionState(state.connecting);

            //Setup the new connection
            connection = new ConnectionLiaison(Disconnected, ReceivedSomething, callWhenWeHaveSendingResults);

            //Grab what we know about this server and setup the connection properties
            separatorInfo si = getGuaranteedSepInfo(serverAddress);
            connection.ESC = si.sep;

            //Try to connect to the server
            connection.tryToConnect(serverAddress, onConnect, onFail);
        }


        private void SafeGuiChange(Action toInvoke)
        {
            if (this.IsHandleCreated)
                this.Invoke((MethodInvoker)delegate { toInvoke(); });
        }


        //-------------------------------------- Call Backs for other threads. ----------------------------------------
        //------  If you change the GUI directly use "SafeGuiChange(Action a)", otherwise, you may get a "Cross-thread operation not valid" --------------

        
        public void nextSendPassword()
        {
            //Send password
            connection.sendPassword(PW_textbox.Text);
        }


        public void failedToConnect(string s)
        {
            SetConnectionState(state.lostConnection);

            //MessageBox does not rely on having access to the main Form thread so SafeGuiChange is not needed.
            MessageBox.Show("Error: " + s);
        }

        /// <summary>
        /// Called when the login loses a live connection
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="e"></param>
        public void Disconnected(SocketConnection sc, Exception e)
        {
            //if we expected to disconnect because we chose to
            if (expectingDisconnect)
            {
                SetConnectionState(state.notLoggedIn);
            }
            else
            {
                SetConnectionState(state.lostConnection);
                //MessageBox.Show("Lost connection"); //don't really need
            }
            expectingDisconnect = false;
        }


        /// <summary>
        /// Keeps track of all the servers we have connected to and the separator they each (supposedly) use.
        /// </summary>
        Dictionary<string, separatorInfo> serverSeparatorTracker = new Dictionary<string, separatorInfo>();

        /// <summary>
        /// Holds information for a character separator and how many times it failed to work.
        /// There should be one of these for each server we connect to.
        /// </summary>
        class separatorInfo
        {
            public char sep;
            public int failedCount;
            public separatorInfo(char sep)
            {
                this.sep = sep;
                failedCount = 0;
            }
        }

        
        /// <summary>
        /// If the server address is previously unknown, it will remember the server (with the default settings). 
        /// Whether the it had to be added or not, it will return the separator info for this server address.
        /// </summary>
        /// <param name="serverAddress">the server address (hostname+":"+port) to add if it already doesn't exist</param>
        /// <returns>Returns the separator infor associated with this server address</returns>
        private separatorInfo getGuaranteedSepInfo(string serverAddress)
        {
            if (!serverSeparatorTracker.ContainsKey(serverAddress))
            {
                serverSeparatorTracker.Add(serverAddress, new separatorInfo(ConnectionLiaison.DEFAULT_ESC));
            }

            return serverSeparatorTracker[serverAddress];
        }
       

        /// <summary>
        /// Method called when the server sends us something
        /// </summary>
        /// <param name="messenger"></param>
        public void ReceivedSomething(MessageReceivedFrom messenger)
        {
            //make sure to catch any issues and ignore it
            if (messenger == null || messenger.message==null || messenger.connection == null)
                return;

            //normally has the hostname and port
            string serverAddress = "unknown:" + ConnectionLiaison.DEFAULT_PORT;
            ConnectionLiaison c;

            //In case it wasn't a connectionliaison
            try
            {
                c = ((ConnectionLiaison)messenger.connection);
                serverAddress = c.hostname +":"+ c.port;
            }
            catch
            {

            }

            //get the separator info for the connection
            separatorInfo si = getGuaranteedSepInfo(serverAddress);

            //Split the message via the special delimiter
            string[] split = messenger.message.Split(connection.ESC);

            //Decide what to do with the message received
            //Password accepted, get a list of all available spreadsheets
            if (split[0] == "FILELIST")
            {
                //if our password was accepted then consider ourselves connected
                //ViewState already uses SafeGuiChange, so we don't need to do it here
                SetConnectionState(state.connected);

                //Safely fill out the listbox with the spreadsheet names
                SafeGuiChange(() =>
                    {
                        ssListBox.Items.Clear();
                        for (int i = 1; i < split.Length; i++)
                            ssListBox.Items.Add(split[i]);
                    });
            }
            else if (split[0] == "INVALID") //Password was rejected
            {
                //Simply disconnect form the server
                manualDisconnect();
                //set the custom status
                SafeGuiChange(() =>
                    {
                        StatusLabel.Text = "Invalid password";
                    });

                MessageBox.Show("Invalid password");
                return;
            }
            else if (split[0] == "ERROR")
            {
                MessageBox.Show("Error: " + split[1]);
            }
            else if (split[0].StartsWith("ERROR") && split[0].Length>=6) //Then our separator is different, so adapt and try to login again (one more attempt)
            {
                //increment the failed count
                si.failedCount++;

                //Check if we've already tried adapting
                if (si.failedCount == 1)
                {
                    //Adapt the separator
                    connection.ESC = split[0][5];
                    si.sep = split[0][5];
                    string encodedValue = "\\u" + ((int)split[0][5]).ToString("x4");
                    MessageBox.Show("Server appears to be using a different separator.\n Adapting this connection to use \"" + encodedValue + "\"");
                    //try to send password again
                    nextSendPassword();
                }
                else if (si.failedCount == 2) //check if the adaptation failed
                {
                    //Failed to adapt :(
                    MessageBox.Show("Failed to adapt to the server's communication protocol.");
                    //force disconnet
                    manualDisconnect();
                }
            }
            else
                MessageBox.Show("Unknown Message from server: \n" + messenger.message);

        }

        public void callWhenWeHaveSendingResults(Exception e, Object o)
        {
            if (e != null)
                MessageBox.Show("Failed to send message:" + o.ToString()+"\nError:"+e.Message);
        }

        /// <summary>
        /// Updates the GUI based off of what the connection state supposedly is.
        /// </summary>
        /// <param name="s">The new state of the connection</param>
        private void SetConnectionState(state s)
        {
            //Safely Change the GUI as other threads may have set this property
            SafeGuiChange(() =>
            {
                if (s == state.connecting)
                {
                    groupBox_connected.Enabled = false;
                    ServerButton.Enabled = true;

                    ServerButton.Text = "Please wait";
                    StatusLabel.Text = "Attempting to connect to server...";
                }
                else if (s == state.connected)
                {
                    ServerButton.Text = "Refresh";
                    StatusLabel.Text = "Connected";

                    groupBox_connected.Enabled = true;
                    this.Height = largeFormHeight;

                }
                else if (s == state.lostConnection)
                {
                    //this.Height = smallFormHeight;
                    groupBox_connected.Enabled = false;
                    StatusLabel.Text = "Lost Connection";
                    ServerButton.Enabled = true;
                    ServerButton.Text = "Connect";
                }
                else if (s == state.notLoggedIn)
                {
                    //this.Height = smallFormHeight;
                    groupBox_connected.Enabled = false;
                    StatusLabel.Text = "Not connected";
                    ServerButton.Enabled = true;
                    ServerButton.Text = "Connect";
                }
            });
            //remember the state
            _lState = s;
        }

        private state GetConnectionState()
        {
            return _lState;
        }


        /// <summary>
        /// Will make sure we are not connected to any server.
        /// </summary>
        private void manualDisconnect()
        {
            //check if we even need to disconnect
            if (connection!=null && connection.isConnected())
            {   //Disconnect
                expectingDisconnect = true;
                connection.CloseSocketConnection(); //this will activate the Disconnected method
            }
        }

        // --------------------------------------  Form Events ----------------------------------------------------

        private void ServerButton_Click(object sender, EventArgs e)
        {
            if (ServerButton.Enabled == true)
            {
                //Verify the validity of the server info:
                //if the IP isn't entered error box
                if (IP_textbox.Text.Length < 1)
                {
                    MessageBox.Show("Please enter a valid server IP address");
                }
                else
                {
                    //Try to connect and login to the server. This method shouldn't hold on to the GUI thread for very long.
                    EstablishConnection(IP_textbox.Text, nextSendPassword, failedToConnect);
                }
            }
        }

        //Attached to the connection info boxes
        private void textbox_keyDown(object sender, KeyEventArgs e)
        {
            //If we are using the connection info when the user is trying to change it...
            if (GetConnectionState() == state.connecting)
            {
                //make sure to suppress the changes. These text boxes should be disabled anyway.
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            //Allows for a refresh of the current connection
            if (e.KeyCode == Keys.Enter)  //only activate if the enter key was pressed
            {
                //Suppress the annoying ding
                e.Handled = true;
                e.SuppressKeyPress = true;

                //enter key is down, Do what we do when we click the ServerButton
                ServerButton_Click(sender, e);
            }

            if (GetConnectionState() == state.connected)
            {
                //suppress the key
                e.Handled = true;
                e.SuppressKeyPress = true;

                //Need to put popup in it's own thread so that this one can finish and
                //  successfully suppress the keystroke
                ThreadPool.QueueUserWorkItem((o) =>
                    {
                        //Ask if they really want to change the server
                        const string message = "You currently have a live connection, \nare you sure you want to terminate it?";
                        const string caption = "Notice";
                        var result = MessageBox.Show(message, caption,
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);

                        //If they do want to then disconnect from the current server
                        if (result == DialogResult.Yes)
                        {
                            manualDisconnect();
                        }
                    });

            }


        }

        /// <summary>
        /// Takes the selected item from the sender and opens a new SpreadsheetGUI with the current connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ssListBox_DoubleClick(object sender, EventArgs e)
        {
            var list = (ListBox)sender;

            //if there isn't anything selected, then do nothing
            if (list.SelectedItem == null)
                return;

            // Grab the selected item
            string selectedSpreadsheetName = list.SelectedItem.ToString();

            //make sure we have a connection
            if (connection != null && connection.isConnected())
            {
                //opens an new spreadsheet gui which takes over the connection
                new SpreadsheetGUIForm(connection, selectedSpreadsheetName, false).Show();
                connection = null;

                //Make a new connection with ther server
                ServerButton_Click(null, null);
            }
        }


        private void newSpreadsheet_textBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)  //only activate if the enter key was pressed
            {
                //Suppress the annoying ding
                e.Handled = true;
                e.SuppressKeyPress = true;

                //enter key is down, Do what we do when we create a new spreadsheet
                button1_Click(null, null);

            }
        }


        //called when we want to create a new spreadsheet
        private void button1_Click(object sender, EventArgs e)
        {
            string problem = null;
            //Make sure the spreadsheet name is valid
            if (newSpreadsheet_textBox.Text == "")
                problem = "Must enter a name.";
            else if (newSpreadsheet_textBox.Text.Contains('.'))
                problem = "Name cannot have any periods.";
            else if (newSpreadsheet_textBox.Text.Contains(ConnectionLiaison.DEFAULT_ESC))
                problem = "Name cannot contain the special separator character.";
            else if (newSpreadsheet_textBox.Text.Contains('\n'))
                problem = "Name cannot contain any new line characters";

            //Check if there was a problem with the spreadsheet name
            if (problem != null)
                MessageBox.Show("Invalid Spreadsheet name");
            else //open a spreadsheet gui and tell it to request a new spreadsheet from the server
            {
                //opens an new spreadsheet gui which takes over the connection
                lock (connection.GagLock)
                {
                    new SpreadsheetGUIForm(connection, newSpreadsheet_textBox.Text, true).Show();
                    connection = null;
                }

                newSpreadsheet_textBox.Text = "";

                //Make a new connection with ther server
                ServerButton_Click(null, null);
            }
        }

    }
}
