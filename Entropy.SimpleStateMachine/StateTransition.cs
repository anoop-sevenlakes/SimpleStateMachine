using Entropy.SimpleStateMachine.Configuration;
using Entropy.SimpleStateMachine.Interfaces;
using System.Collections.Generic;

namespace Entropy.SimpleStateMachine
{
    public class StateTransition : StateMachineComponent, IStateTransition
    {
        public List<string> RolesRequired { get; set; }

        public StateTransition(IStateMachineState parent, IStateMachineEvent triggerEvent,
                               IStateMachineState targetState, List<string> rolesRequired)
            : base(parent.StateMachine)
        {
            ParentState = parent;
            TriggerEvent = triggerEvent;
            TargetState = targetState;
            RolesRequired = rolesRequired;
        }

        public StateTransition(IStateMachineState parent, IStateMachineEvent triggerEvent,
                               IStateMachineState targetState) : base(parent.StateMachine)
        {
            ParentState = parent;
            TriggerEvent = triggerEvent;
            TargetState = targetState;
        }

        internal StateTransition(IStateMachine stateMachine, IStateMachineState initialState) : base(stateMachine)
        {
            TargetState = initialState;
        }

        #region IStateTransition Members

        public virtual bool Matches(object eventId, IStateMachineContext context)
        {
            if (TriggerEvent != null)
                if (TriggerEvent.Matches(eventId) || (eventId is string &&  (  eventId.ToString().Equals( "NA" ))))
                 {
                    if (this.RolesRequired != null && this.RolesRequired.Count > 0)
                    {

                        foreach (string role in this.RolesRequired)
                        {
                            AFEContext afe = new AFEContext();
                            afe.Assignments = new List<AFEAssignment>();
                            afe.UserID = 1;
                            afe.Assignments.Add(new AFEAssignment() { AfeRoleCode = "AAPR", Order = 1, UserID = 1 });
                            afe.Assignments.Add(new AFEAssignment() { AfeRoleCode = "AAPR", Order = 2, UserID = 2 });
                            

                            bool IsLastUserInTheCurrentOrder = afe.IsLastUserInTheCurrentOrder(role);

                            bool IsLastPersonInTheCurrentRole = afe.IsLastPersonInTheCurrentRole(role);

                            if (afe.Assignments.FindAll(t => t.AfeRoleCode == role && t.UserID == afe.UserID).Count > 0)
                                return true;
                            else
                                return false;
                        }
                    }
                    else
                        return true;
                }
            return false;
        }

        public IStateMachineState ParentState { get; protected set; }
        public IStateMachineEvent TriggerEvent { get; protected set; }
        public IStateMachineState TargetState { get; protected set; }

        #endregion

        public override string ToString()
        {
            if (TriggerEvent != null)
                return TriggerEvent.EventName + " => " + TargetState.StateName;
            return "=> " + TargetState.StateName;
        }
    }
}