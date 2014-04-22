

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
        bool DISPLAY = true;


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

        /// <summary>
        /// Displays the string to the console if the DISPLAY boolean is set.
        /// </summary>
        /// <param name="s"></param>
        public void d(string s)
        {
            if(DISPLAY)
                Console.WriteLine(s);
        }

        #endregion

        /// <summary>
        /// Holds a list of all the spreadsheets available on the server.
        /// </summary>
        List<string> savedSpreadsheets = new List<string>(
            new string[] 
        { 
            "spreadsheetOne",
            "number2",
            "three",
            "taxes"
        });

        string[,] setOne = new string[,]
        {
            {"A1","1"},
            {"B1","2"},
            {"C1","3"},
            {"D1","4"},
            {"E1","5"}
        };

        string[,] setTwo = new string[,]
        {
            {"A1","= 1"},
            {"A2","= A1"},
            {"A3","= A1 + A2"},
            {"A4","= A3 + B1"},
            {"A5","word!"}
        };

        int version_number = 0;

        public void processMessage(NetworkStream clientStream, string received)
        {
            //describe what we got
            d("R:" + ToLiteral(received));

            if (received[received.Length - 1] != '\n')
            {
                d("*Err: received command did not end with \\n");
                return;
            }

            //Take off the \n at the end
            received = received.Substring(0, received.Length - 1);

            //ASCIIEncoding encoder = new ASCIIEncoding();
            string respond = "";
            string[] split = received.Split(ESC);

            //Look at what they sent us
            if (split[0] == "PASSWORD")
            {
                if (split[1] == "james")
                {
                    respond = "FILELIST";
                    foreach (string s in savedSpreadsheets)
                        respond += ESC + s;
                }
                else
                {
                    respond = "INVALID";
                }
            }
            else if (split[0] == "OPEN")
            {
                respond = "UPDATE" + ESC + version_number;

                foreach(string s in setTwo)
                    respond += ESC + s;
            }
            else if (split[0] == "CREATE")
            {
                respond = "UPDATE" + ESC + version_number;
            }
            else if (split[0] == "ENTER")
            {
                respond = "ERROR" + ESC + "Not implemented yet";
            }
            else if (split[0] == "RESYNC") 
            {
                respond = "ERROR" + ESC + "Not implemented yet";
            }
            else if (split[0] == "UNDO")
            {
                respond = "ERROR" + ESC + "Not implemented yet";
            }
            else if (split[0] == "SAVE")
            {
                respond = "ERROR" + ESC + "Not implemented yet";
            }
            else if (split[0] == "DISCONNECT")
            {
                respond = "ERROR" + ESC + "Not implemented yet";
            }
            else
            {
                respond = "ERROR"+ESC+"Unknown command: "+received; 
            }

            //Tack on the required ending
            respond += "\n";

            //send the message
            sendMessage(clientStream, respond);
        }

        private void sendMessage(NetworkStream clientStream, string message)
        {
            d(" :" + ToLiteral(message));
            //Relay response
            byte[] buffer = new ASCIIEncoding().GetBytes(message);
            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        /// <summary>
        /// Converts any escaped characters in the input into a seeable format like "\n"
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