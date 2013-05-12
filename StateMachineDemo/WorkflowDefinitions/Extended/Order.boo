workflow "Order Lifecycle"

#Event & State Identifier Targets. 
#This section controls which Types will be used
#to resolve Event or State names into strongly typed CLR objects.
#--------------------------------------------------------

state_identifier_target @OrderStatus
event_identifier_target @OrderEvents

#Global Actions
#--------------------------------------------------------
action_assembly "StateMachineDemo"

on_change_state      @WriteToHistory, "on_change_state"
on_workflow_start    @WriteToHistory, "on_workflow_start"
on_workflow_complete @WriteToHistory, "on_workflow_complete"

#Event Definitions
#--------------------------------------------------------

trigger @OrderPlaced
trigger @CreditCardApproved
trigger @CreditCardDenied
trigger @OrderCancelledByCustomer
trigger @OutOfStock
trigger @OrderStocked
trigger @OrderShipped
trigger @OrderReceived
trigger @OrderLost:
	on_event @WriteToHistory, "Order Lost event occured. Oh No! Sorry Mr. Customer!"


#State & Transition Definitions
#--------------------------------------------------------

state @AwaitingOrder:
	when @OrderPlaced              >> @AwaitingPayment
	
state @AwaitingPayment:
	when @CreditCardApproved       >> @AwaitingShipment
	when @CreditCardDenied         >> @OrderCancelled
	
	when @OrderCancelledByCustomer >> @OrderCancelled
	on_event @WriteToHistory, "The order was cancelled prior to payment. Probably accidentally clicked 'One Click Checkout'"
	
	
state @AwaitingShipment:
	when @OrderCancelledByCustomer >> @OrderCancelled
	on_event @WriteToHistory, "The order was cancelled prior to shipment, but we took the customers money, so we better refund it"

	when @OutOfStock               >> @OnBackorder
	when @OrderShipped             >> @InTransit
	
	
	
	#Individual states can define transition events as well
	
	on_enter_state @WriteToHistory, "on_enter_state(AwaitingShipment)"
	
state @OnBackorder:
	when @OrderCancelledByCustomer >> @OrderCancelled
	on_event @WriteToHistory, "The order was cancelled because it was on backorder. Damn supply chain!"
	
	when @OrderStocked             >> @AwaitingShipment
	
	
state @InTransit:
	when @OrderReceived            >> @OrderComplete
	when @OrderLost                >> @AwaitingShipment
	

#NOTE: State definitions without any transitions will cause
#the state machine to Complete when they are reached.
#------------------------------------------------------------

state @OrderComplete

state @OrderCancelled