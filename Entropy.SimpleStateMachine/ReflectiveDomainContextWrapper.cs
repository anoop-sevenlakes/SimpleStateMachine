using System;
using System.Reflection;
using Entropy.SimpleStateMachine.Interfaces;

namespace Entropy.SimpleStateMachine
{
    public class ReflectiveDomainContextWrapper : IStateMachineDomainContext
    {
        public ReflectiveDomainContextWrapper(object domainContext, string statePropertyName)
        {
            if (domainContext == null)
                throw new ArgumentNullException("domainContext");

            if (String.IsNullOrEmpty(statePropertyName))
                throw new ArgumentNullException("statePropertyName");

            DomainContext = domainContext;

            Type contextType = domainContext.GetType();
            PropertyInfo prop = contextType.GetProperty(statePropertyName);
            if (prop == null)
                throw new ArgumentException(statePropertyName + " is not a property of " + contextType.Name);

            StateProperty = prop;
        }

        public object DomainContext { get; private set; }
        public PropertyInfo StateProperty { get; private set; }

        #region IStateMachineDomainContext Members

        public void AcceptNewState(IStateMachineState state)
        {
        	object value = state.StateIdentifier;

			if (StateProperty.PropertyType.IsEnum)
			{
				value = ConvertEnum(StateProperty.PropertyType, state.StateIdentifier);
			}

            StateProperty.SetValue(DomainContext, value, new object[] {});
        }

    	private static object ConvertEnum(Type desiredType, object input)
    	{
			if (input == null)
			{
				return null;
			}

			string value = Convert.ToString(input);

			if (value == String.Empty)
			{
				return null;
			}

			return Enum.Parse(desiredType, value, true);
    	}

    	#endregion
    }
}