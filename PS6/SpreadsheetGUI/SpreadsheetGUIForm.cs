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

namespace SpreadsheetGUI
{

    public partial class SpreadsheetGUIForm : Form
    {

        #region Initializing stuff

        /// <summary>
        ///  All possible column letters
        /// </summary>
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// The place where the magic happens.
        /// </summary>
        Spreadsheet ss;

        /// <summary>
        /// If the spreadsheet was loaded from a file or if
        /// the spreadsheet has been saved to a file then
        /// hasFileReference will be true. Otherwise, false.
        /// </summary>
        bool hasFileReference = false;

        /// <summary>
        /// Private variable only used by fileName.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// Contains the path to the current file that 
        /// is loaded in the spreadsheet. Defaults to
        /// "\Untitled.ss" with a new spreadsheet.
        /// </summary>
        private string fileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                //extract the short file name
                shortFileName = Path.GetFileName(value);
            }
        }


        /// <summary>
        /// private variable used only by shortFileName
        /// </summary>
        private string _shortFileName;

        /// <summary>
        /// Only contains the file name, not the path to the file.
        /// When set, this updates the title of the form as well.
        /// </summary>
        private string shortFileName
        {
            get
            {
                return _shortFileName;
            }
            set
            {
                this.Text = " " + value + "  -  Spreadsheet";
                _shortFileName = value;
            }
        }

        /// <summary>
        /// The version which all loaded spreadsheets must be
        /// as well as the version which is recorded when
        /// a spreadsheet is saved.
        /// </summary>
        const string version = "ps6";

        /// <summary>
        /// Creates an empty new Spreadsheet Form
        /// </summary>
        public SpreadsheetGUIForm(ConnectionLiaison Connection, string SpreadsheetName)
        {
            initialize();
        }

        /* Disabled for CollaborativeSpreadsheet
        /// <summary>
        /// Brings up the open file dialog if loadFile is true.
        /// If they cancel the load or the file fails to load then
        /// the form does not open.
        /// </summary>
        /// <param name="loadFile"></param>
        public SpreadsheetGUIForm(bool loadFile)
        {
            //initialize form
            initialize();
            //check if they want to load the file
            if (loadFile)
                if (!openDialogAction()) //show the open form dialog
                    Close(); //if it doesn't load for some reason, then close this form.
        }
         * //*/

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
            //Display the loading box
            new LoadingBox().ShowDialog();





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
            this.ActiveControl = null;

            //create a blank spreadsheet
            ss = new Spreadsheet(isValid, normalize, version);

            //set initial fileName (including path)
            fileName = Directory.GetCurrentDirectory() + @"\Untitled.ss";

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


        //finished typing in content box, now try to update everything
        private void contentTextBox_Leave(object sender, EventArgs e)
        {

            //Annoying workaround for the MessageBox double Leave problem
            if (!contentIsFocused)
                return;
            contentIsFocused = false;

            //Validate content and update values/grid
            try
            {
                updateGUICells(ss.SetContentsOfCell(this.CellName.Text, this.contentTextBox.Text));
            }
            catch (Exception ex)
            {
                //If you run into any error, then display it
                MessageBox.Show("Error:" + ex.Message);
            }
            finally
            {
                //make sure the ssPanel display is set correctly
                //update cell info panel
                displaySelection(spreadsheetPanel1);
            }
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

        // Deals with the New menu
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newSpreadsheetAction();
        }

        private void newSpreadsheetAction()
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            /* Disabled for SS Collaboration Project */
            //SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetGUIForm());
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
                                        "      Ctrl+N   :   Opens a new spreadsheet in a new window." + "\n" +
                                        "      Ctrl+O   :   Opens an existing file in this same spreadsheet." + "\n" +
                                        "      Ctrl+S   :   Quickly saves the current spreadsheet to a file." + "\n" +
                                        "      Alt+F4   :   Closes the current spreadsheet." + "\n\n" +
                                        "    For more file options, check out the \"File\" menu left of the Help button" + "\n";
            MessageBox.Show(helpMessage, "Instructions");
        }


        //detects when certain buttons are pushed
        private void contentTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //If enter is pressed then lose focus (which activates contentTextBox_Leave event)
            if (e.KeyValue == 13)
                this.ActiveControl = null;
            else if (e.KeyValue == 46) //if the delete button was pressed then delete the contents of the current cell
            {
                //set the contents to empty
                this.contentTextBox.Text = "";
                //trigger the cell update
                this.ActiveControl = null;
            }
        }

        //allows for keyboard shorcuts
        private void contentTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //check if the Ctrl key is being pressed
            if (e.Control)
            {
                //37 left
                if (e.KeyValue == 37)
                {
                    moveCursorRelative(-1, 0);
                } //38 up
                else if (e.KeyValue == 38)
                {
                    moveCursorRelative(0, -1);
                } //39 right
                else if (e.KeyValue == 39)
                {
                    moveCursorRelative(1, 0);
                } //40 down
                else if (e.KeyValue == 40)
                {
                    moveCursorRelative(0, 1);
                } //other ctrl + keys
                else if (e.KeyValue == 78)  //Ctrl + N
                    newSpreadsheetAction();
                else if (e.KeyValue == 79)  //Ctrl + O
                    openDialogAction();
                else if (e.KeyValue == 83)  //Ctrl + S
                    saveFileAction();

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
        /// Describes whether or not the content box is focused.
        /// Part of Workaround for the Messagebox-double-leave problem.
        /// </summary>
        bool contentIsFocused = false;
        //Part of Workaround for the Messagebox-double-leave problem.
        private void contentTextBox_Enter(object sender, EventArgs e)
        {
            contentIsFocused = true;
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

        //Opens a dialog window for user to select file to load into spreadsheet
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openDialogAction();
            //check if load succeeded
            //if (!openDialogAction())
            //{
            //if load failed then clear the spreadsheet
            //    clearAll();
            //}
        }


        /// <summary>
        /// Shows the Open File Dialog for user to choose a file
        /// to open.
        /// </summary>
        /// <returns>True if user selected a file and it loaded correctly.</returns>
        private bool openDialogAction()
        {
            OpenFileDialog of = new OpenFileDialog();

            // Set filter options and filter index.
            of.Filter = "Spreadsheet Files|*.ss|All Files (*.*)|*.*";
            of.FilterIndex = 1;

            // Call the ShowDialog method to show the dialog box.
            // and check if they selected a document
            if (of.ShowDialog().ToString() == "OK") // or "Cancel"
            {
                //Check if current version has been saved or not
                if (ss.Changed)
                    //Ask if they want to continue without saving.
                    if (!areYouSure())
                        return false; //if they say no then stop.

                //otherwise, continue loading:

                //try to Load the new file
                try
                {
                    //Get a list of the old cells to wipe
                    IEnumerable<string> oldCellsToWipe = ss.GetNamesOfAllNonemptyCells();

                    //load the new spreadsheet data (might throw an error)
                    ss = new Spreadsheet(of.FileName, isValid, normalize, version);

                    //save the FileName
                    fileName = of.FileName;

                    //update the GUI's cells
                    updateGUICells(new HashSet<string>(ss.GetNamesOfAllNonemptyCells().Concat(oldCellsToWipe)));

                    //since we loaded from a file, our current spreadsheet has an actual file on the computer
                    hasFileReference = true;

                    //File sucessfully loaded
                    return true;
                }
                catch (Exception ex)
                {
                    //Show the message why it could not load
                    MessageBox.Show("Error Loading File: " + ex.Message + ".");

                    this.clearAll();
                    //Did not load
                    return false;
                }

            }
            //else they cancelled, so do nothing
            return false;
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
            if (hasFileReference)
            {
                //save the file without opening dialog
                saveFile(this.fileName);
            }
            else
            {
                //open a new dialog to select where to save the file
                saveFile();
            }
        }

        //Forces the OpenFileDialog to select where to save a file
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save file with dialog
            saveFile();
        }

        //Opens a saveFile Dilog and passes path to saveFile(string fileLocation)
        private void saveFile()
        {
            //Create dialog object
            SaveFileDialog sf = new SaveFileDialog();
            //Set filter options
            sf.Filter = "Spreadsheet Files|*.ss|All Files (*.*)|*.*";
            sf.FilterIndex = 1;

            //sets the default filename if it already has one
            if (hasFileReference)
            {
                //set the current directory
                sf.InitialDirectory = Path.GetDirectoryName(fileName);
                //set the default file name
                sf.FileName = this.shortFileName;
            }
            //only continue if they press "OK"
            if (sf.ShowDialog().ToString() == "OK")
            {
                saveFile(sf.FileName);
            }
        }

        //Saves data to the specified fileLocation (which includes the name)
        private void saveFile(string fileLocation)
        {
            try
            {
                //save the spreadsheet to a file
                ss.Save(fileLocation);
                //Update the current file
                fileName = fileLocation;
                //Saved file successfully
                hasFileReference = true;
            }
            catch (Exception ex)
            {
                //Display why the Save didn't work
                MessageBox.Show(ex.Message);
            }
        }

        //If there have been unsaved changed, make sure the user wants to close
        private void SpreadsheetGUIForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //check if there are any unsaved changed
            if (ss.Changed)
                e.Cancel = !areYouSure(); //bring up the confirmation dialog and tell the form to actually close or not
        }

        /// <summary>
        /// Brings up a confirmation dialog asking if the user is sure they want to continue
        /// </summary>
        /// <returns>Return true if they clicked Yes to "Are you sure you want to continue?"</returns>
        private bool areYouSure()
        {
            const string message = "There are unsaved changes to this spreadsheet.\nAre you sure you want to continue?";
            const string caption = "Wait a sec!";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }

        //Creates a new spreadsheet and tells it to immediately open a new dialog
        private void openInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*  Disabled for SS Colaboration Project 
            SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetGUIForm(true));//*/
        }
    }
}
