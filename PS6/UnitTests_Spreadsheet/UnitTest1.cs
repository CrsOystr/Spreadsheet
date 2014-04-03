using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System.IO;

namespace UnitTests_Spreadsheet
{
    [TestClass]
    public class UnitTests
    {
        /*

        #region delegates for isValid and normalize and file references
        //--------------------------------------------------------------------------
        private Func<string, bool> isValid_always = (s) => true;

        private Func<string, string> normalize_same = (s) => s;
        private Func<string, string> normalize_toUpper = (s) => s.ToUpper();

        static string testFolderLocation = @"..\..\..\testFiles\";

        //--------------------------------------------------------------------------
        #endregion

        #region Test Helper Methods
        //-------------------------------------------------------------

        /// <summary>
        /// Turns set into a string array
        /// </summary>
        /// <param name="set">Set of Strings</param>
        /// <returns>Array of Strings from the 'set'</returns>
        private string[] iSetToArray(ISet<String> set)
        {
            string[] output = new string[set.Count];
            int index = 0;
            foreach (string s in set)
                output[index++] = s;

            return output;
        }

        /// <summary>
        /// Turns an IEnumerable into an array
        /// </summary>
        /// <param name="ienum"></param>
        /// <returns></returns>
        private string[] iEnumToArray(IEnumerable<String> ienum)
        {
            List<String> list = new List<string>();
            foreach (string s in ienum)
                list.Add(s);

            return list.ToArray();
        }


        //-------------------------------------------------------------
        #endregion


        #region initial tests
        //-------------------------------------------------------------
        [TestMethod]
        public void zz_initial_setCellContents_double()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "3.14"); //ignore output for now
            //should not throw an error
        }

        [TestMethod]
        public void zz_initial_setCellContents_string()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "this is a string"); //ignore output for now
            //should not throw an error
        }

        [TestMethod]
        public void zz_initial_setCellContents_formula()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=X3"); //ignore output for now
            //should not throw an error
        }
        //-------------------------------------------------------------
        #endregion


        #region SetCellContents_double
        //-------------------------------------------------------------

        [TestMethod()]
        public void scc_double_1()
        {
            string[] expected = { "a1", "b1", "a2" };
            string[] actual;
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "3.0");
            ss.SetContentsOfCell("b1", "4.1");
            ss.SetContentsOfCell("a2", "2.1");
            ss.SetContentsOfCell("b1", "1.01");
            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void scc_double_2()
        {
            Spreadsheet ss = new Spreadsheet();
            double expected = 1.01;
            ss.SetContentsOfCell("a1", "3.0");
            ss.SetContentsOfCell("b1", "4.1");
            ss.SetContentsOfCell("a2", "2.1");
            ss.SetContentsOfCell("b1", "1.01");
            double actual = (double)ss.GetCellContents("b1");
            Assert.AreEqual(expected, actual);
        }

        //-------------------------------------------------------------
        #endregion


        #region SetCellContents_string
        //-------------------------------------------------------------

        [TestMethod()]
        public void scc_string_1()
        {
            string[] expected = { "a1", "b1", "a2" };
            string[] actual;
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "hi");
            ss.SetContentsOfCell("b1", "hello");
            ss.SetContentsOfCell("a2", "0");
            ss.SetContentsOfCell("b1", "howdy");
            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());

            Array.Sort(expected);
            Array.Sort(actual);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void scc_string_2()
        {
            string[] expected = {"b1", "c1" };
            string[] actual;
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "hi");
            ss.SetContentsOfCell("b1", "hello");
            ss.SetContentsOfCell("a1", "");
            ss.SetContentsOfCell("b1", "howdy");
            ss.SetContentsOfCell("c1", "howdy");
            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());

            Array.Sort(expected);
            Array.Sort(actual);
            CollectionAssert.AreEqual(expected, actual);
        }


        //-------------------------------------------------------------
        #endregion


        #region SetCellContent_formula
        //-------------------------------------------------------------

        [TestMethod()]
        public void scc_formula_1()
        {
            string[] expected = { "a1","b1" };
            string[] actual;
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "=1");
            ss.SetContentsOfCell("b1", "=a1 + 2");
            ss.SetContentsOfCell("a1", "=c1 +c1");
            ss.SetContentsOfCell("b1", "=x3");
            ss.SetContentsOfCell("c1", "");
            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());

            Array.Sort(expected);
            Array.Sort(actual);
            CollectionAssert.AreEqual(expected, actual);
        }



        //-------------------------------------------------------------
        #endregion


        #region getCellContents
        //-------------------------------------------------------------

        [TestMethod()]
        public void getCellContents_double()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("b1", "5");
            ss.SetContentsOfCell("a1", "2.1");
            ss.SetContentsOfCell("b1", "2.0");
            Assert.AreEqual(2.1, ss.GetCellContents("a1"));
            Assert.AreEqual(2.0, ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void getCellContents_string_1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "asdf");
            ss.SetContentsOfCell("b1", "kl9823woj");
            ss.SetContentsOfCell("a1", "");
            ss.SetContentsOfCell("b1", "blah");
            Assert.AreEqual("", ss.GetCellContents("a1"));
            Assert.AreEqual("blah", ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void getCellContents_string_2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("b1", "=a1+1");
            ss.SetContentsOfCell("a1", "yeah");
            ss.SetContentsOfCell("b1", "=a1");
            Assert.AreEqual("yeah", ss.GetCellContents("a1"));
            Assert.AreEqual("a1", ss.GetCellContents("b1").ToString());
            Assert.AreEqual("", ss.GetCellContents("c1"));
        }

        [TestMethod()]
        public void getCellContents_formula_1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("b1", "kl9823woj");
            ss.SetContentsOfCell("a1", "=b1*2");
            ss.SetContentsOfCell("b1", "=c1");
            Assert.AreEqual("b1*2", ss.GetCellContents("a1").ToString());
            Assert.AreEqual("c1", ss.GetCellContents("b1").ToString());
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void getCellContents_exception_1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "3543");
            ss.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void getCellContents_exception_2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "3543");
            ss.GetCellContents("a1.");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void getCellContents_exception_3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "skur");
            ss.GetCellContents(".a1");
        }



        //-------------------------------------------------------------
        #endregion


        #region replacing cells
        //-------------------------------------------------------------

        [TestMethod()]
        public void replace_double_with_string_1()
        {
            string[] expected = { "b1" };
            string[] actual;
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("b1", "2.04");
            ss.SetContentsOfCell("a1", "");
            ss.SetContentsOfCell("b1", "hi");
            ss.SetContentsOfCell("c1", "");
            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());

            Array.Sort(expected);
            Array.Sort(actual);
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual("hi", ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void replace_string_with_double_1()
        {
            string[] expected = { "b1" };
            string[] actual;
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "I'm a string");
            ss.SetContentsOfCell("b1", "Me too!");
            ss.SetContentsOfCell("a1", "");
            ss.SetContentsOfCell("b1", "3.4");
            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());

            Array.Sort(expected);
            Array.Sort(actual);
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual(3.4, ss.GetCellContents("b1"));
        }

        [TestMethod()]
        public void replace_double_with_formula_1()
        {
            string[] expected = {"a1", "b1" };
            string[] actual;
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("b1", "2.04");
            ss.SetContentsOfCell("a1", "=a2");
            ss.SetContentsOfCell("b1", "=a1");
            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());
            
            Array.Sort(expected);
            Array.Sort(actual);
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual("a2", ((Formula)ss.GetCellContents("a1")).ToString());
            Assert.AreEqual("a1", ((Formula)ss.GetCellContents("b1")).ToString());
        }

        [TestMethod()]
        public void replace_string_with_formula_1()
        {
            string[] expected = {"a1", "b1" };
            string[] actual;
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "blah blah blah");
            ss.SetContentsOfCell("b1", "");
            ss.SetContentsOfCell("a1", "=67-1+x3");
            ss.SetContentsOfCell("b1", "=a1*c1");
            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());

            Array.Sort(expected);
            Array.Sort(actual);
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual("67-1+x3", ss.GetCellContents("a1").ToString());
            Assert.AreEqual("a1*c1", ss.GetCellContents("b1").ToString());
        }


        //-------------------------------------------------------------
        #endregion


        #region dependencies
        //-------------------------------------------------------------


        [TestMethod()]
        public void dependencies_1()
        {
            string[] _Expected = { "u1","t1" };
            string[] _Actual;

            string[] AExpected = {"u1","a1","b1","t1"};
            string[] AActual;

            string[] BExpected = {"u1","b1","t1"};
            string[] BActual;
            
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("u1","=a1+b1");
            ss.SetContentsOfCell("a1","=x2+c1");
            ss.SetContentsOfCell("b1","=a1+c1");
            ss.SetContentsOfCell("t1","=u1+0");
            AActual = iSetToArray(ss.SetContentsOfCell("a1","=67-1+x3"));
            BActual = iSetToArray(ss.SetContentsOfCell("b1", "=a1*c1"));
            _Actual = iSetToArray(ss.SetContentsOfCell("u1", "=a1-b1"));
            
            
            Array.Sort(AExpected);
            Array.Sort(AActual);
            CollectionAssert.AreEqual(AExpected, AActual);

            Array.Sort(BExpected);
            Array.Sort(BActual);
            CollectionAssert.AreEqual(BExpected, BActual);

            Array.Sort(_Expected);
            Array.Sort(_Actual);
            CollectionAssert.AreEqual(_Expected, _Actual);
        }

        //-------------------------------------------------------------
        #endregion


        #region circular exceptions
        //-------------------------------------------------------------


        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void circularException_1()
        {
            string[] AActual;
            string[] BActual;

            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("u1", "=a1+b1");
            ss.SetContentsOfCell("a1", "=x2+c1");
            ss.SetContentsOfCell("b1", "=a1+c1");
            ss.SetContentsOfCell("t1", "=u1+0");
            AActual = iSetToArray(ss.SetContentsOfCell("a1","=67-1+x3"));
            BActual = iSetToArray(ss.SetContentsOfCell("b1", "=a1*c1"));
            //expect exception:
            ss.SetContentsOfCell("u1", "=a1-b1-t1");

        }

        [TestMethod()]
        public void circularException_2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "3.14159265358979");
            ss.SetContentsOfCell("b1", "=a1");
            ss.SetContentsOfCell("c1", "=b1");
            bool thrown = false;
            try
            {
                ss.SetContentsOfCell("a1", "=c1"); //should throw circular
            }
            catch (CircularException)
            {
                thrown = true;
            }

            Assert.AreEqual(3.14159265358979, ss.GetCellContents("a1"));
            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void circularException_3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "2");
            ss.SetContentsOfCell("b1", "=a1");
            ss.SetContentsOfCell("c1", "=b1");
            bool thrown = false;
            try
            {
                ss.SetContentsOfCell("a1", "=c1"); //should throw circular
            }
            catch (CircularException)
            {
                thrown = true;
            }

            Assert.AreEqual(2.0, ss.GetCellContents("a1"));
            Assert.IsTrue(thrown);
        }

        [TestMethod()]
        public void circularException_4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "xkcd");
            ss.SetContentsOfCell("b1", "=a1");
            ss.SetContentsOfCell("c1", "=b1");
            bool thrown = false;
            try
            {
                ss.SetContentsOfCell("a1", "=c1"); //should throw circular
            }
            catch (CircularException)
            {
                thrown = true;
            }

            Assert.AreEqual("xkcd", ss.GetCellContents("a1").ToString());
            Assert.IsTrue(thrown);
        }

        //-------------------------------------------------------------
        #endregion

        
        #region bad input tests
        //-------------------------------------------------------------

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("8");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("b3.");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_5()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_6()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "=a2");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_7()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("", "1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_8()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("b34.", "string me");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void badInput_9()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("b.4", "0.1");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void badInput_10()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", (string)null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void badInput_11()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", null);
        }

        //-------------------------------------------------------------
        #endregion


        #region PS5 Constructor 2
        //----------------------------------------------------------------------------
        [TestMethod()]
        public void constructor2()
        {
            string[] expected = { "A1", "B1"};
            string[] actual;

            Spreadsheet ss = new Spreadsheet(isValid_always, normalize_toUpper, "2.0");
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("A1", "=b1");
            ss.SetContentsOfCell("b1", "2");
            Assert.AreEqual("B1", ss.GetCellContents("a1").ToString());
            Assert.AreEqual("B1", ss.GetCellContents("A1").ToString());
            Assert.AreEqual(2.0, ss.GetCellContents("b1"));
            Assert.AreEqual(2.0, ss.GetCellContents("B1"));

            actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(expected, actual);
        }

        //----------------------------------------------------------------------------
        #endregion


        #region PS5 Constructor 3 (loading from file)
        //----------------------------------------------------------------------------

        [TestMethod()]
        public void con3_good_1()
        {
            Spreadsheet ss = new Spreadsheet(testFolderLocation+"good\\3.1_Good.txt", isValid_always, normalize_toUpper, "3.1_Good");

            string[] expected = { "A1","A2", "ZB23","P1","ZZ1"};
            string[] actual = iEnumToArray(ss.GetNamesOfAllNonemptyCells());
            CollectionAssert.AreEqual(expected, actual);

            Assert.AreEqual(5.6, ss.GetCellContents("a1"));
            Assert.AreEqual(5.6, ss.GetCellContents("A1"));
            Assert.AreEqual("lots of words", ss.GetCellContents("a2"));
            Assert.AreEqual("lots of words", ss.GetCellContents("A2"));
            Assert.AreEqual("A1+0.5", ss.GetCellContents("zb23").ToString());
            Assert.AreEqual("A1+0.5", ss.GetCellContents("zB23").ToString());
            Assert.AreEqual("1+(ZB23/1)*2+P0", ss.GetCellContents("zz1").ToString());
            Assert.AreEqual("1+(ZB23/1)*2+P0", ss.GetCellContents("ZZ1").ToString());
            Assert.AreEqual(6.1, ss.GetCellValue("Zb23"));
            Assert.IsInstanceOfType(ss.GetCellValue("zz1"), typeof(FormulaError));
            Assert.AreEqual("", ss.GetCellValue("noU1"));
        }

        //----------------------------------------------------------------------------
        #endregion


        #region get saved version
        //----------------------------------------------------------------------------
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod()]
        public void getSavedVersion_ss_misnamed()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion(testFolderLocation + "bad\\b_spreadsheet_misnamed.txt");
        }

        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        [TestMethod()]
        public void getSavedVersion_ss_verMissing()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetSavedVersion(testFolderLocation + "bad\\b_ss_att_0att.txt");
        }

        [TestMethod()]
        public void getSavedVersion_correct()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual("bad", ss.GetSavedVersion(testFolderLocation + "good\\good.txt"));
        }

        //----------------------------------------------------------------------------
        #endregion


        #region Error Tests
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Checks to make sure that all of the files in \testFiles\bad\ throw a
        /// SpreadsheetReadWriteException when loaded.
        /// </summary>
        [TestMethod()]
        public void testAllBadXmlFiles()
        {
            Spreadsheet ss = null;
            string[] badFiles = Directory.GetFiles(testFolderLocation + "bad");
            bool throws = false;
            foreach (string file in badFiles)
            {
                throws = false;
                try
                {
                    ss = new Spreadsheet(file, isValid_always, normalize_same, "bad");
                }
                catch (SpreadsheetReadWriteException)
                {
                    throws = true;
                }
                Assert.IsTrue(throws, "Loading \"" + file + "\" failed to throw an error");
            }
        }


        //-------------------------------------------------------------------------------
        #endregion


        #region Save tests
        //-------------------------------------------------------------------------------
        [TestMethod()]
        public void save_1()
        {
            Spreadsheet ss = new Spreadsheet(isValid_always, normalize_toUpper, "default");
            Assert.IsFalse(ss.Changed);
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("a2", "string me");
            ss.SetContentsOfCell("a3","=a1+1");
            Assert.IsTrue(ss.Changed);
            ss.Save("save1.txt");
            Assert.IsFalse(ss.Changed);

            //now try reloading the spreadsheet
            ss = new Spreadsheet("save1.txt", isValid_always, normalize_toUpper, "default");

        }


        [TestMethod()]
        public void save_2()
        {
            string file = testFolderLocation+"good\\save2.txt";
            Spreadsheet ss = new Spreadsheet(isValid_always, normalize_toUpper, "default");
            ss.SetContentsOfCell("a1", "1");
            ss.SetContentsOfCell("a2", "string me");
            ss.SetContentsOfCell("a3", "=a1+1");

            bool thrown = false;
            StreamWriter sw = new StreamWriter("dummy.txt");
            try
            {
                sw = new StreamWriter(file);
                ss.Save(file);
            }
            catch (SpreadsheetReadWriteException)
            {
                thrown = true;
            }
            finally
            {
                sw.Close();
            }

            Assert.IsTrue(thrown);
        }


        //-------------------------------------------------------------------------------
        #endregion


        #region Beta Test
        //-------------------------

        [TestMethod()]
        public void _betaTest()
        {
            //Spreadsheet ss = new Spreadsheet(t+"b_c_child_text.txt", (s) => true, (s) => s, "bad");
        }

        //---------------------------
        #endregion

        //*/
    }
}
