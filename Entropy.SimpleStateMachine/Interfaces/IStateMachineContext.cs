using System;

namespace Entropy.SimpleStateMachine.Interfaces
{
    public enum EventResponse
    {
        EventNotRegistered,
        EventNotInScope,
        TransitionCancelled,
        TransitionComplete
    }

    public interface IStateMachineContext : IStateMachineComponent
    {
        IStateMachineState CurrentState { get; }

        object DomainContext { get; }
		
		Guid Id { get; set; }

        bool IsStarted { get; }
        bool IsComplete { get; }
        event EventHandler<StateChangingEventArgs> BeforeStateChanged;
        event EventHandler<StateChangedEventArgs> AfterStateChanged;

        event EventHandler<StateTransitionEventArgs> StateFinalized;
        event EventHandler<StateTransitionEventArgs> StateInitialized;

        event EventHandler<StateTransitionEventArgs> StateMachineStarted;
        event EventHandler<StateTransitionEventArgs> StateMachineComplete;

        void Start();

        EventResponse HandleEvent(object eventIdentifier);

        void SetState(IStateMachineState targetState);
    }
}
