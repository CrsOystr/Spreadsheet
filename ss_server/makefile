all:
	gcc server.c -pthread -o server

depend:
	g++ depend_tests.cc depend_graph.cc -o depender /usr/local/stow/boost/boost_1_53_0/lib/libboost_*.a -I/usr/local/stow/boost/boost_1_53_0/include/ 

spread:
	g++ spread_tests.cc spread_sheet.cc depend_graph.cc -o spreader /usr/local/stow/boost/boost_1_53_0/lib/libboost_*.a -I/usr/local/stow/boost/boost_1_53_0/include/ 