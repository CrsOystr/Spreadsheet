#include <stdio.h>
#include <string.h>

typedef enum { false, true } bool;

bool authentication(char input[])
{
	// variables for file open
	char const* const fileName = "passwords.txt";
	FILE* file;
	char line[2048];
	bool authenticated = false;
	
	if(NULL == (file = fopen(fileName, "r")))
	{
		perror("file open failed");
	}
	else
	{
		while(!feof(file))
		{
			fgets(line, 2048, file);
			int len = strlen(line);
			if('\n' == line[len - 1])
				line[len - 1] = '\0';
				
			if(0 == strcmp(line, input))
			{
				authenticated = true;
				break;
			}
		}
		
		fclose(file);
		file = NULL;
	}
	
	return authenticated;
}

int main(int argc, char *argv[])
{
	bool authenticated = authentication("123456");
	
	printf("%d", authenticated);
	
	return 0;
}