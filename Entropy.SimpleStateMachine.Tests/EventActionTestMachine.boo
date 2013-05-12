workflow @SimpleStateMachine

#define events

define_event @Event1 is 1:
	on_event @TestyTheTest
	
define_event @Event2 is 2
define_event @Event3 is 3


state @Initial:
	when @Event1 >> @First		
	
state @First:
  when @Event1 >> @Initial
  
  when @Event2 >> @First
  on_event @TestyTheTest
  
  when @Event3 >> @End
  
  
state @End