namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateTransitionAction : IStateMachineAction
    {
        StateTransitionPhase TargetPhase { get; }

        bool Matches(IStateMachineContext context, StateTransitionPhase phase);
    }
}