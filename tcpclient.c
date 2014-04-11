#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>

int main(int argc, char *argv[])
{
  if(argc < 3)
  {
    fprintf(stderr, "Usage: %s <IP address> <Port>\n", argv[0]);
    return -1;
  }

  printf("Client: Creating Socket...\n");

  int socketfd = socket(AF_INET, SOCK_STREAM, 0);
  if(-1 == socketfd)
  {
    perror("Client: Socket create failed");
    return -1;
  }

  printf("Client: Preparing IP address and connection...\n");

  struct sockaddr_in addr;
  addr.sin_family = AF_INET;
  addr.sin_port = htons(atoi(argv[2]));
  addr.sin_addr.s_addr = inet_addr(argv[1]);

  if(-1 == connect(socketfd, (struct sockaddr*)&addr, sizeof(addr)))
  {
    perror("Client: Socket connection failed");
    return -1;
  }

  printf("Client: Sending request and receiving response..\n");

  while(1)
  {
    printf("Send: ");

    char buf[1024];
    gets(buf);

    //
    if(!strcmp(buf, "quit"))
      break;

    if(-1 == send(socketfd, buf, (strlen(buf) + 1) * sizeof(buf[0]), 0))
    {
      perror("Client: Sending failed");
      return -1;
    }

    ssize_t rb = recv(socketfd, buf, sizeof(buf), 0);

    if(-1 == rb)
    {
      perror("Client: Receiving failed");
      return -1;
    }

    if(0 == rb)
    {
      printf("Client: The server is closed!\n");
      break;
    }

    printf("Receive: %s\n", buf);
  }

  printf("Client: Socket is closing...\n");

  if(-1 == close(socketfd))
  {
    perror("Client: Socket close failed");
    return -1;
  }

  printf("Client: Socket closed successful!\n");

  return 0;
}
