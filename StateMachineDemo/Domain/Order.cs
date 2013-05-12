using System;
using System.Text;

namespace StateMachineDemo.Domain
{
    public class Order
    {
        private readonly StringBuilder _history = new StringBuilder();
        private OrderStatus _currentStatus;

        public OrderStatus CurrentStatus
        {
            get { return _currentStatus; }
            set
            {
                if (value != _currentStatus)
                {
                    _currentStatus = value;
                    OnStatusChanged();
                }
            }
        }

        public string History
        {
            get { return _history.ToString(); }
        }

        public event EventHandler StatusChanged;
        public event EventHandler HistoryChanged;

        public void MakeHistory(string historyTemplate, params object[] args)
        {
            if (args != null && args.Length > 0)
                historyTemplate = String.Format(historyTemplate, args);

            _history.AppendLine(historyTemplate);
            OnHistoryChanged();
        }

        protected virtual void OnStatusChanged()
        {
            EventHandler evt = StatusChanged;
            if (evt != null)
                evt(this, new EventArgs());
        }

        protected virtual void OnHistoryChanged()
        {
            EventHandler evt = HistoryChanged;
            if (evt != null)
                evt(this, new EventArgs());
        }
    }
}