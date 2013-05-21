namespace Entropy.SimpleStateMachine.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Boo.Lang;
	using Boo.Lang.Compiler.Ast;
	using Interfaces;

	public delegate void ActionDelegate();

	public abstract class StateMachineBuilder
	{
		[ThreadStatic] protected static string _event_identifier_target;
		[ThreadStatic] protected static string _state_identifier_target;

		protected IStateMachineStateBuilder CurrentState { get; set; }
		protected IStateMachineBuilder StateMachine { get; set; }
		protected List<TransitionDef> PendingTransitions { get; set; }
        protected IStateMachineEvent CurrentEvent { get; set; }

		public void BuildStateMachine(IStateMachineBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder");

			StateMachine = builder;
			PendingTransitions = new List<TransitionDef>();

			Prepare();

			//We have to add transitions to a temporary cache and apply them last
			//because in the definition of the state machine states may specify 
			//transitions to states not specified yet in the file, and circular references
			//are allowed, so we need to collect all states and then configure transitions
			//between them.
			CommitPendingTransitions();
		}

		protected void set_workflow(string workflowName)
		{
			StateMachine.SetName(workflowName);
		}


		[Meta]
		public static Expression workflow(Expression ex)
		{
			//This is very important hack, and the reason the 'workflow' element of a
			//state machine definition is required at the top of the file -
			//these 'targets' are thread static, because they need to be used 
			//in meta-methods. Therefore, they need to be cleared first thing
			//when processing a new file values from previously compiled state machine
			//files might be used and cause totally undesired consequences.
			_state_identifier_target = null;
			_event_identifier_target = null;

			return new MethodInvocationExpression(new ReferenceExpression("set_workflow"), ex);
		}

		//This is the method that the DSL code runs in
		protected abstract void Prepare();

		protected virtual IStateMachineEvent BuildEvent(string eventName, object eventIdentifier)
		{
            if (eventIdentifier != null)
                return StateMachine.AddNewEvent(eventName, eventIdentifier);
			return StateMachine.AddNewEvent(eventName);
		}

		protected void CommitPendingTransitions()
		{
			foreach (TransitionDef transition in PendingTransitions)
			{
				IStateMachineEvent triggerEvent = StateMachine.FindEventByName(transition.TriggerEvent);
				IStateMachineState targetState = StateMachine.FindStateByName(transition.TargetState);

				if (triggerEvent == null)
					throw new Exception("A non-existent event was specified in a transition for " +
					                    transition.ParentState.StateName + " : " +
					                    transition.TriggerEvent);

				if (targetState == null)
					throw new Exception(
						"A non-existent target state was specified in a transition for " +
						transition.ParentState.StateName + " : " + transition.TargetState);

				((StateMachineState)transition.ParentState ).AddNewTransition(triggerEvent, targetState, transition.RolesRequired);
			}
		}

		protected void action_assembly(string assemblyName)
		{
			Assembly asm = Assembly.Load(assemblyName);
			if (asm == null)
				throw new Exception("Action assembly " + assemblyName +
				                    " cannot be resolved. Please check your action_assembly statements to ensure they reference assemblies accessible to the runtime environment. You may need to call ReferenceAssembly on the DslEngine to provide your custom assembly to the DSL.");

			StateMachine.RegisterTaskAssembly(asm);
		}

        protected void add_event(string eventName)
        {
            add_event(eventName, null, null);
        }

        protected void add_event(string eventName, object eventIdentifier)
        {
            add_event(eventName, eventIdentifier, null);
        }

        protected void add_event(string eventName, ActionDelegate taskDelegate)
        {
            add_event(eventName, null, taskDelegate);
        }

        protected void add_event(string eventName,object eventIdentifier,ActionDelegate taskDelegate)
        {
            EnsureName();

            CurrentEvent = BuildEvent(eventName, eventIdentifier);

            if (taskDelegate != null)
                taskDelegate();

            CurrentEvent = null;
        }

	    protected void add_state(string name, object identifier, ActionDelegate taskDelegate)
		{
			EnsureName();

			CurrentState = StateMachine.AddNewState(name, identifier ?? name);
			//Load transitions & actions
			if (taskDelegate != null)
				taskDelegate();

			CurrentState = null;
		}

        protected void add_state(string name, object identifier)
		{
			add_state(name, identifier, null);
		}

		protected void add_state(string name, ActionDelegate taskDelegate)
		{
			add_state(name, null, taskDelegate);
		}

		protected void add_state(string name)
		{
			add_state(name, null, null);
		}

        protected void add_transition(string eventName, string targetStateName)
        {
            if (CurrentState != null)
            {
                var transition = new TransitionDef
                {
                    ParentState = CurrentState,
                    TriggerEvent = eventName,
                    TargetState = targetStateName,
                    RolesRequired = new List<string>()
                };

                PendingTransitions.Add(transition);
                CurrentEvent = StateMachine.FindEventByName(eventName);
            }
        }
        protected void add_transition(string eventName, string targetStateName, string rolesRequired)
        {
            if (CurrentState != null)
            {
                var transition = new TransitionDef
                {
                    ParentState = CurrentState,
                    TriggerEvent = eventName,
                    TargetState = targetStateName,
                    RolesRequired = new List<string>()
                };
                foreach (string role in rolesRequired.ToString().Replace("(", "").Replace(")", "").Split(new char[] { ',' }))
                    transition.RolesRequired.Add(role);
                PendingTransitions.Add(transition);
                CurrentEvent = StateMachine.FindEventByName(eventName);
            }
        }

		protected void on_enter_state(string taskName, params object[] args)
		{
			AddAction(taskName, args, StateTransitionPhase.StateInitialization);
		}

		protected void on_exit_state(string taskName, params object[] args)
		{
			AddAction(taskName, args, StateTransitionPhase.StateFinalization);
		}

		protected void on_change_state(string taskName, params object[] args)
		{
			AddAction(taskName, args, StateTransitionPhase.All);
		}

		protected void on_workflow_start(string taskName, params object[] args)
		{
			AddAction(taskName, args, StateTransitionPhase.StateMachineStarting);
		}

		protected void on_workflow_complete(string taskName, params object[] args)
		{
			AddAction(taskName, args, StateTransitionPhase.StateMachineComplete);
		}

        protected void on_event(string taskName, params object[] args)
        {
           if (CurrentEvent != null)
               AddEventAction(CurrentEvent.EventName, taskName, args);
        }

		//This is required to allow a meta method to be used to set variable from source and inject a null statement
		protected void no_op_placeholder(object nullArg)
		{
		}

		[Meta]
		protected static Expression state_identifier_target(StringLiteralExpression template)
		{
			_state_identifier_target = template.Value;
			return new MethodInvocationExpression(new ReferenceExpression("no_op_placeholder"),
			                                      new NullLiteralExpression());
		}

		[Meta]
		protected static Expression event_identifier_target(StringLiteralExpression template)
		{
			_event_identifier_target = template.Value;
			return new MethodInvocationExpression(new ReferenceExpression("no_op_placeholder"),
			                                      new NullLiteralExpression());
		}

		/// <summary>
		/// A shortcut/alternative syntax to define_event. Unfortunately, event itself is a keyword in boo and C#, so it doesn't seem like a 
		/// candidate, even though it would obviously be the ideal.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		[Meta]
		protected static Expression _event(Expression expression)
		{
			return define_event(expression);
		}

		[Meta]
		protected static Expression trigger(Expression expression)
		{
			return define_event(expression);
		}

        [Meta]
        protected static Expression trigger(Expression expression, BlockExpression block)
        {
            return define_event(expression, block);
        }

		[Meta]
		protected static Expression define_event(Expression expression)
		{
			var args = new List<Expression>();
			if (expression is StringLiteralExpression)
			{
				args.Add(expression);
				if (!String.IsNullOrEmpty(_event_identifier_target))
					args.Add(ExpandEventIdentifier((StringLiteralExpression) expression));
			}
			else if (expression is BinaryExpression)
			{
				var original = (BinaryExpression) expression;
				args.Add(original.Left);
				args.Add(original.Right);
			}

			var method = new ReferenceExpression("add_event");

			var invoker = new MethodInvocationExpression(method, args.ToArray());

			return invoker;
		}

        [Meta]
        protected static Expression define_event(Expression expression, BlockExpression block)
        {
            var invoker = (MethodInvocationExpression)define_event(expression);
            invoker.Arguments.Add(block);
            return invoker;
        }

		[Meta]
		protected static Expression state(Expression expression)
		{
			var args = new List<Expression>();
			if (expression is StringLiteralExpression)
			{
				args.Add(expression);
				if (!String.IsNullOrEmpty(_state_identifier_target))
				{
					args.Add(ExpandStateIdentifier((StringLiteralExpression) expression));
				}
			}
			else if (expression is BinaryExpression)
			{
				var original = (BinaryExpression) expression;
				args.Add(original.Left);
				args.Add(original.Right);
			}

			var method = new ReferenceExpression("add_state");

			var invoker = new MethodInvocationExpression(method, args.ToArray());

			return invoker;
		}

		[Meta]
		protected static Expression state(Expression expression, BlockExpression block)
		{
			var invoker = (MethodInvocationExpression) state(expression);
			invoker.Arguments.Add(block);
			return invoker;
		}


		[Meta]
		protected static Expression when(Expression expression)
		{
			var original = (BinaryExpression) expression;

			var method = new ReferenceExpression("add_transition");
            if (original.Left is BinaryExpression)
            {
                var left = (BinaryExpression) original.Left;
                //ArrayLiteralExpression rolesRequiredArray = (ArrayLiteralExpression)left.Right;
                //List<string> rolesRequired = new List<string>();
                //foreach (Expression exp in rolesRequiredArray.Items)
                //    rolesRequired.Add(exp.ToString());
                var invoker = new MethodInvocationExpression(method, left.Left, original.Right, left.Right);
                return invoker;
            }
            else
            {
                var invoker = new MethodInvocationExpression(method, original.Left, original.Right);
                return invoker;
            }
		}

		protected void AddAction(string taskName, object[] args, StateTransitionPhase phase)
		{
			EnsureName();

			if (CurrentState != null)
				CurrentState.AddNewTransitionAction(taskName, phase, args);
			else
			{
				StateMachine.AddNewTransitionAction(taskName, phase, args);
			}
		}

        protected void AddEventAction(string eventName, string taskName, object[] args)
        {
            EnsureName();

            IStateMachineEvent evt = StateMachine.FindEventByName(eventName);
            if (evt != null)
            {
                if (CurrentState != null)
                    CurrentState.AddNewEventAction(taskName, evt, args);
                else
                {
                    StateMachine.AddNewEventAction(taskName, evt, args);
                }
            }
        }

		private static Expression ExpandStateIdentifier(StringLiteralExpression stateId)
		{
			if (!String.IsNullOrEmpty(_state_identifier_target))
			{
				return new MemberReferenceExpression(new ReferenceExpression(_state_identifier_target), stateId.Value);
			}
			return stateId;
		}

		private static Expression ExpandEventIdentifier(StringLiteralExpression eventId)
		{
			if (!String.IsNullOrEmpty(_event_identifier_target))
			{
				return new MemberReferenceExpression(new ReferenceExpression(_event_identifier_target), eventId.Value);
			}
			return eventId;
		}

		private void EnsureName()
		{
			if (String.IsNullOrEmpty(StateMachine.Name))
				throw new InvalidOperationException(
					"A 'workflow' statement must be the first item in a workflow file identifying the name of the state machine. Please use the format 'workflow @MyWorkflowName");
		}

		#region Nested type: TransitionDef

		protected class TransitionDef
		{
			public IStateMachineStateBuilder ParentState;
			public string TargetState;
			public string TriggerEvent;
            public List<string> RolesRequired;
		}

		#endregion
	}
}