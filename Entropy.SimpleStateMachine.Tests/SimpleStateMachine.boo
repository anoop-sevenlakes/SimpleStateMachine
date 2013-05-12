workflow @SimpleStateMachine

#define events

define_event @Event1 is 1
define_event @Event2 is 2
define_event @Event3 is 3

#global state initialization tasks

on_enter_state @TestyTheTest

state @Initial:
	when @Event1 >> @First		
	
state @First:
  when @Event1 >> @Initial
  when @Event2 >> @First
  when @Event3 >> @End
  
  on_exit_state "TestyTheTest::AlternateMethod"
  
state @End