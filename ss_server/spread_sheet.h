// spread_sheet.h -- Header file for spread-sheet class
// Written for CS 3505 at the University of Utah by Nicolas Metz
// meant to be used in conjunction with spreadsheet server project

#ifndef SPREAD_SHEET_H
#define SPREAD_SHEET_H

#include <string>
#include <list>
#include "depend_graph.h"
#include <map>
#include <mutex>

using namespace depend;

namespace ss
{
  typedef std::list< std::pair<std::string,std::string> > listPair;
  
  class spread_sheet
  {
  private:
    int ss_version;
    depend_graph ss_dg;
    std::list<std::pair<std::string, std::string> > ss_changes;
    std::map<std::string,std::string> ss_map;
    std::mutex ss_lock;
    std::string ss_name;

  public: 
    spread_sheet(std::string name, bool exists); //constructor
    
    std::string get_name();
    void save();
    std::pair<std::string,std::string> undo();
    bool load();

    //returns true if valid change, false if not
    bool change(std::string cell, std::string cell_content);
  };
}

#endif
