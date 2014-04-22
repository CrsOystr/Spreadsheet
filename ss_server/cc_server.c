// Server.c -- Spring 2014. Written for CS 3505 at the University of Utah
// Intended to be used as a server for a spreadsheet program with multiple clients

#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <errno.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <pthread.h>
#include <string.h>
#include <dirent.h>
#include "spread_sheet.h"
#include <cstring>
#include <sys/stat.h>
#include <map>
#include <boost/regex.hpp>
#include <utility>


using namespace ss;

//struct for a client data type
//containing a connection ID
//and the client address
typedef struct
{
  int connectionfd;
  struct sockaddr_in addrclient;
} Client;

//
std::multimap<spread_sheet*, Client*> server_map;

std::string searchDir();

//Defines our escape character that will be used to delimit as per protocol
const char ESC = '\e';

//defining function for authentication
bool authentication(char buf[], int);

//handles all connected client requests
void * thread_handle_clients(void *arg)
{
  Client *client;
  client = (Client*)arg;
  int connectionfd = client->connectionfd;
  struct sockaddr_in addrclient = client->addrclient;
  int i,k;

  //while server is running listen for connections -- all of our commands will be handled here
  while(1)
  {
    //recieved connection request
    printf("Server: %lu thread: Receive the request from %s\n", pthread_self(), inet_ntoa(addrclient.sin_addr));
    
    //create buffer to store incoming data
    char buf[1024];
    char command[1024];
    char command_content[1024];
    char *tmp;
    //rb acts as a status code for the current connection
    ssize_t rb = recv(connectionfd, buf, sizeof(buf), 0);

    //didn't receive thread id rb is negative
    if(-1 == rb)
    {
      perror("Server: thread receive failed");
      return NULL;
    }

    //client disconnected if rb is 0
    if(0 == rb)
    {
      printf("Server: %lu thread client is closed\n", pthread_self());
      break;
    }

    //thread properly recieved send response
    printf("Server: %lu thread send the response to client\n", pthread_self());

    // std::cout << buf << '\n' <<std::endl;
    
    memset(command, '\0', sizeof(char)*1024);
    memset(command_content, '\0', sizeof(char)*1024);

    
    for(i = 0; buf[i] != '\e'; i++)
      {
	command[i] = buf[i];
	k = i;
      }
    for(;buf[i] != '\n';i++)
      {
	command_content[i-k] = buf[i];
      }

    std::string command_string(command);
    std::string command_content_string(command_content);

    std::cout << command_string <<std::endl;
    //WHERE WE START DEALING WITH OUR COMMANDS
    if(command_string == "PASSWORD")
    {
      printf("RECIEVED PASSWORD COMMAND\n");

      if(authentication(buf,k))
      {	
	printf("authenticated\n");

	std::string msg;
	msg += "FILELIST";
	msg += ESC;

	msg += searchDir();
	
	msg += "\n\0";
	
	const char * var;

	var = msg.c_str();


	//send message and print out status
	char str[256];
	sprintf(str,"%d",send(connectionfd, var, strlen(var),0));
	printf(str);
	printf("   send return\n");
      }
      else
      {
	printf("authen failed");
	
	buf[0] = 'I';
	buf[1] = 'N';
	buf[2] = 'V';
	buf[3] = 'A';
	buf[4] = 'L';
	buf[5] = 'I';
	buf[6] = 'D';
	buf[7] = '\n';
	buf[8] = '\0';
	
	if(-1 == send(connectionfd, buf, strlen(buf), 0))
	{
	  perror("Server: thread send failed authen failed");
	  return NULL;
	}
       }
    }
    else if(command_string == "OPEN")
    {
      printf("RECIEVED OPEN COMMAND\n");  
      



      std::string name = command_content_string.substr(6,command_content_string.find_first_of('\n'));
      std::cout << name <<std::endl;      

    }
    else if(command_string == "CREATE")
    {
      printf("RECIEVED CREATE COMMAND\n"); 
      std::string name = command_content_string.substr(0, command_content_string.find_first_of(ESC));
     
      spread_sheet* ss = new spread_sheet("true",false);
      server_map.insert(std::pair<spread_sheet*, Client*>(ss, client));
      

      std::cout << name << std::endl;
    }
    else if(command_string == "ENTER")
    {
      printf("RECIEVED ENTER COMMAND\n");
      std::string Message;
      std::string cell_name = command_content_string.substr(0, command_content_string.find_first_of("\e"));
      std::string cell_content = command_content_string.substr(command_content_string.find_first_of("\e") + 1, command_content_string.find_last_of("\e") - command_content_string.find_first_of("\e") - 1);
      std::string spreadsheet_name = command_content_string.substr(command_content_string.find_last_of("\e") + 1);
      int successful = 0;
      const char * msg;
	  
      for(std::multimap<spread_sheet*, Client*>::iterator it = server_map.begin(); it != server_map.end(); it++)
	{
	  if(it->first->get_name() == spreadsheet_name && it->second->connectionfd == connectionfd)
	    {
	      if(it->first->change(cell_name, cell_content))
		{
		  Message = "UPDATE\e" + cell_name + "\e" + cell_content + "\n";
			successful = 1;
			break;
		}
	    }
	}
      
      if(successful)
	{
	  for(std::multimap<spread_sheet*, Client*>::iterator it = server_map.begin(); it != server_map.end(); it++)
	    {
	      if(it->first->get_name() == spreadsheet_name)
		{
		  send(it->second->connectionfd, msg , strlen(msg), 0);
		}
	    }
	  }
      else
	{
	  // error
	}
      
    }

    //UNDO COMMANDS
    else if(command_string == "UNDO")
    {
      printf("RECIEVED UNDO COMMAND\n");
	  
	  std::string Message = "";
	  std::pair<std::string, std::string> cell = make_pair((std::string)NULL, (std::string)NULL);
	  
	  int successful = 0;
	  const char * var;
	  for(std::multimap<spread_sheet*, Client*>::iterator it = server_map.begin(); it != server_map.end(); it++)
	  {
		if(it->first->get_name() == command_content_string && it->second->connectionfd == connectionfd)
		{
		  cell = it->first->undo();
		  if(cell.first != (std::string)NULL && cell.second != (std::string)NULL)
		  {
			Message = "UPDATE\e" + cell.first + "\e" + cell.second + "\n";
			successful = 1;
			break;
		  }
		}
	  }
	  
	  // Sending the UPDATE message to all the clients
	  if(successful)
	  {
	    var = Message.c_str();
	    
	    for(std::multimap<spread_sheet*, Client*>::iterator it = server_map.begin(); it != server_map.end(); it++)
		  {
			if(it->first->get_name() == command_content_string)
			{
			  send(it->second->connectionfd, var , strlen(var), 0);
			}
		  }
	  }
	  
    }
    else if(command_string == "SAVE")
    {
      printf("RECIEVED SAVE COMMAND\n");
	  
	  for(std::multimap<spread_sheet*, Client*>::iterator it = server_map.begin(); it != server_map.end(); it++)
	  {
		if(it->first->get_name() == command_content_string && it->second->connectionfd == connectionfd)
		{
		  it->first->save();
		  break;
		}
	  }
    }
    else if(command_string == "DISCONNECT")
    {
      printf("RECIEVED DISCONNECT COMMAND\n");

	  // Disconnect the client with the server
	  if (-1 == close (connectionfd))
	  {
		perror ("close");
	  }
    }
    else
    {
      printf("COMMAND RECIEVED IS NOT VALID\n");
      std::cout <<command_string<<std::endl;
    }
	
  }
  return NULL;
}



//AUTH function finds if the password is valid for the server and if itis returns true
bool authentication(char buf[], int escseq)
{
 //variables for file open
  char const* const fileName = "passwords.txt";
  FILE* file;
  char line[256];
  char pass[256];
  bool authenticated = true;
  int i;

  file = fopen(fileName, "r");

  while(fgets(line, sizeof(line),file))
    {
      for(i = 0; line[i] != '\n'; i++)
	{
	  // printf("line at %i: %c\n ",i,line[i]);
	  //printf("buf at  %i: %c\n ",i, buf[escseq+2+i]);
	  if(line[i] != buf[escseq+2+i])
	    {
	      authenticated = false;
	    }
	}
    }
  return authenticated;
}

//searches directory for spreadsheet files
std::string searchDir()
{
  //pointer for current path
  char *currentPath;

  //get current working directory
  if((currentPath = getcwd(NULL, 0)) == NULL)  
    {  
      perror("getcwd error");  
    }  
 
  std::string msg;
  DIR* dp = opendir (currentPath);
  if (! dp) {
    perror ("opendir");
   }

  if (chdir (currentPath) == -1) {
    perror ("chdir");
   }

  errno = 0;
  struct dirent* de;

  for (de = readdir (dp); de; de = readdir (dp))
    {
      int size = 0;
      while(de->d_name[size] != 0)
	{
	  size++;
	}
      //printf("%d\n", size);
      if(de->d_name[size - 4] != '.' && de->d_name[size - 3] == '.' &&  de->d_name[size - 2] == 's' && de->d_name[size - 1] == 's' )
      {
	msg+= de->d_name;
	msg+= ESC;
      }
    }
   
  for(int i = 0; i < msg.length(); i++)
    {
      if(msg[i] == '.' && msg[i+1] == 's' && msg[i+2] == 's')
	{
	  //printf("%c%c%c  at %i,%i,%i\n",msg[i],msg[i+1],msg[i+2],i,i+1,i+2);
	  msg.erase(i,3);
	}
      for(int o = 0; o < msg.length(); o++)
	{
	  if(msg[o] == ESC)
	    {
	      // std::cout << '\n';
	    }
	  else
	    {
	      //  std::cout << msg[o];
	    }
	}
      
      //printf("\n");
    }

  if (errno) {
    perror ("readdir");
   }
  
  free(currentPath);
  closedir (dp);
  
  return msg;
}




//MAIN ENTRY POINT FOR PROGRAM. 
int main(int argc, char *argv[])
{
  //need port from command line
  if(argc < 2)
  {
    fprintf(stderr, "Usage: %s <Port>\n", argv[0]);
    return -1;
  }

  printf("Server: Creating server socket..\n");

  //open a socket in the internet namespace(AF_INET) with the connection style of streaming bytes
  //serverSocketfd serves as an error code when negative and file desciptor otherwise
  int serverSocketfd = socket(AF_INET, SOCK_STREAM, 0);

  //error
  if(-1 == serverSocketfd)
  {
    perror("Server: Server socket create failed");
    return -1;
  }

  printf("Server: Preparing IP address and bind...\n");

  //prepare socket for individual connections
  struct sockaddr_in addr;
  //internet socket
  addr.sin_family = AF_INET;
  //assign port number form command line
  addr.sin_port = htons(atoi(argv[1]));
  //accept any interface
  addr.sin_addr.s_addr = INADDR_ANY;

  //assigns local address to serverSocketfd
  //return -1 on error
  if(-1 == bind(serverSocketfd, (struct sockaddr*)&addr, sizeof(addr)))
  {
    perror("Server: Server socket bind failed");
    return -1;
  }

  printf("Server: Start listening...\n");

  //listens for connections over serverSocketfd with a message queue of size 1024
  if(-1 == listen(serverSocketfd, 1024))
  {
    perror("Server: Server socket listen failed");
    return -1;
  }

  //while server on
  while(1)
  {
    printf("Server: Server is waiting for clients...\n");

    //null socket creation for next client connection
    struct sockaddr_in addrclient = { 0 };
    //length of socket
    socklen_t addrlength = sizeof(addrclient);

    //deques serverSocket's list of connections, creates ne connection ID and return connection ID
    //connectionfd acts as the file descriptor for one client to server connection
    int connectionfd = accept(serverSocketfd, (struct sockaddr*)&addrclient, &addrlength);
    
    //server couldn't accept socket
    if(-1 == connectionfd)
    {
      perror("Server: Server socket accept failed");
      return -1;
    }

    printf("Server: Received the requset from %s, %uClient is asking for connetion..\n", inet_ntoa(addrclient.sin_addr), ntohs(addrclient.sin_port));

    printf("Server: Creating thread to handle %uClient\n", ntohs(addrclient.sin_port));

    //create client with connection id and clients address
    Client client;
    client.connectionfd = connectionfd;
    client.addrclient = addrclient;
    
    //create a new pthread to handle multiple clients
    pthread_t tid;
    int error = pthread_create(&tid, NULL, thread_handle_clients, &(client));

    //detect threading error
    if(error)
    {
      perror("Server: pthread_create failed");
      return -1;
    }
    //failed to detatch client from server
    if(0 != (error = pthread_detach(tid)))
    {
      perror("Server: pthread_detach failed");
      return -1;
    }
  }

  printf("Server: Server is closing...\n");

  if(-1 == close(serverSocketfd))
  {
    perror("Server: Server socket close failed");
    return -1;
  }

  printf("Server: Server close sucessful!\n");

  return 0;
}
