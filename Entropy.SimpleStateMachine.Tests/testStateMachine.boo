workflow @TestStateMachine

#state id template
#----------------------------------------
state_identifier_target @MyCustomEnum

#register any task assemblies
action_assembly "Entropy.SimpleStateMachine.Tests"

#define events
#shorthand for definetrigger
#----------------------------------------
trigger @Event1 is "My/Event/Path"
trigger @Event2:
	on_event @TestyTheTest


#alternate syntax
#----------------------------------------
define_event @Event3 is System.TypeCode.String


#global tasks
#----------------------------------------
on_workflow_start    @TestyTheTest
on_workflow_complete @TestyTheTest
on_enter_state       @TestyTheTest


#States & Transitions
#----------------------------------------
state @Initial is "haHaInitialState":
	when @Event1 >> @First	 	
	on_event @TestyTheTest
	 
	when @Event2 >> @Second
	
	#state specific finalization task	
	#----------------------------------------
	on_exit_state @TestyTheTest	 
	
	
	
state @First is typeof(string):
	when @Event1 >> @Initial
	when @Event2 >> @Second
	
state @Second is "Second":
	when @Event1 >> @Initial
	when @Event2 >> @First
	when @Event3 >> @NoTransitions

state @NoTransitions is "NoTransitions"

state @NoTransitions2 is MyCustomEnum.Chicken

state @Chicken

	

	

	


