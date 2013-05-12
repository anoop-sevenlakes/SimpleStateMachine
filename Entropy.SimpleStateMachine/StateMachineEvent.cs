using System;
using Entropy.SimpleStateMachine;
using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class StateMachineEvent : StateMachineComponent, IStateMachineEvent
    {
        public StateMachineEvent(IStateMachine stateMachine, string eventName) : this(stateMachine, eventName, null)
        {
        }

        public StateMachineEvent(IStateMachine parent, string eventName, object eventIdentifier) : base(parent)
        {
            EventName = eventName;
            EventIdentifier = eventIdentifier ?? eventName;
        }

        #region IStateMachineEvent Members

        public string EventName { get; private set; }

        public object EventIdentifier { get; private set; }

        public bool Matches(object eventIdentifier)
        {
            if (eventIdentifier == null)
                return false;

            if (eventIdentifier is IStateMachineEvent)
                return EventName.Equals(((IStateMachineEvent) eventIdentifier).EventName);

            if (eventIdentifier is string)
                return ((string) eventIdentifier).Equals(EventIdentifier.ToString(),
                                                         StringComparison.CurrentCultureIgnoreCase);

            if (EventIdentifier is string)
                return ((string) EventIdentifier).Equals(eventIdentifier.ToString(),
                                                         StringComparison.CurrentCultureIgnoreCase);

            if (eventIdentifier is IComparable)
                return ((IComparable) eventIdentifier).CompareTo(EventIdentifier) == 0;

            return EventIdentifier.Equals(eventIdentifier);
        }

        #endregion
    }
}