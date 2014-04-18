// Dependency  graph for spreadsheet server
// THE DEPENDENT DEPENDS ON THE DEPENDEE
//Parent first,child second order

#include <iostream>
#include <list>
#include <utility>
#include <string>
#include "depend_graph.h"

namespace depend
{
  //Constructor
  depend_graph::depend_graph()
  {
    std::cout << "initializing a dependency graph" << std::endl;
  }
  
  //simple accessor for amount of dependencys in file
  int depend_graph::size()
  {
    return depend_list.size();
  }


  bool depend_graph::circular_check(string start, string name, list<string> visited)
  {
    bool is_circ = false;
    visited.push_back(name);
    list<string> direct_dependents = this->get_dependents(name);
    list<string>::iterator iter = direct_dependents.begin();
    
    for(;iter != direct_dependents.end(); iter++)
      {
	if ((*iter) == start)
	  is_circ = true;
	else if(!is_circ)
	  is_circ = this->circular_check(start, (*iter), visited);
      }
    if (is_circ)
      return true;
    else
      return false;
  }


  //returns true boolean if the cell is a dependee
  bool depend_graph::has_dependents(string cell)
  {
    bool is_dependee = false;
    listPair::iterator iter = depend_list.begin();
    
    for(;iter != depend_list.end(); iter++)
      {
	if ((*iter).first == cell)
	  is_dependee = true;
      }
    return is_dependee;
  }

  //returns true boolean if arg cell is a dependent
  bool depend_graph::has_dependees(string cell)
  {
    bool is_dependent = false;
    listPair::iterator iter = depend_list.begin();
    
    for(;iter != depend_list.end(); iter++)
      {
	if ((*iter).second == cell)
	  is_dependent = true;
      }
    return is_dependent;
  }

  //returns list of all dependents of arg cell
  list<string> depend_graph::get_dependents(string cell)
  {
    list<string> dependent_list;
    listPair::iterator iter = depend_list.begin();
    
    for(;iter != depend_list.end(); iter++)
      {
	if ((*iter).first == cell)
	  {
	    dependent_list.push_back((*iter).second);
	  }
      }
    return dependent_list;
  }

  //returns list of all dependees of arg cell
  list<string> depend_graph::get_dependees(string cell)
  {
    list<string> dependee_list;
    listPair::iterator iter = depend_list.begin();
    
    for(;iter != depend_list.end(); iter++)
      {
	if ((*iter).second == cell)
	  {
	    dependee_list.push_back((*iter).first);
	  }
      }
    return dependee_list;
  }
  
  //adds a dependency to our list with arg cells as dependee and dependent
  void depend_graph::add_dependency(string dependee, string dependent)
  {
    bool is_dup = false;
    pair<string, string> depend_pair (dependee, dependent);
    
    listPair::iterator iter = depend_list.begin();
    
    for(;iter != depend_list.end(); iter++)
      {
	if ((*iter).first == dependee && (*iter).second == dependent)
	  is_dup = true;
      }
    if (!is_dup)
      depend_list.push_back(depend_pair);
  }

   //removes a dependency from our list with arg cells as dependee and dependent
  void depend_graph::remove_dependency(string dependee, string dependent)
  {
    listPair::iterator iter = depend_list.begin();
    while(iter != depend_list.end())
      {
	if ((*iter).first == dependee && (*iter).second == dependent)
	  {
	    depend_list.erase(iter++);
	  }
	else
	  {
	    ++iter;
	  }
      }
  }


  //Used to remove all instances wher cell arg is a dependee and adds depency pairs will all dependents in arg list
  void depend_graph::replace_dependents(string cell, list<string> dependents)
  {
    //iterator for dependency list
    listPair::iterator iter = depend_list.begin();

    //iterator for input arg
    list<string>::iterator iter2 = dependents.begin();

    //While loop looks through dependency list and removes any pair 
    //where cell is a dependee
    while(iter != depend_list.end())
      {
	if ((*iter).first == cell)
	  {
	    depend_list.erase(iter++);
	  }
	else
	  {
	    ++iter;
	  }
      }

    //adds a dependency pair for each string in arg list as a dependent 
    //with arg cell as dependee 
    for(;iter2 != dependents.end();iter2++)
      {
	this->add_dependency(cell, (*iter2));
      }
  }



  //Used to remove all instances wher cell arg is a dependent and adds depency pairs will all dependees in arg list
  void depend_graph::replace_dependees(string cell, list<string> dependees)
  {
    //iterator for dependency list
    listPair::iterator iter = depend_list.begin();

    //iterator for input arg
    list<string>::iterator iter2 = dependees.begin();

    //While loop looks through dependency list and removes any pair 
    //where cell is a dependee
    while(iter != depend_list.end())
      {
	if ((*iter).second == cell)
	  {
	    depend_list.erase(iter++);
	  }
	else
	  {
	    ++iter;
	  }
      }

    //adds a dependency pair for each string in arg list as a dependent 
    //with arg cell as dependee 
    for(;iter2 != dependees.end();iter2++)
      {
	this->add_dependency((*iter2),cell);
      }
  }
  
}

