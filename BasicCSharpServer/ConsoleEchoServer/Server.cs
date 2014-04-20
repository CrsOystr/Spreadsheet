

using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections.Generic;

namespace ConsoleEchoServer
{
  class Server
  {
    private TcpListener tcpListener;
    private Thread listenThread;
    public const char ESC = '\u001B';

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

            respond(clientStream, received);

        }

        tcpClient.Close();
    }

#endregion


    List<string> spreadsheets = new List<string>(new string[] 
    { 
        "spreadsheetOne.ss",
        "number2.ss",
        "three.some",
        "four.doc"
    });

    public void respond(NetworkStream clientStream, string received)
    {
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
            respond = "ERROR"+ESC+"Not implemented yet\n";
        }
        else if (split[0] == "CREATE")
        {
            respond = "ERROR" + ESC + "Not implemented yet\n";
        }
        else if (split[0] == "ENTER")
        {
            respond = "ERROR" + ESC + "Not implemented yet\n";
        }
        else if (split[0] == "RESYNC")
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
            respond = "Unknown command: " + received ; //received should already have the \n at the end
        }




        //Relay response
        byte[] buffer = encoder.GetBytes(respond);

        clientStream.Write(buffer, 0, buffer.Length);
        clientStream.Flush();


    }


  }
}