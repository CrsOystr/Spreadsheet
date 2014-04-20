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
        private string hostname;

        private int port;
        const int DEFAULT_PORT = 2500;
        const char ESC = '\u001B';

        /// <summary>
        /// Gets called when there was a connection but lost it.
        /// </summary>
        public Action<string> onDisconnect;


        //*variables to be used
        private bool authenticated = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="whenDisconnected"></param>
        /// <param name="whenMessageIsReceived"></param>
        public ConnectionLiaison(Action<SocketConnection, Exception> whenDisconnected, Action<MessageReceivedFrom> whenMessageIsReceived):base(whenDisconnected,whenMessageIsReceived)
        {


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="successfulConnect">Called if we successfully connect to the server</param>
        /// <param name="failedToConnect">Called if unable to connect to server at all.</param>
        public void tryToConnect(string server, string password, Action successfulConnect, Action<string> failedToConnect)
        {
            //* check if we currently have a connection
            if (this.isConnected())
                failedToConnect("Already have an active connection");



            //Figure out if they specified a port
            string[] split = server.Split(':');

            this.hostname = split[0]; //assume the first part is the host name
            //If there was not a single split or if the second half did not parsed correctly
            if (!(split.Length == 2 && int.TryParse(split[1], out this.port)))
            {
                //Use the default port
                this.port = DEFAULT_PORT;
            }


            //Shoots off an attempt
            this.TCPConnect(hostname, port, 2, successfulConnect, failedToConnect);



            /* Test code
            TcpClient client = null;
            if (socket == null)
            {
                try
                {
                    client = new TcpClient(hostname, 2500);
                    Connected = true;
                }
                catch
                {
                    Connected = false;
                }
                if (Connected)
                {
                    socket = new StringSocket(client.Client, UTF8Encoding.Default);
                    socket.BeginReceive(LineReceived, null);
                    socket.BeginSend("PASSWORD\u001B"+password+"\n", (e, p) => { }, null);
                }
            }
            //*/

            //successfulConnect("Connected!");
        }

        public void sendPassword(string pw)
        {
            this.SendMessage("PASSWORD" + ESC + pw, null); //TODO what if fail?

        }



    }
}
