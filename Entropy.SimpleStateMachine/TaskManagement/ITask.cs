using System.Runtime.Serialization;

namespace Entropy.SimpleStateMachine.TaskManagement
{
    /// <summary>
    /// Summary description for ITask.
    /// </summary>
    public interface ITask : ISerializable
    {
        object Execute(ITaskContext context);
    }
}