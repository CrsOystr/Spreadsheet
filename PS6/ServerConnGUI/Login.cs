using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerConnGUI
{
    public partial class Login : Form
    {
        //variables to be used
        private bool authenticated = false;
        private string PW = "";
        private string IP = "";
      
        /// <summary>
        /// Login GUI start point
        /// </summary>
        public Login()
        {
        
            InitializeComponent();

            //create keypress handlers for each text box
            this.IP_textbox.KeyPress += new KeyPressEventHandler(waitForEnter);
            this.PW_textbox.KeyPress += new KeyPressEventHandler(waitForEnter);

        }

        /// <summary>
        /// Wait for user to enter username and password
        /// </summary>
        /// <param name="sender">object event is coming from</param>
        /// <param name="e"></param>
        private void waitForEnter(object sender, KeyPressEventArgs e)
        {
            //if keycode is enter
           if(e.KeyChar == (char)13)
           {
               //if the IP isn't entered error box
               if(IP_textbox.Text.Length < 1)
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
                   PW = PW_textbox.Text;
                   IP = IP_textbox.Text;
                    
                   //hide login window
                   this.Hide();
                   //open new spreadsheet
                   new SpreadsheetGUI.SpreadsheetGUIForm().Show();
               }
           }
        }
    }
}
