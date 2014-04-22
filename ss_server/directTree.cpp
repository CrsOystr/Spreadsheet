/*
#include <iostream>
#include <cstring>

using namespace std;

int main()
{
	// string str = "cell_name\ecell_content\espreadsheet_name";
	
	// cout << str.substr(0, str.find_first_of("\e")) << endl;
	// cout << str.substr(str.find_first_of("\e") + 1, str.find_last_of("\e") - str.find_first_of("\e") - 1) << endl;
	// cout << str.substr(str.find_last_of("\e") + 1) << endl;
	
	
}
*/

#include <stdio.h>
#include <cstring>
#include <unistd.h>
#include <dirent.h>
#include <errno.h>
#include <limits.h>
#include <stdlib.h>

int main (int argc, char* argv[]) {

  char *currentPath;

  if((currentPath = getcwd(NULL, 0)) == NULL)  
    {  
      perror("getcwd error");  
    }  
  else  
    {  
      printf("%s\n", currentPath);
    }  
	
  DIR* dp = opendir (currentPath);
  if (! dp) {
    perror ("opendir");
    return -1;
  }

  if (chdir (currentPath) == -1) {
    perror ("chdir");
    return -1;
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
	  printf("%s\n", de->d_name);
      }
    }

  if (errno) {
    perror ("readdir");
    return -1;
  }

  free(currentPath);
  currentPath = NULL;
  closedir (dp);
	
  return 0;
}
