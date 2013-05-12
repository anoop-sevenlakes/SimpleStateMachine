using System;
using System.Collections.Generic;
using Entropy.SimpleStateMachine.Interfaces;
using Entropy.SimpleStateMachine.TaskManagement;

namespace Entropy.SimpleStateMachine.Tests
{
    [TaskDescriptor(TaskName = "TestyTheTest")]
    public class TestStateMachineTask : StateMachineTask
    {
        public static List<StateTransitionPhase> PhaseHistory = new List<StateTransitionPhase>();
        public static List<IStateMachineState> StateHistory = new List<IStateMachineState>();
        public static List<IStateMachineEvent> EventHistory = new List<IStateMachineEvent>();

        public static void Clear()
        {
            StateHistory.Clear();
            PhaseHistory.Clear();
            EventHistory.Clear();
        }

        protected override object PerformStateMachineTask(StateMachineActionContext context)
        {
            StateHistory.Add(context.WorkflowContext.CurrentState);
            PhaseHistory.Add(context.CurrentPhase);
            EventHistory.Add(context.CurrentTransition.TriggerEvent);

            return DateTime.Now;
        }

        public DateTime AlternateMethod(TaskContext context)
        {
            var ctx = context.ContextObject as StateMachineActionContext;
            if (ctx != null)
            {
                StateHistory.Add(ctx.WorkflowContext.CurrentState);
                PhaseHistory.Add(ctx.CurrentPhase);
            }
            return DateTime.Now;
        }
    }
}