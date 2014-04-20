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

#define pass_command = {'P','A','S','S','W','O','R','D'};

//struct for a client data type
//containing a connection ID
//and the client address
typedef struct
{
  int connectionfd;
  struct sockaddr_in addrclient;
} Client;

typedef enum { false, true } bool;

bool authentication(char buf[], int);

//handles all connected client requests
void * thread_handle_clients(void *arg)
{
  Client *client;
  client = (Client*)arg;
  int connectionfd = client->connectionfd;
  struct sockaddr_in addrclient = client->addrclient;
  int i,j,k;

  

  //while server is running listen for connections
  while(1)
  {
    //recieved connection request
    printf("Server: %lu thread: Receive the request from %s\n", pthread_self(), inet_ntoa(addrclient.sin_addr));
    
    //create buffer to store incoming data
    char buf[1024];
    char command[1024];
    char temp[1024];
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
	//command[i] = buf[i];
	k = i;
      }

    bool authen = authentication(buf,k);
    
    if(authen)
      {	
	printf("authenticated\n");
	
	buf[0] = 'P';
	buf[1] = 'a';
	buf[2] = 's';
	buf[3] = 's';
	buf[4] = 'w';
	buf[5] = 'o';
	buf[6] = 'r';
	buf[7] = 's';
	buf[8] = ' ';
	buf[9] = 'A';
	buf[10] = 'c';
	buf[11] = 'c';
	buf[12] = 'e';
	buf[13] = 'p';
	buf[14] = 't';
	buf[15] = 'e';
	buf[16] = 'd';
	buf[17] = '\n';

	
	if(-1 == send(connectionfd, buf, rb, 0))
	  {
	    perror("Server: thread send failed authenicated");
	    return NULL;
	  }
      }
    else
      {
	printf("authen failed");

	if(-1 == send(connectionfd, temp, rb, 0))
	  {
	    perror("Server: thread send failed authen failed");
	    return NULL;
	  }
       }
	      
  
    
    if(-1 == send(connectionfd,temp, rb, 0))
      {
	perror("Server: thread send failed");
	return NULL;
      }

  }
  
  return NULL;
}

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
	      return authenticated;
	    }
	}
    }
  
  return authenticated;
      
}


//Entry point for program
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

