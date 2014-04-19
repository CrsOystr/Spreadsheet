using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SS
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectionLiaison
    {
        /// <summary>
        /// Address of the server to connect to
        /// </summary>
        private string server;

        /// <summary>
        /// Gets called when there was a connection but lost it.
        /// </summary>
        public Action<string> onDisconnect;

        /// <summary>
        /// Describes if this has a connection to a server.
        /// </summary>
        public bool Connected
        {
            get;
            private set;
        }



        /// <summary>
        /// Sets up a ConnectionLiaison. Does not automatically connect upon initialization.
        /// </summary>
        /// <param name="server">Address of the server this connection will be associated with.</param>
        /// <param name="onDisconnect">This will be called when there was a connection but we lost it.</param>
        public ConnectionLiaison(string server, Action<string> onDisconnect)
        {
            this.server = server;
            this.onDisconnect = onDisconnect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="successfulConnect">Called if we successfully connect to the server</param>
        /// <param name="failedToConnect">Called if unable to connect to server at all.</param>
        public void tryToConnect(Action<string> successfulConnect, Action<string> failedToConnect)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Thread.Sleep(1000);
                successfulConnect("Connected!");
            });

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string echo(string s)
        {
            return s.ToUpper();
        }
    }
}
