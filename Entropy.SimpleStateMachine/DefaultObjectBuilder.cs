namespace Entropy.SimpleStateMachine
{
	using System;
	using Interfaces;

	/// <summary>
	/// The reason for this seemingly peculiar layer of indirection is to allow
	/// easy substitution of an IoC for DefaultObjectBuilder. I didn't want
	/// to hard code to a particular IoC, but I wanted to build internal
	/// services using the IObjectBuilder so that if someone does use an IoC
	/// the IoC can build internal services if desired.
	/// </summary>
	public class DefaultObjectBuilder : IObjectBuilder
	{
		#region IObjectBuilder Members

		/// <summary>
		/// Creates the specified type.
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <returns></returns>
		public T Create<T>() where T : class
		{
			return Create<T>(typeof(T));
		}

		/// <summary>
		/// Creates the specified type.
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public T Create<T>(Type type) where T : class
		{
			return (T)Activator.CreateInstance(type);
		}

		#endregion
	}
}