
#ifndef DEPEND_GRAPH_H
#define DEPEND_GRAPH_H

#include <list>
#include <string>

using namespace std;

namespace depend
{
  typedef list< pair<string,string> > listPair;
  
  class depend_graph
  {
  private:
    list<pair<string,string> > depend_list;
    int dependee_size(string cell);

  public:
    depend_graph();
    int size();
    bool has_dependents(string cell);
    bool has_dependees(string cell);

    list<string> get_dependents(string cell);
    list<string> get_dependees(string cell);

    void add_dependency(string dependee, string dependent);
    void remove_dependency(string dependee, string dependent);

    void replace_dependents(string cell, list<string>);
    void replace_dependees(string cell, list<string>);
  };

}


#endif
