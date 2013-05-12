namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateMachineEvent : IStateMachineComponent
    {
        string EventName { get; }
        object EventIdentifier { get; }
        bool Matches(object eventIdentifier);
    }
}