namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateMachineState : IStateMachineComponent, IStateTransitionActionManager, IStateEventActionManager
    {
        string StateName { get; }
        object StateIdentifier { get; }

        IStateTransition[] Transitions { get; }
        IStateTransitionAction[] StateTransitionActions { get; }
        IStateMachineEventAction[] EventActions { get; }

        bool Matches(object stateIdentifier);
    }

    public interface IStateMachineStateBuilder : IStateMachineState
    {
        IStateTransition AddNewTransition(IStateMachineEvent triggerEvent, IStateMachineState targetState);
    }
}