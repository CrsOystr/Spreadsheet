using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CustomNetworking;

namespace SS
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionLiaison : SocketConnection
    {
        /// <summary>
        /// Address of the server to connect to
        /// </summary>
        public string hostname
        {
            get;
            private set;
        }

        /// <summary>
        /// Describes the port this connection is using/will use
        /// </summary>
        public int port
        {
            get;
            private set;
        }

        /// <summary>
        /// Describes the default port this class will use, unless otherwise specified
        /// </summary>
        public const int DEFAULT_PORT = 2500;

        /// <summary>
        /// Represents the delimiter used for parsing communications between the Spreadsheet 
        /// client and server. This value can adapt if the server is using a different separation character (singular)
        /// </summary>
        public char ESC;

        /// <summary>
        /// The default value for the escape sequence
        /// </summary>
        public const char DEFAULT_ESC = '\u001B';

        /// <summary>
        /// Method to call when sending something fails
        /// </summary>
        public Action<Exception, Object> callBack;

        /// <summary>
        /// Basic constructor for a connection liaison. This will not start a connection.
        /// </summary>
        /// <param name="whenDisconnected"></param>
        /// <param name="whenMessageIsReceived"></param>
        /// <param name="whenSendingFails"></param>
        public ConnectionLiaison(Action<SocketConnection, Exception> whenDisconnected, Action<MessageReceivedFrom> whenMessageIsReceived, Action<Exception,Object> whenSendingFails):base(whenDisconnected,whenMessageIsReceived)
        {
            this.callBack = whenSendingFails;
            ESC = DEFAULT_ESC;
        }

        /// <summary>
        /// Attempts to connect to the specified server
        /// </summary>
        /// <param name="server">the server address (hostname+":"+port) to which you want to connect</param>
        /// <param name="successfulConnect">Called if we successfully connect to the server</param>
        /// <param name="failedToConnect">Called if unable to connect to server at all.</param>
        public void tryToConnect(string server, Action successfulConnect, Action<string> failedToConnect)
        {
            //* check if we currently have a connection
            if (this.isConnected())
                failedToConnect("Already have an active connection");


            //Figure out the host name and port (if any specified)
            string[] split = server.Split(':');

            this.hostname = split[0]; //assume the first part is the host name
            //If there was not a single split or if the second half did not parsed correctly
            int toTry = 0;
            if (!(split.Length == 2 && int.TryParse(split[1], out toTry)))
            {
                //Use the default port
                this.port = DEFAULT_PORT;
            }
            else
                this.port = toTry;


            //tries to create a TCP Connection
            this.TCPConnect(hostname, port, 2, successfulConnect, failedToConnect);
        }

        /// <summary>
        /// used to send the password
        /// </summary>
        /// <param name="pw"></param>
        public void sendPassword(string pw)
        {
             //the zeros are to enable using this same buffer to send back messages of longer length to avoid formatting issues
            this.SendMessage("PASSWORD" + ESC + pw + "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\n" , callBack);
        }

        /// <summary>
        /// Used to request a spreadsheet be opened
        /// </summary>
        /// <param name="spreadsheetName"></param>
        public void sendOpen(string spreadsheetName)
        {
            this.SendMessage("OPEN" + ESC + spreadsheetName, callBack);
        }

        /// <summary>
        /// used to request a spreadsheet be created and opened
        /// </summary>
        /// <param name="spreadsheetname"></param>
        public void sendCreate(string spreadsheetname)
        {
            this.SendMessage("CREATE" + ESC + spreadsheetname, callBack);
        }

        /// <summary>
        /// used to request a change to the official spreadsheet
        /// </summary>
        /// <param name="versionNumber"></param>
        /// <param name="cellName"></param>
        /// <param name="cellContent"></param>
        public void sendEnter(int versionNumber, string cellName, string cellContent)
        {
            this.SendMessage("CREATE" + ESC + versionNumber + ESC + cellName + ESC + cellContent, callBack);
        }

        /// <summary>
        /// Used to ask for the entire spreadsheet again
        /// </summary>
        public void sendResync()
        {
            this.SendMessage("RESYNC", callBack);
        }

        /// <summary>
        /// used to request an undo on the spreadsheet
        /// </summary>
        /// <param name="versionNumber"></param>
        public void sendUndo(int versionNumber)
        {
            this.SendMessage("UNDO" + ESC + versionNumber, callBack);
        }

        /// <summary>
        /// Used to request the official spreadsheet be saved
        /// </summary>
        /// <param name="versionNumber"></param>
        public void sendSave(int versionNumber)
        {
            this.SendMessage("SAVE" + ESC + versionNumber, callBack);
        }

        /// <summary>
        /// used to request that the server disconnect. (?)
        /// </summary>
        public void sendDisconnect()
        {
            this.SendMessage("DISCONNECT", callBack);
        }


        

        // ** TODO  Create more methods like the sendPassword that the Spreadsheet can use

    }
}
