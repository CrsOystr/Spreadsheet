// depend_graph.c -- Header file for dependency graph class
// Written for CS 3505 at the University of Utah by Nicolas Metz
// meant to be used in conjuction with spreadsheet server project

#ifndef DEPEND_GRAPH_H
#define DEPEND_GRAPH_H

#include <list>
#include <string>

namespace depend
{
  typedef std::list< std::pair<std::string,std::string> > listPair;
  
  class depend_graph
  {
  private:
    std::list<std::pair<std::string,std::string> > depend_list;
    int dependee_size(std::string cell);

  public:
    depend_graph();
    int size();
    bool circular_check(std::string start, std::string name, std::list<std::string> visited);

    bool has_dependents(std::string cell);
    bool has_dependees(std::string cell);

    std::list<std::string> get_dependents(std::string cell);
    std::list<std::string> get_dependees(std::string cell);

    void add_dependency(std::string dependee, std::string dependent);
    void remove_dependency(std::string dependee, std::string dependent);

    void replace_dependents(std::string cell, std::list<std::string> dependents);
    void replace_dependees(std::string cell, std::list<std::string> dependess);
  };
}

#endif
