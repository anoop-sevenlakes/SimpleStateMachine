workflow "Telephone Call"

trigger @CallDialed
trigger @HungUp
trigger @CallConnected
trigger @LeftMessage
trigger @PlacedOnHold
trigger @TakenOffHold
trigger @PhoneHurledAgainstWall

state @OffHook:
	when @CallDialed             >> @Ringing	
	
state @Ringing:
	when @HungUp                 >> @OffHook
	when @CallConnected          >> @Connected

state @Connected:
	when @LeftMessage            >> @OffHook
	when @HungUp                 >> @OffHook
	when @PlacedOnHold           >> @OnHold
	
state @OnHold:
	when @TakenOffHold           >> @Connected
	when @HungUp                 >> @OffHook
	when @PhoneHurledAgainstWall >> @PhoneDestroyed
	
state @PhoneDestroyed