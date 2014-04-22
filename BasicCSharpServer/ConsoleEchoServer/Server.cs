

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

        /// <summary>
        /// Holds all of the cells (for every spreadsheet)
        /// </summary>
        Dictionary<string, string> cells = new Dictionary<string, string>();
        bool first = true;


        int version_number = 0;

        public void processMessage(NetworkStream clientStream, string received)
        {
            //Just so I don't have to go up to the constructor everytime I change something.
            if (first)
            {
                first = false;

                //add all the initial cells
                for (int i = 0; i < setTwo.GetLength(0); i++)
                    cells.Add(setTwo[i, 0], setTwo[i, 1]);

            }



            //describe what we got
            d("R:" + ToLiteral(received));

            if (received[received.Length - 1] != '\n')
            {
                d("*Err: the received command did not end with \\n");
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

                foreach (string n in cells.Keys)
                    respond += ESC + n + ESC + cells[n];

            }
            else if (split[0] == "CREATE")
            {
                //Eh, clear the cells
                cells = new Dictionary<string, string>();
                respond = "UPDATE" + ESC + version_number;
            }
            else if (split[0] == "ENTER")
            {
                int ver = 0;
                //Try to get the version number. Throw error if failed
                if (!int.TryParse(split[1], out ver))
                    throw new Exception("Received ENTER error: Cannot parse version number: \"" + split[1] + "\".");

                //If they have the wrong version, send them a sink.
                if (ver != version_number)
                {
                    respond = "SYNC" + ESC + version_number;
                    foreach (string n in cells.Keys)
                        respond += ESC + n + ESC + cells[n];
                }
                else //if they do have the right version, add the cell and broadcast
                {
                    if (cells.ContainsKey(split[2]))
                        cells[split[2]] = split[3];
                    else
                        cells.Add(split[2], split[3]);

                    version_number++;
                    respond = "UPDATE" + ESC + version_number + ESC + split[2] + ESC + split[3];
                }
            }
            else if (split[0] == "RESYNC")
            {
                respond = "SYNC" + ESC + version_number;
                foreach (string n in cells.Keys)
                    respond += ESC + n + ESC + cells[n];
            }
            else if (split[0] == "UNDO")
            {
                respond = "ERROR" + ESC + "Not implemented yet";
            }
            else if (split[0] == "SAVE")
            {
                int ver = 0;
                //Try to get the version number. Throw error if failed
                if (!int.TryParse(split[1], out ver))
                    throw new Exception("Received ENTER error: Cannot parse version number: \"" + split[1] + "\".");

                respond = "SAVED";
            }
            else if (split[0] == "DISCONNECT")
            {
                //Save the file
                //Disconnect them
                clientStream.Close();
                return;
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