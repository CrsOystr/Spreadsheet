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
#include <string>


//this wouldnt compile i dunno if we need it -Nic
//typedef enum { false, true } bool;


//struct for a client data type
//containing a connection ID
//and the client address
typedef struct
{
  int connectionfd;
  struct sockaddr_in addrclient;
} Client;

//Defines our escape character that will be used to delimit as per protocol
const char ESC = '\e';

//defining function for authentication
bool authentication(char buf[], int);

//function for getting what files are available to user
const char* get_file_list();


/*const char* get_file_list()
{
 
  //ret = malloc(256*sizeof(char));

  ret = &temp[0];
  printf(ret);
  printf("BEFORE RETURN\n");
  return ret;
  }*/



//handles all connected client requests
void * thread_handle_clients(void *arg)
{
  Client *client;
  client = (Client*)arg;
  int connectionfd = client->connectionfd;
  struct sockaddr_in addrclient = client->addrclient;
  int i,j,k;

  

  //while server is running listen for connections -- all of our commands will be handled here
  while(1)
  {
    //recieved connection request
    printf("Server: %lu thread: Receive the request from %s\n", pthread_self(), inet_ntoa(addrclient.sin_addr));
    
    //create buffer to store incoming data
    char buf[1024];
    char command[1024];
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
    //printf("Server: %s", buf);
    
    for(i = 0; buf[i] != '\e'; i++)
      {
	command[i] = buf[i];
	k = i;
      }
    
    std::string command_string(command);
    if(command_string == "PASSWORD")
      {
	printf("RECIEVED PASSWORD COMMAND");
      }
    else if(command_string == "OPEN")
      {
	printf("RECIEVED OPEN COMMAND");
      }
    else if(command_string == "CREATE")
      {
	printf("RECIEVED CREATE COMMAND");
      }
    else if(command_string == "ENTER")
      {
	printf("RECIEVED ENTER COMMAND");
      }
    else if(command_string == "UNDO")
      {
	printf("RECIEVED UNDO COMMAND");
      }
    else if(command_string == "SAVE")
      {
	printf("RECIEVED SAVE COMMAND");
      }
    else if(command_string == "DISCONNECT")
      {
	printf("RECIEVED DISCONNECT COMMAND");
      }
    else
      {
	printf("COMMAND RECIEVED IS NOT VALID");
      }






    











    bool authen = authentication(buf,k);
    
    if(authen)
      {	
	printf("authenticated\n");


	//////
	//
	//Variables
	//
	//////
	
	char const* const fileName = "filelist.txt";
	FILE* file;
	char line[256];
	char temp[256];
	char array[256];
	const char* ret;
	int i;
	int last_endline = 0;
	int current_line = 0;
	

	//populate with filelist command and ESC	  
	temp[0] = 'F';
	temp[1] = 'I';
	temp[2] = 'L';
	temp[3] = 'E';
	temp[4] = 'L';
	temp[5] = 'I';
	temp[6] = 'S';
	temp[7] = 'T';
	temp[8] = 27;

	

	//open filelist.txt which holds a list of all the spreadsheets
	file = fopen(fileName, "r");
	
	//while there are more lines
	while(fgets(line, sizeof(line), file))
	  {
	    //increment current line variable
	    current_line++;
	    //step down current line until new line
	    for(i = 0; line[i] != '\n'; i++)
	      {
		//set temp[8(allow for command)+current_line(for adding in ESC)+last_endline(how far into the array we are)+i]
		temp[8+current_line+last_endline+i] = line[i];
	      }
	    if(line[i] == '\n')
	      {
		//ass to last_endline when endline
		last_endline += i;
		//add in ESC char
		temp[8+current_line+last_endline+i] = 27 ;
	      }
	    
	  }	

	//pointer to temp
	tmp = &temp[0];

	printf(tmp);
	printf("\n");

	//populate buffer with new string
	for(i = 0; i < strlen(temp); i++)
	  {
	    buf[i] = temp[i];
	    // printf("buf[%i] = %c , temp[%i] = %c\n",i,buf[i],i,temp[i]);
	  }
	
	//send message and print out status
	char str[256];
	sprintf(str,"%d",send(connectionfd, buf, strlen(buf),0));
	printf(str);
	printf("   send return\n");
	/*if(-1 == send(connectionfd, y, strlen(y), 0))
	  {
	    perror("Server: thread send failed authenicated");
	    return NULL;
	    }*/
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
	
	if(-1 == send(connectionfd, buf, rb, 0))
	  {
	    perror("Server: thread send failed authen failed");
	    return NULL;
	  }
       }
	      
 
    /*if(-1 == send(connectionfd,temp, rb, 0))
      {
	perror("Server: thread send failed");
	return NULL;
	}*/

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
  int i, j;

  file = fopen(fileName, "r");

  while(fgets(line, sizeof(line),file))
    {
      for(i = 0; line[i] != '\n'; i++)
	{
	  printf("line at %i: %c\n ",i,line[i]);
	  printf("buf at  %i: %c\n ",i, buf[escseq+2+i]);
	  if(line[i] != buf[escseq+2+i])
	    {
	      authenticated = false;
	    }
	}
    }
  return authenticated;
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
