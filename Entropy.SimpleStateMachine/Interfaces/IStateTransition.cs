namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateTransition : IStateMachineComponent
    {
        IStateMachineState ParentState { get; }
        IStateMachineEvent TriggerEvent { get; }
        IStateMachineState TargetState { get; }
        bool Matches(object eventIdentifier, IStateMachineContext context);
    }
}