//Tests for dependancy graph

#include <iostream>
//include <boost/regex.hpp>
#include <string>
#include "depend_graph.h"

using namespace depend;
using namespace std;

int main()
{
  cout << "Testing program for Dependency Graph" << endl;
  

/*
  //BOOST TEST
  {
    string test_string("1 + 4 + A54");
    const char* pattern = "[a-zA-Z_](?: [a-zA-Z_]|\d)*";

    boost::regex variable_regex(pattern);

    boost::sregex_iterator iter(test_string.begin(), test_string.end(), variable_regex);
    boost::sregex_iterator end;
    for (; iter != end; ++iter)
      {
	cout << it->str() << "\n";
      }
  }
  */








  {
    depend_graph test1;
    string t1 = "a";
    string t2 = "b";
    test1.add_dependency(t1,t2);
    if (test1.size()==1)
      {
	cout << "test One Passed" << endl;
      }
  }
  
  {
    depend_graph test2;
    string t1 = "a";
    string t2 = "b";
    test2.add_dependency(t1,t2);
    test2.add_dependency(t1,t2);
    test2.add_dependency(t1,t2);

    if (test2.size()==1)
      {
	cout << "test Two Passed" << endl;
      }
  }

{
    depend_graph test3;
    string t1 = "a";
    string t2 = "b";
    test3.add_dependency(t1,t2);

    if (test3.has_dependents(t1) == true)
      {
	cout << "test Three Passed" << endl;
      }
  }


{
    depend_graph test4;
    string t1 = "a";
    string t2 = "b";
    test4.add_dependency(t1,t2);

    list<string> test_list = test4.get_dependents(t1);

    list<string>::iterator iter = test_list.begin();
    for(;iter != test_list.end(); iter++)
      {
	cout << "Test 4 dependents " << (*iter) << endl; 
      }
 }

//TEST NUMBER 5
 {
    depend_graph test5;
    string t1 = "a";
    string t2 = "b";
    test5.add_dependency(t1,t2);
    test5.add_dependency(t2,t1);

    test5.remove_dependency(t1,t2);

    if (test5.has_dependents(t2) == true && test5.has_dependents(t1) == false)
      {
	cout << "Test Five Passed" << endl;
      }

 }



//TEST NUMBER 6
 {
    depend_graph test6;
    string t1 = "a";
    string t2 = "b";
    string t3 = "c";
    list<string> t_list;
    t_list.push_back(t3);
    t_list.push_back(t2);

    test6.add_dependency(t1,t1);

    test6.replace_dependents(t1,t_list);

    list<string> test_list = test6.get_dependents(t1);

    list<string>::iterator iter = test_list.begin();
    for(;iter != test_list.end(); iter++)
      {
	cout << "Test 6 dependents: " << (*iter) << endl; 
      }

 }



//TEST NUMBER 7
 {
    depend_graph test7;
    string t1 = "a";
    string t2 = "b";
    string t3 = "c";
    list<string> t_list;
    t_list.push_back(t1);
    t_list.push_back(t2);

    test7.add_dependency(t3,t3);

    test7.replace_dependees(t3,t_list);

    list<string> test_list = test7.get_dependees(t3);

    list<string>::iterator iter = test_list.begin();
    for(;iter != test_list.end(); iter++)
      {
	cout << "Test 7 dependees: " << (*iter) << endl; 
      }

 }

}
