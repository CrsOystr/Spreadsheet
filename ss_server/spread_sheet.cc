// spread_sheet.cc -- sreadsheet class for spreadsheet server
// written for CS3505 at the University of Utah by Nicolas Metz
// encapsulates all the actions that a spreadsheet can have

#include <iostream>
#include <list>
#include <string>
#include "spread_sheet.h"
#include <boost/regex.hpp>
#include <fstream>

namespace ss
{
  //constructor to open ss, bool true denotes it exists
  //false will create new ss with name instead of load with name
  spread_sheet::spread_sheet(std::string name, bool exists)
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
  
  std::string spread_sheet::get_name()
  {
    return this->ss_name;
  }
  
  std::string spread_sheet::get_version()
  {
    std::stringstream version;
    version << this->ss_version;
    std::string ss_vers = version.str();
    return ss_vers;
  }

  std::string spread_sheet::get_spread()
  {
    std::stringstream state;
    std::string ss_state;
    state << this->ss_version;
    
    std::map<std::string,std::string>::iterator iter = this->ss_map.begin();
    for(;iter != ss_map.end(); iter++)
      {
	state << '\e';
	state << (*iter).first;
	state << '\e';
	state << (*iter).second;
      } 
    ss_state = state.str();
    ss_state.push_back('\n');
    return ss_state;
  }
  
  void spread_sheet::save()
  {
    //LOck
    ss_lock.lock();
    std::ofstream ss_file;
    std::string file_name  = this->ss_name + ".ss";

    ss_file.open(file_name.c_str());
    ss_file << "<spreadsheet>" << std::endl;
    ss_file << "<version>" << std::endl;
    ss_file << this->ss_version << std::endl;
    ss_file << "</version>" << std::endl;
    std::map<std::string,std::string>::iterator iter = this->ss_map.begin();
    for(; iter != ss_map.end(); iter++)
      {
	ss_file << "<cell>" << std::endl;
	ss_file << "<name>" << std::endl << (*iter).first << std::endl << "</name>" << std::endl;
	ss_file << "<content>" << std::endl << (*iter).second << std::endl << "</content>" << std::endl;
	ss_file << "</cell>" << std::endl;
      }
    ss_file << "</spreadsheet>" << std::endl;
    ss_file.close(); 
    ss_lock.unlock();
  }

  bool spread_sheet::load()
  {
    ss_lock.lock();
    std::string tag, value;
    std::ifstream ss_file ;
    std::string file_name  = this->ss_name + ".ss";
    bool valid_load = true;
    ss_file.open(file_name.c_str());
    getline(ss_file,tag);
    if (tag != "<spreadsheet>")
      {
   	std::cout << "ERROR READING FILE" << std::endl;
	valid_load = false;
      }
    while (getline(ss_file,tag))
      {
	if (tag == "<version>")
	  {
	    getline(ss_file,value);
	    this->ss_version = atoi(value.c_str());
	  }
	if (tag == "<cell>")
	  {
	    std::string cell, content;
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
	    this->change(cell, content);
	      
	    getline(ss_file,tag);
	    getline(ss_file,tag);
	  } 
      }
    //UNLOCK
    ss_lock.unlock();
    return valid_load;
  }

  //undo function for spreadsheet, tries to undo last change made to spreadsheet
  std::pair<std::string,std::string> spread_sheet::undo()
  {
    //LOCK
    ss_lock.lock();
    std::string cell;
    std::string cell_content;
    bool found_change = false;
    if (this->ss_changes.size()>0)
      {
	cell = this->ss_changes.back().first;
	cell_content = this->ss_changes.back().second;
	this->ss_map[cell]=cell_content;
	this->ss_version++;
	found_change = true;
	this->ss_changes.pop_back();
      }  
    //LOCK
    ss_lock.unlock();  
    return std::pair<std::string,std::string>(cell, cell_content);
  }
  

  //spreadsheet change function, tries to determine if the change request is a 
  //formula, tokenizes it makes sure it valid and adds change to SS
  bool spread_sheet::change(std::string cell, std::string cell_content)
  {
    this->ss_lock.lock();

    cell[0]=toupper(cell[0]);
    bool valid_change = true;
    if (cell_content[0] == '=')
      {
	const char* pattern = "[a-zA-Z_][0-9][0-9]?";
	boost::regex variable_regex(pattern);
	boost::sregex_iterator iter(cell_content.begin(), cell_content.end(), variable_regex);
	boost::sregex_iterator end;
	for(;iter !=end; ++iter)
	  {
	    std::string dependee = iter->str();
	    dependee[0] = toupper(dependee[0]);
	    //adds dependency
	    this->ss_dg.add_dependency(cell,dependee);
	    
	    //list used to test for circular dependencys
	    std::list<std::string> t_list;
	    //if statement checks for circular dependencies, if circular
	    //it removes dependencys and this method returns false
	    if (this->ss_dg.circular_check(cell, cell, t_list))
	      {
		valid_change = false;
		std::cout << "CIRCULAR" << std::endl;
		this->ss_dg.remove_dependency(cell, iter->str());
	      }
	  }
      }
    if (valid_change)
      {
	//gets a map iterator and tries to see if cell exists in map yet
	std::map<std::string,std::string>::iterator iter;
	iter = ss_map.find(cell);
	
	//if the cell exists in map we put the old value in the change log 
	if(iter!=ss_map.end())
	  {
	    this->ss_changes.push_back(std::pair<std::string,std::string>(cell,(*iter).second));
	  }
	//if the cell doesnt exist in map we add a blank string to our changes
	else
	  {
	    this->ss_changes.push_back(std::pair<std::string,std::string>(cell, ""));
	  }
	//regardless of whether the cell exists in the map or not this works
	this->ss_map[cell]=cell_content;
	this->ss_version++;
      }
    ss_lock.unlock();
    return valid_change;
  }
}

