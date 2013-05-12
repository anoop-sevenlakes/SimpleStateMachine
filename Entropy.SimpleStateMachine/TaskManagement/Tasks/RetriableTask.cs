using System;
using System.Runtime.Serialization;
using System.Timers;

namespace Entropy.SimpleStateMachine.TaskManagement.Tasks
{
    public interface IRetriableTask : ITask
    {
        int[] RetryInternals { get; set; }
    }

    [Serializable]
    public abstract class RetriableTask : TaskBase, IRetriableTask
    {
        #region private/protected fields

        [NonSerialized] protected TaskEventHandler _callBack;
        [NonSerialized] protected int _nCurrentAttempt;

        [NonSerialized] private int[] _retryIntervals = new[] {1000};
        // intervals in milli-sec for each retry; default to try once at an interval of 1 sec

        [NonSerialized] protected ITaskContext _taskContext;
        [NonSerialized] private Timer _timer = new Timer();

        #endregion

        protected RetriableTask()
        {
            SetTimerCallBackFunction();
        }

        protected RetriableTask(SerializationInfo info, StreamingContext context)
        {
            SetTimerCallBackFunction();
        }

        public int CurrentAttempt
        {
            get { return _nCurrentAttempt; }
        }

        #region Protected methods/properties

        // The task context is available via ITask's Execute() method; it's always a good idea to check its null status
        // when you work with it
        protected ITaskContext TaskContext
        {
            get { return _taskContext; }
        }

        protected bool CanRetry
        {
            get { return (CurrentAttempt < RetryInternals.Length); }
        }

        #endregion

        #region private methods/properties

        private object ExecuteTask(ITaskContext context, TaskEventHandler callBack)
        {
            _callBack = callBack;
            _taskContext = context;
            object taskResult = null;
            bool bExecutedOK = false;
            try
            {
                //using (BatchUpdate batch = new BatchUpdate())
                //{
                taskResult = PerformTask(_taskContext as TaskContext);
                //}
                bExecutedOK = !(taskResult is Exception);
                if (!bExecutedOK)
                    throw ((Exception) taskResult);
                return taskResult;
            }
            catch (Exception ex)
            {
                taskResult = ex;
                WaitForRetry(ex);
                if (!CanRetry)
                    return ex; // Tried the best with no luck
                return false; // In the middle of re-try
            }
            finally
            {
                var args = new TaskEventArgs(this, TaskContext, taskResult);
                if (callBack != null)
                    callBack(this, args);
                if (bExecutedOK)
                    OnTaskCompleted(args);
                OnTaskExecuted(args);
            }
        }

        private void WaitForRetry(Exception ex)
        {
            _timer.Stop();
            if (CanRetry)
            {
                _timer.Interval = _retryIntervals[CurrentAttempt];
                _timer.Start();
                _nCurrentAttempt++;
            }
            else
            {
                ActionOnCompletedWithError(ex);
            }
        }

        private void ActionOnCompletedWithError(Exception ex)
        {
            if (TaskCompletedWithError != null)
            {
                var args = new TaskEventArgs(this, TaskContext, ex);
                OnTaskCompletedWithError(args);
            }
        }

        private void SetTimerCallBackFunction()
        {
            _timer.Elapsed += Timer_Elapsed;
        }

        private void OnTaskCompleted(TaskEventArgs args)
        {
            if (TaskCompleted != null)
            {
                TaskCompleted(this, args);
            }
        }

        private void OnTaskCompletedWithError(TaskEventArgs args)
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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Time to try again!
            _timer.Stop();
            //object taskResult = ExecuteTask(TaskContext, _callBack);
            ExecuteTask(TaskContext, _callBack);
        }

        #endregion

        #region ITask members

        public new object Execute(ITaskContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            try
            {
                object output = ExecuteTask(context, null);
                return output;
            }
            catch (Exception ex)
            {
                //RuntimeEnvironment.Current.ServiceManager.ExceptionHandlerService.HandleException(ex, AppLayerName.Domain);
                return ex;
            }
        }

        #endregion

        #region IRetriableTask Members

        public int[] RetryInternals
        {
            get { return _retryIntervals; }
            set
            {
                if (value.Length > 0)
                    _retryIntervals = value;
            }
        }

        #endregion

        public event TaskEventHandler TaskExecuted;
        public event TaskEventHandler TaskCompleted;
        public event TaskEventHandler TaskCompletedWithError;

        // Overloaded in case you want to have a call back.
        public object Execute(ITaskContext context, TaskEventHandler callBack)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            return ExecuteTask(context, callBack);
        }
    }
}