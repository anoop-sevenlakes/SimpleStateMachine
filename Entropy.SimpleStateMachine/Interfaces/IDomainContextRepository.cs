namespace Entropy.SimpleStateMachine.Interfaces
{
	/// <summary>
	/// A domain model repostitory used by the workflow engine to persist 
	/// business entities associated with a workflow instance.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	public interface IDomainContextRepository
	{
		/// <summary>
		/// Gets the id of the business entity this worflow applies to
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		object GetId(object instance);

		/// <summary>
		/// Gets the type description of the business entity this worflow applies to
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		string GetTypeDescription(object instance);
		
		/// <summary>
		/// Loads the business entity based on the description.
		/// </summary>
		/// <param name="typeDescription">The type description.</param>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		object Load(string typeDescription, object id);

		/// <summary>
		/// Saves the business entity this worflow applies to.
		/// </summary>
		/// <param name="instance">The instance.</param>
		void Save(object instance);
	}
}
