#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <errno.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <pthread.h>

typedef struct
{
  int connectionfd;
  struct sockaddr_in addrclient;
} Client;

void * thread_handle_clients(void *arg)
{
  Client *client;
  client = (Client*)arg;
  int connectionfd = client->connectionfd;
  struct sockaddr_in addrclient = client->addrclient;

  while(1)
  {
      
    printf("Server: %lu thread: Receive the request from %s\n", pthread_self(), inet_ntoa(addrclient.sin_addr));
    char buf[1024];

    ssize_t rb = recv(connectionfd, buf, sizeof(buf), 0);

    if(-1 == rb)
    {
      perror("Server: thread receive failed");
      return NULL;
    }

    if(0 == rb)
    {
      printf("Server: %lu thread client is closed\n", pthread_self());
      break;
    }

    printf("Server: %lu thread send the response to client\n", pthread_self());

    if(-1 == send(connectionfd, buf, rb, 0))
    {
      perror("Server: thread send failed");
      return NULL;
    }
  }
  
  return NULL;
}

int main(int argc, char *argv[])
{
  if(argc < 2)
  {
    fprintf(stderr, "Usage: %s <Port>\n", argv[0]);
    return -1;
  }

  printf("Server: Creating server socket..\n");

  int serverSocketfd = socket(AF_INET, SOCK_STREAM, 0);
  if(-1 == serverSocketfd)
  {
    perror("Server: Server socket create failed");
    return -1;
  }

  printf("Server: Preparing IP address and bind...\n");

  struct sockaddr_in addr;
  addr.sin_family = AF_INET;
  addr.sin_port = htons(atoi(argv[1]));
  addr.sin_addr.s_addr = INADDR_ANY;

  if(-1 == bind(serverSocketfd, (struct sockaddr*)&addr, sizeof(addr)))
  {
    perror("Server: Server socket bind failed");
    return -1;
  }

  printf("Server: Start listening...\n");

  if(-1 == listen(serverSocketfd, 1024))
  {
    perror("Server: Server socket listen failed");
    return -1;
  }

  while(1)
  {
    printf("Server: Server is waiting for clients...\n");

    struct sockaddr_in addrclient = { 0 };
    socklen_t addrlength = sizeof(addrclient);

    int connectionfd = accept(serverSocketfd, (struct sockaddr*)&addrclient, &addrlength);

    if(-1 == connectionfd)
    {
      perror("Server: Server socket accept failed");
      return -1;
    }

    printf("Server: Received the requset from %s, %uClient is asking for connetion..\n", inet_ntoa(addrclient.sin_addr), ntohs(addrclient.sin_port));

    printf("Server: Creating thread to handle %uClient\n", ntohs(addrclient.sin_port));

    Client client;
    client.connectionfd = connectionfd;
    client.addrclient = addrclient;
    
    pthread_t tid;
    int error = pthread_create(&tid, NULL, thread_handle_clients, &(client));

    if(error)
    {
      perror("Server: pthread_create failed");
      return -1;
    }

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
