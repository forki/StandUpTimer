using System;
using System.Windows.Threading;
using StandUpTimer.Models;

namespace StandUpTimer.Views
{
    public class DispatcherTimerWrapper : ITimer
    {
        public event EventHandler Tick;

        private readonly DispatcherTimer internalTimer;

        public DispatcherTimerWrapper()
        {
            internalTimer = new DispatcherTimer();
            internalTimer.Tick += (s, e) => OnTick();
        }

        protected virtual void OnTick()
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        public TimeSpan Interval
        {
            get { return internalTimer.Interval; }
            set { internalTimer.Interval = value; }
        }

        public void Start()
        {
            internalTimer.Start();
        }

        public void Stop()
        {
            internalTimer.Stop();
        }
    }
}