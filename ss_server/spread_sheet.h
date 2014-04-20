
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
    int ss_version;
    depend_graph ss_dg;
    list<pair<string, string> > ss_changes;
    
    

  public: 
    string ss_name;
    
    //constructor to start a new spreadsheet with a name
    spread_sheet(string name); 
    
    void save();
    void undo();

    //returns true if valid change, false if not
    bool change(string cell, string cell_content); 

  };
}

    
#endif
