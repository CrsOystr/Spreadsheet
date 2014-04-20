
#ifndef SPREAD_SHEET_H
#define SPREAD_SHEET_H

#include <string>
#include <list>
#include "depend_graph.h"

using namespace std;
using namespace depend;

namespace ss
{
  
  class spread_sheet
  {
  private:
    depend_graph ss_dg;
    list<string> ss_changes;

  public: 
    string ss_name;
    
    //constructor to start a new spreadsheet with a name
    spread_sheet(string name); 
    
    void save();
    void undo();
    void change(string change);

  };
}

    
#endif
