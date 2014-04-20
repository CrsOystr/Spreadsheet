//Spreadsheet class for spreadsheet server
// encapsulates all the actions that a spreadsheet can have

#include <iostream>
#include <list>
#include <string>
#include "spread_sheet.h"
#include <boost/regex.hpp>

namespace ss
{
  spread_sheet::spread_sheet(string name)
  {
    this->ss_name = name;
  }
  
  bool spread_sheet::change(string cell, string cell_content)
  {
    bool valid_change = true;
    if (cell_content[0] == '=')
      {
	const char* pattern = "[a-zA-Z_][0-9][0-9]?";
	boost::regex variable_regex(pattern);
	boost::sregex_iterator iter(cell_content.begin(), cell_content.end(), variable_regex);
	boost::sregex_iterator end;
	for(;iter !=end; ++iter)
	  {
	    //adds dependency
	    this->ss_dg.add_dependency(cell,iter->str());
	    
	    //list used to test for circular dependencys
	    list<string> t_list;
	    //if statement checks for circular dependencies, if circular
	    //it removes dependencys and this method returns false
	    if (this->ss_dg.circular_check(cell, cell, t_list))
	      {
		valid_change = false;
		cout << "CIRCULAR" <<endl;
		this->ss_dg.remove_dependency(cell, iter->str());
	      }
	  }
      }
    if (valid_change)
      {
	this->ss_changes.push_back(pair<string,string>(cell, cell_content));
      }

    return valid_change;
  }
 
  

}

