//Spreadsheet class for spreadsheet server
// encapsulates all the actions that a spreadsheet can have

#include <iostream>
#include <list>
#include <string>
#include "spread_sheet.h"
#include <boost/regex.hpp>
#include <fstream>

namespace ss
{
  spread_sheet::spread_sheet(string name, bool exists)
  {
    this->ss_name = name;

    if(exists)
      {
	load();
      }
    else
      {
	this->ss_version = 0;
      }
  }
  
  void spread_sheet::save()
  {
    map<string,string> save_map = this->ss_map;
    ofstream ss_file;
    string file_name  = this->ss_name + ".txt";

    ss_file.open(file_name.c_str());
    ss_file << "<spreadsheet>" << endl;
    ss_file << "<version>" << endl;
    ss_file << this->ss_version << endl;
    ss_file << "</version>" << endl;
    map<string,string>::iterator iter = this->ss_map.begin();
    for(; iter != ss_map.end(); iter++)
      {
	ss_file << "<cell>" <<endl;
	ss_file << "<name>" << endl << (*iter).first << endl << "</name>" << endl;
	ss_file << "<content>" << endl << (*iter).second << endl << "</content>" << endl;
	ss_file << "</cell>" <<endl;
      }
    ss_file << "</spreadsheet>" << endl;
    ss_file.close(); 
  }

  void spread_sheet::load()
  {
    //ss_lock.lock();
    string tag, value;
    ifstream ss_file ;
    string file_name  = this->ss_name + ".txt";
    ss_file.open(file_name.c_str());
    getline(ss_file,tag);
    if (tag != "<spreadsheet>")
   	cout << "ERROR READING FILE" << endl;
     
    while (getline(ss_file,tag))
      {
	if (tag == "<version>")
	  {
	    getline(ss_file,value);
	    this->ss_version = atoi(value.c_str());
	  }
	if (tag == "<cell>")
	  {
	    string cell, content;
	    getline(ss_file,tag);
	    if(tag == "<name>")
		{
		  getline(ss_file, cell);
		  getline(ss_file,tag);
		}
	    getline(ss_file,tag);
	    if (tag == "<content>")
	      {
		getline(ss_file, content);
		getline(ss_file,tag);
	      }
	    this->ss_map[cell] = content;
	    getline(ss_file,tag);
	    getline(ss_file,tag);
	  }
	  
	  
      }
  }



  void spread_sheet::undo()
  {
    bool found_change = false;
    if (this->ss_changes.size()>0)
      {
	string cell = this->ss_changes.back().first;
	string cell_contents = this->ss_changes.back().second;
	this->ss_map[cell]=cell_contents;
	this->ss_version++;
	found_change = true;
	this->ss_changes.pop_back();
      }    
  }
  
  bool spread_sheet::change(string cell, string cell_content)
  {
    //this->ss_lock.lock();
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
	//gets a map iterator and tries to see if cell exists in map yet
	map<string,string>::iterator iter;
	iter = ss_map.find(cell);
	
	//if the cell exists in map we put the old value in the change log 
	if(iter!=ss_map.end())
	  {
	    this->ss_changes.push_back(pair<string,string>(cell,(*iter).second));
	  }
	//if the cell doesnt exist in map we add a blank string to our changes
	else
	  {
	    this->ss_changes.push_back(pair<string,string>(cell, ""));
	  }
	//regardless of whether the cell exists in the map or not this works
	this->ss_map[cell]=cell_content;
	this->ss_version++;
      }
    return valid_change;
  }
 
  

}

