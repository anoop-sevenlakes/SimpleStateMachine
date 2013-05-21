using System;
using System.Collections.Generic;
using Entropy.SimpleStateMachine;
using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public static class StateMachineExtensions
    {
        public static IStateMachineState FindState(this IStateMachine stateMachine, object stateIdentifier)
        {
            if (stateMachine == null)
                return null;

            if (stateIdentifier == null)
                throw new ArgumentNullException("stateIdentifier");

            if (stateMachine.States == null)
                return null;

            foreach (IStateMachineState state in stateMachine.States)
            {
                if (state.Matches(stateIdentifier))
                    return state;
            }

            return null;
        }

        public static IStateMachineState FindStateByName(this IStateMachine stateMachine, string stateName)
        {
            if (stateMachine == null)
                return null;

            if (String.IsNullOrEmpty(stateName))
                throw new ArgumentNullException("stateName");

            if (stateMachine.States == null)
                return null;

            foreach (IStateMachineState state in stateMachine.States)
            {
                if (state.StateName.Equals(stateName, StringComparison.OrdinalIgnoreCase))
                    return state;
            }
            return null;
        }

        public static IStateMachineEvent FindEvent(this IStateMachine stateMachine, object eventIdentifier)
        {
            if (stateMachine == null)
                return null;

            if (eventIdentifier == null)
                throw new ArgumentNullException("eventIdentifier");

            if (stateMachine.Events == null)
                return null;

            foreach (IStateMachineEvent smEvent in stateMachine.Events)
            {
                if (smEvent.Matches(eventIdentifier))
                    return smEvent;
            }

            return null;
        }

        public static IStateMachineEvent FindEventByName(this IStateMachine stateMachine, string eventName)
        {
            if (stateMachine == null)
                return null;

            if (String.IsNullOrEmpty(eventName))
                throw new ArgumentNullException("eventName");

            if (stateMachine.Events == null)
                return null;

            foreach (IStateMachineEvent smEvent in stateMachine.Events)
            {
                if (smEvent.EventName.Equals(eventName, StringComparison.CurrentCultureIgnoreCase))
                    return smEvent;
            }
            return null;
        }

        public static IStateMachineEvent AddNewEvent(this IStateMachineBuilder stateMachine, string eventName)
        {
            return stateMachine.AddNewEvent(eventName, eventName);
        }


        public static List<IStateTransition>  FindMatchingTransitions(this IStateMachineContext context)
        {
            if (context == null || context.CurrentState == null)
                throw new ArgumentNullException("context");
            
            if (context.CurrentState.Transitions == null)
                return null;

            List<IStateTransition> transitions = new List<IStateTransition>();
            foreach (IStateTransition transition in context.CurrentState.Transitions)
            {
                if (transition.Matches("NA", context))
                    transitions.Add(transition);
            }
            if (transitions.Count > 0)
                return transitions;
            else
                return null; 
            
        }

        public static IStateTransition FindFirstMatchingTransition(this IStateMachineContext context,
                                                                   IStateMachineEvent transitionEvent)
        {
            if (context == null || context.CurrentState == null)
                return null;

            if (transitionEvent == null)
                throw new ArgumentNullException("transitionEvent");

            if (context.CurrentState.Transitions == null)
                return null;

            foreach (IStateTransition transition in context.CurrentState.Transitions)
            {
                if (transition.Matches(transitionEvent, context))
                    return transition;
            }
            return null;
        }

        public static List<IStateTransitionAction> GetMatchingTransitionActions(this IStateMachineContext context,
                                                                                StateTransitionPhase transitionPhase)
        {
            var actions = new List<IStateTransitionAction>();

            if (context == null || context.StateMachine == null)
                return actions;

            if (context.StateMachine.StateTransitionActions != null)
            {
                foreach (IStateTransitionAction action in context.StateMachine.StateTransitionActions)
                {
                    if (action.Matches(context, transitionPhase))
                        actions.Add(action);
                }
            }

            if (context.CurrentState != null && context.CurrentState.StateTransitionActions != null)
            {
                foreach (IStateTransitionAction action in context.CurrentState.StateTransitionActions)
                {
                    if (action.Matches(context, transitionPhase))
                        actions.Add(action);
                }
            }

            return actions;
        }

        public static List<IStateMachineEventAction> GetMatchingEventActions(this IStateMachineContext context, object eventIdentifier)
        {
            var actions = new List<IStateMachineEventAction>();

            if (context == null || context.StateMachine == null)
                return actions;

            if (context.StateMachine.EventActions != null)
            {
                foreach (IStateMachineEventAction action in context.StateMachine.EventActions)
                {
                    if (action.Matches(context, eventIdentifier))
                        actions.Add(action);
                }
            }

            if (context.CurrentState != null && context.CurrentState.EventActions != null)
            {
                foreach (IStateMachineEventAction action in context.CurrentState.EventActions)
                {
                    if (action.Matches(context, eventIdentifier))
                        actions.Add(action);
                }
            }

            return actions;
        }

        public static void SetState(this IStateMachineContext context, object stateIdentifier)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (stateIdentifier == null)
                throw new ArgumentNullException("stateIdentifier");

            IStateMachineState target = FindState(context.StateMachine, stateIdentifier);
            if (target != null)
                context.SetState(target);
        }

        public static bool Matches(this StateTransitionPhase left, StateTransitionPhase right)
        {
            if (left == StateTransitionPhase.All || right == StateTransitionPhase.All)
                return true;
            return left == right;
        }

        public static IStateTransitionAction AddStateInitializationAction(this IStateTransitionActionManager manager,
                                                                          string actionTaskName)
        {
            return manager.AddNewTransitionAction(actionTaskName, StateTransitionPhase.StateInitialization);
        }

        public static IStateTransitionAction AddStateFinalizationAction(this IStateTransitionActionManager manager,
                                                                        string actionTaskName)
        {
            return manager.AddNewTransitionAction(actionTaskName, StateTransitionPhase.StateFinalization);
        }

        public static IStateTransitionAction AddGlobalStateTransitionAction(this IStateTransitionActionManager manager,
                                                                            string actionTaskName)
        {
            return manager.AddNewTransitionAction(actionTaskName, StateTransitionPhase.All);
        }

        public static T Create<T>(this IObjectBuilder builder) where T:class
        {
            return builder.Create<T>(typeof (T));
        }
    }
}