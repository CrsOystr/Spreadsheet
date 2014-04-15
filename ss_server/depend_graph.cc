// Dependency  graph for spreadsheet server

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
    return dependencies.size();
  }
  
  void depend_graph::add_dependency(string dependee, string dependent)
  {
    bool is_dup = false;
    for (int i = 0; i < dependencies.size(); i++)
      {
      }
    dependencies.push_back(make_pair(dependee, dependent));
  }
  
}

