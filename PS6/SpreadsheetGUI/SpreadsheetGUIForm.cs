using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SpreadsheetGUI
{

    public partial class SpreadsheetGUIForm : Form
    {
        /// <summary>
        /// Holds and manages the connection with the server
        /// </summary>
        ConnectionLiaison connection;

        /// <summary>
        /// Another form used to display information while the spreadsheet is loading.
        /// </summary>
        LoadingBox loadingBox;
        /// <summary>
        /// Used for the first time the spreadsheet gui is opened
        /// </summary>
        bool firstShown = true;

        /// <summary>
        /// Determines whether to use OPEN or CREATE the first time this spreadsheet is opened.
        /// </summary>
        bool requestedNewSpreadsheet;

        /// <summary>
        /// Describes how the form will close. "True" will close the spreadsheet without any prompts or sent disconnect requests.
        /// </summary>
        bool forcedClosed = false;
        
        /// <summary>
        /// True if this spreadsheet client has already closed and is ignoring any more socket stuff.
        /// </summary>
        bool clientAlreadyClosed = false;

        /// <summary>
        /// This number respesents the version number the client believes the spreadsheet is on. 
        /// -1 means the number has not been set yet.
        /// </summary>
        int version_number = -1;

        #region Initializing stuff



        /// <summary>
        ///  All possible column letters
        /// </summary>
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// The place where the magic happens.
        /// </summary>
        Spreadsheet ss;


        private string _spreadsheetName;
        /// <summary>
        /// Constains the name of the spreadsheet
        /// </summary>
        private string spreadsheetName
        {
            get
            {
                return _spreadsheetName;
            }
            set
            {
                this.Text = " " + value + "  -  Spreadsheet";
                _spreadsheetName = value;
            }
        }


        /// <summary>
        /// The version which all loaded spreadsheets must be
        /// as well as the version which is recorded when
        /// a spreadsheet is saved.
        /// </summary>
        const string versionType = "ps6";

        /// <summary>
        /// Creates an empty new Spreadsheet Form
        /// </summary>
        public SpreadsheetGUIForm(ConnectionLiaison Connection, string SpreadsheetName, bool requestNewSpreadsheet)
        {
            this.requestedNewSpreadsheet = requestNewSpreadsheet;

            initialize();
            this.spreadsheetName = SpreadsheetName;

            //Take over the connection with the server
            this.connection = Connection;
            this.connection.setDirectOutputTo(CalledWhenDisconnected, receivedSomething);
            this.connection.callBack = receivedSendingResults;

            //Initially the form is disabled until it has loaded all the cell data
            this.Enabled = false;
        }

        /// <summary>
        /// initializes all components on creation.
        /// </summary>
        private void initialize()
        {
            InitializeComponent();

            //setup click event on spreadsheet
            spreadsheetPanel1.SelectionChanged += displaySelection;

            //finish initializing everything and clear them
            clearAll();
        }


        //Stuff to do after form loads
        private void SpreadsheetGUIForm_Load(object sender, EventArgs e)
        {
            //put focus on content textbox
            this.ActiveControl = this.contentTextBox;
        }

        /// <summary>
        /// Clears everything in the spreadsheet
        /// </summary>
        private void clearAll()
        {
            //clear the textbox
            contentTextBox.Text = "";
            //this.ActiveControl = null;  //TODO why was this here?

            //create a blank spreadsheet
            ss = new Spreadsheet(isValid, normalize, versionType);

            //set initial fileName (including path)
            //fileName = Directory.GetCurrentDirectory() + @"\Untitled.ss";
            //fileName = "";

            //clear the GUI cells
            spreadsheetPanel1.Clear();

            //set first selection to A1 (0,0)
            spreadsheetPanel1.SetSelection(0, 0);
        }

        /// <summary>
        /// Delegate to use in the spreadsheet validation process
        /// </summary>
        private bool isValid(string cellName)
        {
            return Regex.IsMatch(cellName, "^[a-zA-Z]+[1-9]\\d*$");
        }

        /// <summary>
        /// Delegate to use in the spreasheet validation process
        /// </summary>
        private string normalize(string cellName)
        {
            return cellName.ToUpper();
        }
        #endregion


        /// <summary>
        /// This method is called the first time the spreadsheet starts up and
        /// needs to load the cells from the server. When it is done loading, then hide 
        /// the loading box and show the spreadsheet.
        /// </summary>
        /// <param name="ssName"></param>
        public void askServerForFirstSpreadsheet()
        {
            //Show the loading box
            loadingBox = new LoadingBox(this);
            loadingBox.Show();

            ThreadPool.QueueUserWorkItem((o) =>
            {
                //Make the appropriate request to the server
                if (requestedNewSpreadsheet)
                {
                    connection.sendCreate(spreadsheetName);
                }
                else
                {
                    connection.sendOpen(spreadsheetName);
                }
            });
        }

        /// <summary>
        /// This method will hide the loading box and re-enable the form
        /// </summary>
        private void doneLoading()
        {
            //Make sure you use protection. God I'm tired.
            SafeGuiChange(() =>
                {
                    //Done with the loading box
                    loadingBox.Hide();
                    //Re-enable the spreadsheet form
                    this.Enabled = true;
                });
        }


        /// <summary>
        /// The method called when we lose the connection.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="e"></param>
        private void CalledWhenDisconnected(SocketConnection sc, Exception e)
        {
            if (!clientAlreadyClosed)
            {
                SafeGuiChange(() =>
                {
                    //Disable the spreadsheet
                    this.Enabled = false;

                    MessageBox.Show("Error on spreadsheet \"" + spreadsheetName + "\": \n - Lost connection with server.");
                    forcedClosed = true;

                    Close();
                });
            }
        }

        /// <summary>
        /// Method called whenever the server sends us something.
        /// The connection manager will not process any other 
        /// receive commands until this method has finished
        /// (So don't let this method stall for a long time)
        /// </summary>
        /// <param name="messenger"></param>
        private void receivedSomething(MessageReceivedFrom messenger)
        {
            //Remeber what cell were updated
            HashSet<string> cellsUpdated = new HashSet<string>();

            //We shouldn't be getting any nulls here, but just in case...
            if (messenger == null || messenger.message == null)
            {
                throw new Exception("Internal Error: passed an invalid message?");
            }

            try
            {
                ConnectionLiaison cl = ((ConnectionLiaison)messenger.connection);

                //Split the message via the special delimiter
                string[] split = messenger.message.Split(connection.ESC);

                //Decide what to do with the message received
                if (split[0] == "UPDATE")
                {
                    //Make sure the parsing is good
                    if (split.Length < 2)
                        throw new Exception("Error: Invalid UPDATE received.");

                    //first check version number
                    int givenVersion = 0;
                    if (!int.TryParse(split[1], out givenVersion))
                    {
                        throw new Exception("Error: Could not parse version number from UPDATE.");
                    }

                    //If we do not have a verson number stored yet, just take theirs
                    if (version_number < 0)
                        version_number = givenVersion;
                    else //verify it's the version we expect
                    {
                        //if the given version is not one more than the version we have
                        if (++version_number != givenVersion)
                        {
                            //Then we need to resync instead of continuing.
                            cl.sendResync();
                            return;
                        }
                    }

                    SafeGuiChange(() =>
                    {
                        try
                        {
                            //Grab the cell stuff and put it into the spreadsheet
                            for (int i = 2; i < split.Length; i += 2)
                            {
                                addCellToSpreadsheet(split[i], split[i + 1]);
                            }

                            //Update the spreadsheet view
                            displaySelection(spreadsheetPanel1);
                        }
                        catch
                        {
                            //TODO report?
                        }
                            //Let the speadsheet open when we're done loading
                            doneLoading();
                    });
                }
                else if (split[0] == "SAVED")
                {
                    MessageBox.Show("SAVED!");
                }
                else if (split[0] == "SYNC")
                {

                    //first check version number
                    int givenVersion = 0;
                    if (!int.TryParse(split[1], out givenVersion))
                    {
                        throw new Exception("Error: Could not parse version number from UPDATE.");
                    }


                    //if the given version is not one more than the version we have
                    if (++version_number != givenVersion)
                    {
                        //Then we need to resync instead of continuing.
                        cl.sendResync();
                        return;
                    }
                    
                    //Update the cells
                    SafeGuiChange(() =>
                    {
                        //Grab the cell stuff and put it into the spreadsheet
                        for (int i = 2; i < split.Length; i += 2)
                        {
                            addCellToSpreadsheet(split[i], split[i + 1]);
                        }

                        //Update the spreadsheet view
                        displaySelection(spreadsheetPanel1);
                        //Let the speadsheet open when we're done loading
                        doneLoading();
                    });


                }
                else if (split[0] == "ERROR")
                {
                    //** TODO?
                    MessageBox.Show("Server Error: " + split[1]);
                }
                else
                    MessageBox.Show("Unknown Message from server: \n" + messenger.message);

            } 
            catch (Exception e)
            {
                /*
                SafeGuiChange(() =>
                    {
                        //TODO update status label with exception?
                        MessageBox.Show("Error processing received message:\n" + e.Message);
                    });
                //*/
            } 
        }


        /// <summary>
        /// Ensures any changes to the GUI will be made in the appropriate thread. Example:
        /// 
        ///   SafeGuiChange(() =>
        ///   {
        ///       label1.text = "I can change!";
        ///   });
        /// 
        /// </summary>
        /// <param name="toInvoke"></param>
        private void SafeGuiChange(Action toInvoke)
        {
            if (this.IsHandleCreated)
                this.Invoke((MethodInvoker)delegate { toInvoke(); });
        }

        /// <summary>
        /// Attempts to add the contents of the content box to the spreadsheet
        /// </summary>
        private void addCellToSpreadsheet(string CellName, string CellContent)
        {
            //Add the cell and, sure, update the GUI while we're at it.
            updateGUICells(ss.SetContentsOfCell(CellName, CellContent));
        }

        
        /// <summary>
        /// Attempts to add the contents of the content box to the spreadsheet
        /// </summary>
        private void processContentTextBox()
        {
            //TODO convert to our needs
            connection.sendEnter(version_number,this.CellName.Text, this.contentTextBox.Text);
        }


        //checks if it already has a fileReference, if not then do the same thing as "Save As.."
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileAction();
        }

        /// <summary>
        /// Initiates the Save file action (same as Ctrl+S)
        /// </summary>
        private void saveFileAction()
        {
            connection.sendSave(version_number);
        }


        private void SpreadsheetGUIForm_Shown(object sender, EventArgs e)
        {
            //When the spreadsheet is shown 
            if (firstShown)
            {
                firstShown = false;
                askServerForFirstSpreadsheet();
            }
        }

        /// <summary>
        /// Called when we have the results from sending a message (not the response from the server, 
        /// but rather, if the message made it to it's destination all right).
        /// </summary>
        /// <param name="e"></param>
        /// <param name="o"></param>
        public void receivedSendingResults(Exception e, Object o)
        {
            if (e != null)
                MessageBox.Show("Failed to send message:" + o.ToString() + "\nError:" + e.Message);
        }

        //----------  Original Spreadsheet stuff (mostly) -----
        #region Original GUI methods

        /// <summary>
        /// Updates the display (mainly the cell info) when a cell is clicked
        /// </summary>
        /// <param name="spreadsheetPanel">The spreadsheet panel of cells</param>
        private void displaySelection(SpreadsheetPanel spreadsheetPanel)
        {
            //the row and column of the currently selected cell
            int row, col;
            //the value of the currently selected cell
            String value;
            //an intermediate holder for the content of the selected cell
            object preContent;
            //the content of the selected cell
            string content;
            //the cell name of the selected cell
            string cellName;

            //find out which cell was selected
            spreadsheetPanel.GetSelection(out col, out row);

            //Get the contents and value of the selected cell
            cellName = getCellName(row, col);
            //figure out what the content is
            preContent = ss.GetCellContents(cellName);
            content = preContent.ToString();
            //put a '=' in front if it's a formula
            if (preContent is Formula)
                content = "=" + preContent.ToString();
            else
                content = preContent.ToString();

            //determine the correct value
            value = getProperValue(cellName);

            //Update the cell info panel display
            this.CellName.Text = cellName;
            this.contentTextBox.Text = content;
            this.valueLabel.Text = value;

            //put focus on the text box
            this.ActiveControl = this.contentTextBox;
            this.contentTextBox.Select(this.contentTextBox.Text.Length, 0);
        }



        /// <summary>
        /// Updates the visuals of all the cells listed.
        /// </summary>
        /// <param name="toUpdate">A set of all the cell names that need to be updated.</param>
        private void updateGUICells(ISet<string> toUpdate)
        {
            int row, col;
            //set the contents of the cell, then update the graphics for each cell that was changed
            foreach (string cell in toUpdate)
            {
                //get the coordinates of the cell to update
                getCellCoordinates(cell, out row, out col);
                //set the visual value of the cell
                spreadsheetPanel1.SetValue(col, row, getProperValue(cell));
            }

            //Make sure cell info is updated as well
            displaySelection(spreadsheetPanel1);
        }


        // Deals with the Close menu
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        //Show help menu
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            const string helpMessage = "Below are some features of this Spreadsheet program:\n\n" +
                                        "  ¬ You can navigate the spreadsheet by either clicking on a \n" +
                                        "    cell or by using Ctrl+<Arrow_Key> navigate the spreadsheet." + "\n\n" +
                                        "  ¬ To quickly delete a cell, simply press the \"Delete\"" + "\n" +
                                        "    button on your keyboard. " + "\n\n" +
                                        "  ¬ To the right of the Help button is the cell info panel." + "\n" +
                                        "    This panel contains the name of the cell you have selected, as" + "\n" +
                                        "    well as its contents (which you can edit) and its calculated value." + "\n\n" +
                                        "  ¬ To edit a cell, simply have the desired cell selected and begin" + "\n" +
                                        "    typing. To save the contents you must press \"Enter\"." + "\n\n" +
                                        "  ¬ Here are some more keyboard shortcuts:" + "\n" +
                                        "      Ctrl+S   :   Quickly saves the current spreadsheet to a file." + "\n" +
                                        "      Alt+F4   :   Closes the current spreadsheet." + "\n";
            MessageBox.Show(helpMessage, "Instructions");
        }

        //activate when we press a button while in the cell content box
        private void contentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //describes if a special key action was taken
            bool didSomething = false;

            //If enter is pressed
            if (e.KeyValue == 13)
            {
                processContentTextBox();
                didSomething = true;
            }
            else if (e.KeyValue == 46) //if the delete button was pressed then delete the contents of the current cell
            {
                //set the contents to empty
                this.contentTextBox.Text = "";
                //update the spreadsheet with the new cell info
                processContentTextBox();
                didSomething = true;
            }

            //check if the Ctrl key is being pressed
            if (e.Control)
            {
                //37 left
                if (e.KeyValue == 37)
                {
                    moveCursorRelative(-1, 0);
                    didSomething = true;
                } //38 up
                else if (e.KeyValue == 38)
                {
                    moveCursorRelative(0, -1);
                    didSomething = true;
                } //39 right
                else if (e.KeyValue == 39)
                {
                    moveCursorRelative(1, 0);
                    didSomething = true;
                } //40 down
                else if (e.KeyValue == 40)
                {
                    moveCursorRelative(0, 1);
                    didSomething = true;
                } //other ctrl + keys  *** Some disabled for the collaborative spreadsheet project ***
                //else if (e.KeyValue == 78)  //Ctrl + N
                //    newSpreadsheetAction();
                //else if (e.KeyValue == 79)  //Ctrl + O
                //    openDialogAction();
                else if (e.KeyValue == 83)  //Ctrl + S
                {
                    saveFileAction();
                    didSomething = true;
                }
            }

            //Check if we handled any special key strokes
            if (didSomething)
            {
                //Declare we handled the keypress already and don't apply it to anything else
                //These will suppress the annoying ding sound when you do something valid
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        /// <summary>
        /// Moves the cursor to (row+x,col+y) if
        /// it is a valid position.
        /// </summary>
        /// <param name="x">The positive or negative number of rows to move the cursor.</param>
        /// <param name="y">The positive or negative number of columns to move the cursor.</param>
        /// <returns>True if cursor move is successful</returns>
        private void moveCursorRelative(int y, int x)
        {
            int row, col;
            //get the current position
            spreadsheetPanel1.GetSelection(out col, out row);
            //adjust the position
            spreadsheetPanel1.SetSelection(col + y, row + x);
            //update the display
            displaySelection(spreadsheetPanel1);
        }

        /// <summary>
        /// Gets the cell name of the position in the spreadsheet.
        /// </summary>
        private string getCellName(int row, int col)
        {
            if (col >= 0 && col <= 25)
            {
                return letters[col] + (row + 1).ToString();
            }
            else //otherwise throw an error
                throw new Exception("Invalid row/column to retrieve");
        }

        /// <summary>
        /// Returns the row and column of the named cell.
        /// </summary>
        private void getCellCoordinates(string cellName, out int row, out int col)
        {
            //the exception to throw if something goes wrong when parsing the cell name
            Exception aFit = new InvalidNameException();

            //check for valid input
            if (cellName == null)
                throw aFit;
            //try to pull cell Name apart
            string[] nameSplit = Regex.Split(cellName, "([a-zA-Z]+|%d+)");
            //Must be split into 3 parts (first element should be empty)
            if (nameSplit.Length != 3)
                throw aFit;

            //try to find the column given the letter
            col = letters.IndexOf(nameSplit[1].ToUpper());
            //if we can't find the column letter..
            if (col == -1)
                throw aFit;

            //try to parse the row
            if (int.TryParse(nameSplit[2], out row))
                row--; //to make sure that A1 maps to 0,0
            else
                throw aFit;

            //Otherwise, we're done!
        }


        /// <summary>
        /// Returns the proper value to display rather
        /// than possibly a "Spreadsheet.FormulaError" value.
        /// </summary>
        /// <param name="cellName"></param>
        /// <returns></returns>
        private string getProperValue(string cellName)
        {
            //determine the correct value
            object preValue = ss.GetCellValue(cellName);
            if (preValue is FormulaError)
                return ((FormulaError)preValue).Reason;
            else
                return preValue.ToString();
        }


        //If there have been unsaved changed, make sure the user wants to close
        private void SpreadsheetGUIForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if we aren't trying to force the client to shutdown
            if (!forcedClosed)
            {
                DialogResult dr = areYouSure();
                if (dr == DialogResult.Yes)
                    connection.sendDisconnect();
                else if (dr == DialogResult.No)
                {
                    //Simply let the server disconnect
                }
                else //Anything else, cancel closing
                {
                    e.Cancel = true;
                    return;
                }
            }

            //Ignore all further communications from the server
            clientAlreadyClosed = true;
        }

        /// <summary>
        /// Brings up a confirmation dialog asking if the user is sure they want to continue
        /// </summary>
        /// <returns>Return true if they clicked Yes to "Are you sure you want to continue?"</returns>
        private DialogResult areYouSure()
        {
            const string message = "Do you want to save changes before you close?";
            const string caption = "Wait a sec!";
            return MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNoCancel,
                                         MessageBoxIcon.Question);
        }

        #endregion

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connection.sendUndo(version_number);
        }

    }
}
