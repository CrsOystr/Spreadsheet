// Dependency  graph for spreadsheet server

#include <list>
#include <string>

using namespace std;

namespace depend
{

  class depend_graph
  {
  private:
    list<pair<string,string> > dependencies;
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

  depend_graph::depend_graph()
  {
    // dependencies = new list<pair<string,string> >();
  }


  int depend_graph::size()
  {
  }
  
  
  
}

