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
 
  depend_graph::depend_graph()
  {
    std::cout << "initializing" << std::endl;
  }
  
  int depend_graph::size()
  {
    return depend_list.size();
  }

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

  void replace_dependents

  
}

