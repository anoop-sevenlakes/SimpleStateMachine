namespace Entropy.SimpleStateMachine.Interfaces
{
    public interface IStateMachineDomainContext
    {
        void AcceptNewState(IStateMachineState state);
    }
}