using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SS
{
    /// <summary>
    /// Realizes AbstractSpreadsheet
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Holds a reference to all the cells on this spreadsheet with 
        /// the key being the cell name.
        /// </summary>
        Dictionary<string, Cell> cells;

        /// <summary>
        /// Keeps track of the cell dependencies.
        /// </summary>
        DependencyGraph dg;


        /// <summary>
        /// Creates an empty spreadsheet with default settings.
        /// </summary>
        public Spreadsheet()
            : base((s) => true, (s) => s, "default")  //isValid always true, Normalize returns the same thing, Version is "default"
        {
            //create empty spreadsheet
            initializeEmptySpreadsheet();
        }

        /// <summary>
        /// Creates an empty spreadsheet with the given properties.
        /// </summary>
        /// <param name="isValid">Determines whether or not a given string is a valid variable.</param>
        /// <param name="normalize">Turns a string containing a variable into a standard format.</param>
        /// <param name="version">Defines the version of the spreadsheet.</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            //create empty spreadsheet
            initializeEmptySpreadsheet();
        }

        /// <summary>
        /// Loads a spreadsheet from a file and attempts to load it with the properties given here.
        /// </summary>
        /// <param name="fileName">Path and File name of file to load.</param>
        /// <param name="isValid">Determines whether or not a given string is a valid variable.</param>
        /// <param name="normalize">Turns a string containing a variable into a standard format.</param>
        /// <param name="version">Defines the version of the spreadsheet.</param>
        public Spreadsheet(string fileName, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            //create empty spreadsheet
            initializeEmptySpreadsheet();

            //Load xml into spreadsheet memory
            loadXmlSpreadsheet(fileName);
        }

        /// <summary>
        /// Resets all the cells in the spreadsheet as well as the dependency graph.
        /// </summary>
        private void initializeEmptySpreadsheet()
        {
            cells = new Dictionary<string, Cell>();
            dg = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<String> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(String name)
        {
            name = validateCellName(name); //throws InvalidNameException if name is invalid

            //if cell doesn't exist return an empty string
            if (!cells.ContainsKey(name))
                return "";

            return cells[name].content;
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, double number)
        {
            //makes sure cell name is valid
            name = validateCellName(name);

            //Removes cell's old dependees
            removeOldDependeesFromCell(name);

            Cell newCell = new Cell();
            newCell.conType = cType.dbl;
            newCell.content = number;

            //replace or add new cell contents
            if (cells.ContainsKey(name))
                cells[name] = newCell;
            else
                cells.Add(name, newCell);

            //return the list of cells that need to be updated
            return new HashSet<String>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, String text)
        {
            //make sure text is valid
            if (text == null)
                throw new ArgumentNullException();

            //makes sure cell name is valid
            name = validateCellName(name);

            //Removes cell's old dependees
            removeOldDependeesFromCell(name);

            //if text is empty, then erase the cell from the list
            if (text == "")
            {
                cells.Remove(name);
                return new SortedSet<String>(GetCellsToRecalculate(name));
            }

            Cell newCell = new Cell();
            newCell.conType = cType.str;
            newCell.content = text;

            //replace or add new cell contents
            if (cells.ContainsKey(name))
                cells[name] = newCell;
            else
                cells.Add(name, newCell);

            //return the list of cells that need to be updated
            return new HashSet<String>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<String> SetCellContents(String name, Formula formula)
        {
            //formula must be valid
            if (formula == null)
                throw new ArgumentNullException();

            //makes sure cell name is valid
            name = validateCellName(name);

            //Save the Old One!
            Cell oldCell;
            if (cells.ContainsKey(name))
            {
                //grab the old cell
                oldCell = cells[name];
            }
            else
            {
                //otherwise the old cell would have been a blank one
                oldCell = new Cell();
                oldCell.conType = cType.str;
                oldCell.content = "";
            }

            //Removes cell's old dependees if any
            removeOldDependeesFromCell(name);

            Cell newCell = new Cell();
            newCell.conType = cType.fa;
            newCell.content = formula;

            //replace or add new cell contents
            if (cells.ContainsKey(name))
                cells[name] = newCell;
            else
                cells.Add(name, newCell);

            //add new depencies
            foreach (string s in formula.GetVariables())
                dg.AddDependency(s, name);

            ISet<String> toRecalc = new HashSet<string>();

            try
            {
                toRecalc = new HashSet<String>(GetCellsToRecalculate(name));
            }
            catch (CircularException e)
            {
                ISet<string> toUpdate = new HashSet<string>();
                //Determine the type of the old cell and re-add it.
                //Do not need to recalculate any cells because we
                //are reverting back to the old cell and cell value
                if (oldCell.conType == cType.dbl)
                    toUpdate = this.SetCellContents(name, (double)oldCell.content);
                else if (oldCell.conType == cType.str)
                    toUpdate = this.SetCellContents(name, (string)oldCell.content);
                else if (oldCell.conType == cType.fa)
                    toUpdate = this.SetCellContents(name, (Formula)oldCell.content);

                //see if there are any updates that need to be done
                updateCellValues(toUpdate);

                //rethrow the error after adding the old cell back in
                throw e;
            }

            //return the list of cells that need to be updated
            return toRecalc;
        }


        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<String> GetDirectDependents(String name)
        {
            if (name == null)
                throw new ArgumentNullException("Cannot determine direct dependents from a \"null\" cell name");

            name = validateCellName(name); //throws InvalidNameException if name is invalid

            return dg.GetDependents(name); //return the (direct) dependents of the named cell
        }


        /// <summary>
        /// Removes all dependees on the named cell.
        /// Requires Valid cell name.
        /// </summary>
        /// <param name="name">A valid name for a cell to remove the dependees of.</param>
        protected void removeOldDependeesFromCell(string name)
        {

            //if named cell doesn't exist yet, then we're done
            if (!cells.ContainsKey(name))
                return;

            if (cells[name].conType == cType.fa)
            {
                //get the formula
                Formula oldFormula = (Formula)cells[name].content;

                //remove all dependees (replaces them with an empty list)
                foreach (string s in oldFormula.GetVariables())
                    dg.ReplaceDependees(name, new List<string>());

                //cells[name].content = new Formula("0");
            }
            //Do nothing if it's not a formula cell
        }


        /// <summary>
        /// A string is a valid cell name if and only if:
        ///   (1) its first character is an underscore or a letter
        ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits. 
        /// If a cell name is invalid this method will throw an InvalidNameException
        /// </summary>
        /// <param name="name">the cell name to validate</param>
        /// <returns>The name but normalized.</returns>
        protected string validateCellName(string name)
        {
            if (name != null)
            {
                //normalize the name
                name = Normalize(name);
                //name must follow the passed rules of IsValid as well as our base definition
                if (IsValid(name) && Regex.IsMatch(name, @"^[a-zA-Z]+\d+$")) // "^[a-zA-Z_](?:[a-zA-Z_]|\\d)*$"))
                {
                    return name;
                }
            }
            throw new InvalidNameException();
        }


        /// <summary>
        /// Used to describe the type of the content or value.
        /// </summary>
        protected enum cType
        {
            /// <summary>
            /// string type
            /// </summary>
            str,
            /// <summary>
            /// double type
            /// </summary>
            dbl,
            /// <summary>
            /// Formula type
            /// </summary>
            fa,
            /// <summary>
            /// FormualError type
            /// </summary>
            fe
        }

        /// <summary>
        /// A simple container class. Relinquishes all functionality to 
        /// the Spreadsheet class which contains it.
        /// </summary>
        protected class Cell
        {
            /// <summary>
            /// Describes the type of content stored.
            /// </summary>
            public cType conType;
            /// <summary>
            /// Holds the content of a cell. Must be a double,
            /// string, or Formula type.
            /// </summary>
            public object content;

            /// <summary>
            /// Describes the type of value stored
            /// </summary>
            public cType valType;

            /// <summary>
            /// Holds the value of a cell. Must be a stirng, 
            /// double, or FormulaError.
            /// </summary>
            public object value;

            /// <summary>
            /// Constructs an empty cell.
            /// </summary>
            public Cell()
            {
                conType = cType.str;
                content = "";
                valType = cType.str;
                value = "";
            }

        }


        /// <summary>
        /// Attempts to load data from 'filename' into Spreadsheet memory, checking against
        /// the properties stored in the spreadsheet (isValid, Normalize, version)
        /// </summary>
        /// <param name="fileName">Path and file name to load.</param>
        private void loadXmlSpreadsheet(string fileName)
        {
            //grab some memory
            XmlNode nodeName = null;
            XmlNode nodeContents = null;
            string name = String.Empty;
            string contents = String.Empty;

            //Catch, wrap, and throw any errors that occur
            try
            {
                //loads the document and checks if it's a valid format
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);

                //reference the spreadsheet node
                XmlNode s = doc.FirstChild;

                //First node could be:
                //<?xml version="1.0" encoding="utf-8"?>
                if (s.Name.ToLower() == "xml")
                    s = s.NextSibling;

                //Make sure root node is valid
                if (s.Name != "spreadsheet")
                    throw new SpreadsheetReadWriteException("Root node is invalid");

                if (s.Attributes.Count != 1)
                    throw new SpreadsheetReadWriteException("Invalid number of attributes for <spreadsheet>");

                if (s.Attributes[0].Name != "version" ||
                    s.Attributes[0].Value != Version)
                {
                    throw new SpreadsheetReadWriteException("Incorrect version number");
                }

                //loop through all child nodes
                foreach (XmlNode c in s.ChildNodes)
                {
                    //make sure node is named cell
                    if (c.Name != "cell")
                        throw new SpreadsheetReadWriteException("All child nodes under <spreadsheet..> must be <cell>");

                    //<cell> can't have any attributes
                    if (c.Attributes.Count != 0)
                        throw new SpreadsheetReadWriteException("<cell> cannot have any attributes");

                    //quick catch for number of nodes inside <cell>
                    if (c.ChildNodes.Count != 2)
                        throw new SpreadsheetReadWriteException("Invalid number of child nodes in <cell>");

                    {//Simply for indenting

                        //grab <name>
                        nodeName = c["name"];
                        //check if <name> exists
                        if (nodeName == null)
                            throw new SpreadsheetReadWriteException("<name> does not exist in <cell>");

                        //<name> cannot have any attributes
                        if (nodeName.Attributes.Count != 0)
                            throw new SpreadsheetReadWriteException("<name> cannot have any attributes");


                        //<name> can only contain text
                        if (nodeName.ChildNodes.Count != 1 || nodeName.ChildNodes[0].GetType().Name != "XmlText")
                            throw new SpreadsheetReadWriteException("<name> can only contain text");


                        //grab the text of name
                        name = nodeName.InnerText.Trim();


                        //grab <contents>
                        nodeContents = c["contents"];
                        //check if <contents> exists
                        if (nodeContents == null)
                            throw new SpreadsheetReadWriteException("<contents> does not exist in <cell>");

                        //<name> cannot have any attributes
                        if (nodeContents.Attributes.Count != 0)
                            throw new SpreadsheetReadWriteException("<contents> cannot have any attributes");

                        //<contents> can only contain text
                        if (nodeContents.ChildNodes.Count != 1 || nodeContents.ChildNodes[0].GetType().Name != "XmlText")
                            throw new SpreadsheetReadWriteException("<contents> can only contain text");

                        //grab the text of contents
                        contents = nodeContents.InnerText.Trim();
                    }

                    //try to add items to list, if successful then update the values of the affected cells
                    updateCellValues(SetContentsOfCell(name, contents));


                }
                //end of all child nodes in spreadsheet
                this.Changed = false;
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }


        /// <summary>
        /// Calculates and updates all the values of the given cell names.
        /// </summary>
        /// <param name="toUpdate">Ordered set of all cells to update.</param>
        private void updateCellValues(ISet<string> toUpdate)
        {
            Cell cur = null;
            foreach (string name in toUpdate)
            {
                //grab the cell to update
                //if the cell doesn't exist (because the content was set to "")
                //  then there is no need to update it's value
                if (!cells.ContainsKey(name))
                    continue;
                //otherwise, get the named cell
                cur = cells[name];

                //figure out the value of the cell given the type of it's content
                if (cur.conType == cType.dbl) //if content is a double
                {
                    //set the value to a double
                    cur.valType = cType.dbl;
                    cur.value = cur.content;
                }
                else if (cur.conType == cType.fa) //if the content is a formula
                {
                    try
                    {
                        //Evaluate the content
                        object formulaReturn = ((Formula)cur.content).Evaluate(lookup);
                        if (formulaReturn is double)
                        {
                            //set the value to a double
                            cur.valType = cType.dbl;
                            cur.value = formulaReturn;
                        }
                        else
                        {
                            //set the value as a formula error
                            cur.valType = cType.fe;
                            cur.value = formulaReturn;
                        }
                    }
                    catch (Exception e)
                    {
                        //Wrap all errors as a formula error
                        new FormulaError(e.Message);
                    }

                }
                else //simply copy over the values (because it's a string)
                {
                    cur.valType = cur.conType;
                    cur.value = cur.content;
                }
            }

            //if updating cell values was successful (and there were some to update) then the spreadsheet has changed
            if (toUpdate.Count > 0)
                Changed = true;
        }

        /// <summary>
        /// Returns the double value of the given variable. If the named cell does
        /// not have a double as a value then this throws an ArgumentException.
        /// </summary>
        /// <param name="name">Cell name to extract the value of.</param>
        /// <returns></returns>
        private double lookup(string name)
        {   //unsafe use of name, I know. It shouldn't ever come to that.

            //if the cell doesn't 
            if (!cells.ContainsKey(name))
                throw new ArgumentException("Cell \"" + name + "\" does not have a value.");

            //if the value is not a double
            if (cells[name].valType != cType.dbl)
                throw new ArgumentException("Cell \"" + name + "\" does not contain a double");

            //otherwise return the value of the named cell
            return (double)cells[name].value;
        }

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get;
            protected set;
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            string extractedVersion = null;
            try
            {
                //Handle the xml stream reader correctly
                using (XmlReader r = XmlReader.Create(filename))
                {
                    r.MoveToContent();
                    if (r.Name != "spreadsheet")
                        throw new SpreadsheetReadWriteException("Fist tag must be a spreadsheet!");
                    //grab the version number
                    extractedVersion = r.GetAttribute("version");
                    if (extractedVersion == null)
                        throw new SpreadsheetReadWriteException("<spreadsheet> does not have the attribute \"version\"");
                }
            }
            catch (Exception e)
            {
                //catch error and rethrow as SpreadsheetReadWriteException (including error message)
                throw new SpreadsheetReadWriteException(e.Message);
            }

            return extractedVersion;
        }


        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        ///   <name>
        ///     cell name goes here
        ///   </name>
        ///   <contents>
        ///     cell contents goes here
        ///   </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            //set the settings for the xml writer
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            string toWrite = String.Empty;

            //any thrown errors get translated into a SpreadsheetReadWriteException
            try
            {

                //create the writer and correctly handle it.
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    //Opening tag
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);


                    Cell cell = null;
                    foreach (string name in GetNamesOfAllNonemptyCells())
                    {
                        //<cell>
                        writer.WriteStartElement("cell");

                        //<name> a1 </name>
                        writer.WriteElementString("name", name);

                        //Retrieve the contents of name
                        cell = cells[name];
                        //If the content is a formula, put it in proper formula format ("=")
                        if (cell.conType == cType.fa)
                            toWrite = "=" + cell.content;
                        else //otherwise, just grab the double or string
                            toWrite = cell.content.ToString();

                        //Write <contents> =b1+c1 </contents>
                        writer.WriteElementString("contents", toWrite);

                        //</cell>
                        writer.WriteEndElement();
                    }

                    //</spreadsheet> tag
                    writer.WriteEndElement();
                }

            }
            catch (Exception e)
            {
                //rethrow the error but as a SpreadsheetReadWriteException (keeping the message)
                throw new SpreadsheetReadWriteException(e.Message);
            }

            //Otherwise, write was sucessful
            Changed = false;
        }


        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            //make sure name is valid
            name = validateCellName(name);

            //if the cell doesn't exist, the default value is 0
            if (!cells.ContainsKey(name))
                return "";

            return cells[name].value;
        }


        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            ISet<string> toChange = null;

            //make sure content contains something
            if (content == null)
                throw new ArgumentNullException("Content cannot be null");

            //make sure the cell name is valid
            name = validateCellName(name);

            //if the content can be parsed as a double
            double resultOfDoubleParse = 0;
            if (double.TryParse(content, out resultOfDoubleParse))
            {
                toChange = SetCellContents(name, resultOfDoubleParse);
            }
            else if (content.Length > 0 && content[0] == '=')
            {
                //try to parse the rest as a formula
                toChange = SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            }
            else
            {
                //simply set it as a string
                toChange = SetCellContents(name, content);
            }

            //If there is at least one cell to update, then calculate the values
            if (toChange.Count > 0)
            {
                Changed = true;
                //update the values
                updateCellValues(toChange);
            }
            //Ignore me: Quiet Change..

            return toChange;
        }
    }
}
