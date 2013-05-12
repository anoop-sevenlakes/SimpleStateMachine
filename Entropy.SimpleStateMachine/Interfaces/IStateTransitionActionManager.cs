namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateTransitionActionManager
    {
        IStateTransitionAction AddNewTransitionAction(string actionTaskName, StateTransitionPhase targetPhase,
                                                      params object[] args);
    }
}