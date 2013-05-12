using System.Collections.Generic;

namespace Entropy.SimpleStateMachine.TaskManagement.Tasks
{
    public class RetriableTaskContainer
    {
        private readonly Dictionary<ITask, ITaskContext> _registry = new Dictionary<ITask, ITaskContext>();

        public event TaskEventHandler TaskExecuted;
        public event TaskEventHandler TaskCompleted;
        public event TaskEventHandler TaskCompletedWithError;

        public void RegisterTask(RetriableTask rTask, ITaskContext taskContent)
        {
            rTask.TaskExecuted += RetriableTask_TaskExecuted;
            rTask.TaskCompleted += RetriableTask_TaskCompleted;
            rTask.TaskCompletedWithError += RetriableTask_TaskCompletedWithError;
            _registry.Add(rTask, taskContent);
        }

        #region private methods

        private void UnRegisterTask(ITask task)
        {
            if (_registry.ContainsKey(task))
            {
                _registry.Remove(task);
            }
        }

        private void RetriableTask_TaskExecuted(object sender, TaskEventArgs args)
        {
            OnTaskExecuted(args);
        }

        private void RetriableTask_TaskCompleted(object sender, TaskEventArgs args)
        {
            UnRegisterTask(args.Task);
            OnTaskComplete(args);
        }

        private void RetriableTask_TaskCompletedWithError(object sender, TaskEventArgs args)
        {
            UnRegisterTask(args.Task);
            OnTaskError(args);
        }

        protected virtual void OnTaskComplete(TaskEventArgs args)
        {
            if (TaskCompleted != null)
            {
                TaskCompleted(this, args);
            }
        }

        protected virtual void OnTaskError(TaskEventArgs args)
        {
            if (TaskCompletedWithError != null)
            {
                TaskCompletedWithError(this, args);
            }
        }

        private void OnTaskExecuted(TaskEventArgs args)
        {
            if (TaskExecuted != null)
            {
                TaskExecuted(this, args);
            }
        }

        #endregion
    }
}