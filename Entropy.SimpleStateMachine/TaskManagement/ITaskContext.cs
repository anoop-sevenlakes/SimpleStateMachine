namespace Entropy.SimpleStateMachine.TaskManagement
{
    /// <summary>
    /// Summary description for ITaskContext.
    /// </summary>
    public interface ITaskContext
    {
        object ContextObject { get; set; }
        object[] TaskArgs { get; set; }
    }
}