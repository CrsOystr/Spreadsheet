//Tests for spreadsheet 

#include <iostream>
#include <string>
#include "spread_sheet.h"

using namespace std;
using namespace ss;

int main()
{
  cout << "Testing program for Spreadsheet" << endl;

  {
    cout << "TEST ONE" << endl;
    spread_sheet name("goog");
    name.change("B4", "= B5 + 6");
    name.change("B4", " B5 + 6");
    name.change("B5", "= B4");


  }

}
