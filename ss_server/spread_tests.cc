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
    /*
    spread_sheet name("Funyons", false);
    name.change("B4", "= B5 + 6");
    name.change("B4", " B5 + 6");
    //name.change("B5", "= B4");
    name.change("B43", "= B5 + 6");
    name.change("B45", "= B5 + 6");
    name.change("B45", "duh");

    name.undo();
    name.save();
    */
    spread_sheet name1("Funyons", true);
    name1.change("H8", "=45");
    name1.change("H9", "=45");
    name1.undo();
    name1.save();




  }

}
