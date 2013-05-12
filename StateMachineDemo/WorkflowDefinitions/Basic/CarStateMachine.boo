workflow "The Life of a Car"

trigger @EngineStarted
trigger @GearEngaged
trigger @GasApplied
trigger @BrakeApplied
trigger @GearDisengaged
trigger @EngineKilled
trigger @CarCrashedIntoTree

state @Parked:
	when @EngineStarted      >> @IdleInNeutral
	
state @IdleInNeutral:
	when @EngineKilled       >> @Parked
	when @GearEngaged        >> @IdleInGear
	
state @IdleInGear:
	when @GearDisengaged     >> @IdleInNeutral
	when @GasApplied         >> @RacingAlong

state @RacingAlong:
	when @BrakeApplied       >> @IdleInGear
	when @CarCrashedIntoTree >> @CarDestroyed

state @CarDestroyed