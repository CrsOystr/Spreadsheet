

using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;

namespace ConsoleEchoServer
{
    class Server
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        /// <summary>
        /// Respresents the escape character. Used to parse the communications we receive from
        /// a spreadsheet client.
        /// </summary>
        //*
        public const char ESC = '\u001B';
        /*/
        public const char ESC = '|';
        //*/

        #region Server Stuff
        public Server(int port)
        {
            this.tcpListener = new TcpListener(IPAddress.Any, port);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }



        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }


        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                string received = encoder.GetString(message, 0, bytesRead);
                System.Diagnostics.Debug.WriteLine(received);


                //Respond
                /*
                buffer = encoder.GetBytes("I see you!\n");

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                //*/

                processMessage(clientStream, received);

            }

            tcpClient.Close();
        }

        #endregion

        /// <summary>
        /// Holds a list of all the spreadsheet available on the server.
        /// (Currently holds empty defaults)
        /// </summary>
        List<string> spreadsheets = new List<string>(
            new string[] 
        { 
            "spreadsheetOne.ss",
            "number2.ss",
            "three.some",
            "four.doc"
        });


        public void processMessage(NetworkStream clientStream, string received)
        {
            //describe what we got
            Console.WriteLine("Received: "+ToLiteral(received));

            ASCIIEncoding encoder = new ASCIIEncoding();
            string respond = "";
            string[] split = received.Split(ESC);

            //Look at what they sent us
            if (split[0] == "PASSWORD")
            {
                if (split[1] == "james\n")
                {

                    respond = "FILELIST";
                    foreach (string s in spreadsheets)
                        respond += ESC + s;
                    respond += "\n";
                }
                else
                {
                    respond = "INVALID\n";
                }
            }
            else if (split[0] == "OPEN")
            {
                respond = "ERROR" + ESC + "Not implemented yet\n";
            }
            else if (split[0] == "CREATE")
            {
                respond = "ERROR" + ESC + "Not implemented yet\n";
            }
            else if (split[0] == "ENTER")
            {
                respond = "ERROR" + ESC + "Not implemented yet\n";
            }
            else if (split[0] == "RESYNC") //NOTE: may need a \n at the end
            {
                respond = "ERROR" + ESC + "Not implemented yet\n";
            }
            else if (split[0] == "UNDO")
            {
                respond = "ERROR" + ESC + "Not implemented yet\n";
            }
            else if (split[0] == "SAVE")
            {
                respond = "ERROR" + ESC + "Not implemented yet\n";
            }
            else if (split[0] == "DISCONNECT")
            {
                respond = "ERROR" + ESC + "Not implemented yet\n";
            }
            else
            {
                respond = "ERROR"+ESC+"Unknown command: "+received; //received should already have the \n at the end
            }




            //Relay response
            byte[] buffer = encoder.GetBytes(respond);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();


        }

        /// <summary>
        /// Borrowed from http://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

    }
}