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

        public int port
        {
            get;
            private set;
        }

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

        //*variables to be used
        private bool authenticated = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="whenDisconnected"></param>
        /// <param name="whenMessageIsReceived"></param>
        public ConnectionLiaison(Action<SocketConnection, Exception> whenDisconnected, Action<MessageReceivedFrom> whenMessageIsReceived):base(whenDisconnected,whenMessageIsReceived)
        {
            ESC = DEFAULT_ESC;
        }

        /// <summary>
        /// Attempts to connect to the specified server
        /// </summary>
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


        public void sendPassword(string pw)
        {
            this.SendMessage("PASSWORD" + ESC + pw, null); //TODO what if fail?
        }


        // ** TODO  Create more methods like the sendPassword that the Spreadsheet can use

    }
}
