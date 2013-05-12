namespace Entropy.SimpleStateMachine.Interfaces
{
	using System;

	public interface IObjectBuilder
	{
		/// <summary>
		/// Creates the specified type.
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <returns></returns>
		T Create<T>() where T : class;

		/// <summary>
		/// Creates the specified type.
		/// </summary>
		/// <typeparam name="T">The type</typeparam>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		T Create<T>(Type type) where T : class;
	}
}