namespace Entropy.SimpleStateMachine.Utility
{
	using System;
	using System.Collections.Generic;
	using Interfaces;

	public static class ServiceLocator
	{
		private static readonly object _lock = new object();
		private static readonly Dictionary<Type, object> _serviceTable = new Dictionary<Type, object>();
		private static bool _isInitialized;

		/// <summary>
		/// Clears the registered services.
		/// </summary>
		public static void Clear()
		{
			lock (_lock)
			{
				_serviceTable.Clear();
				_isInitialized = false;
			}
		}

		/// <summary>
		/// Registers the service.
		/// </summary>
		/// <typeparam name="T">The service type</typeparam>
		/// <param name="serviceImplementation">The service implementation.</param>
		public static void RegisterService<T>(object serviceImplementation) where T : class
		{
			if (serviceImplementation == null)
				throw new ArgumentNullException("serviceImplementation");

			if (typeof (T).IsAssignableFrom(serviceImplementation.GetType()) == false)
				throw new ArgumentException(
					"The service implemenation provided is not assignable from the type provided to the generic argument when invoking RegisterService.");

			lock (_lock)
			{
				if (_serviceTable.ContainsKey(typeof (T)))
					_serviceTable[typeof (T)] = serviceImplementation;
				else
					_serviceTable.Add(typeof (T), serviceImplementation);
			}
		}

		/// <summary>
		/// Registers the service.
		/// </summary>
		/// <typeparam name="T">The service type</typeparam>
		/// <param name="serviceType">The service type.</param>
		public static void RegisterService<T>(Type serviceType) where T : class
		{
			if (serviceType == null)
				throw new ArgumentNullException("serviceType");

			if (typeof(T).IsAssignableFrom(serviceType) == false)
				throw new ArgumentException(
					"The service type provided is not assignable from the type provided to the generic argument when invoking RegisterService.");

			lock (_lock)
			{
				if (_serviceTable.ContainsKey(typeof (T)))
					_serviceTable[typeof(T)] = serviceType;
				else
					_serviceTable.Add(typeof(T), serviceType);
			}
		}

		/// <summary>
		/// Determines whether the service has been registered.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>
		/// 	<c>true</c> if this instance has the service; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasService<T>()
		{
			lock (_lock)
			{
				if (_isInitialized == false) {
					RegisterMissingServices();
					_isInitialized = true;
				}
			}

			return _serviceTable.ContainsKey(typeof (T));
		}

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <typeparam name="T">The service type</typeparam>
		/// <returns>The service or null</returns>
		public static T GetService<T>()
		{
			lock (_lock)
			{
				if(_isInitialized == false)
				{
					RegisterMissingServices();
					_isInitialized = true;
				}

				if (HasService<T>())
				{
					return (T) _serviceTable[typeof (T)];
				}
			}

			return default(T);
		}

		/// <summary>
		/// Registers the missing services.
		/// </summary>
		private static void RegisterMissingServices()
		{
			if (_serviceTable.ContainsKey(typeof(IObjectBuilder)) == false)
			{
				_serviceTable.Add(typeof(IObjectBuilder), new DefaultObjectBuilder());
			}
		}
	}
}