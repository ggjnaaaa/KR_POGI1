using System;
using System.Windows.Threading;

namespace WpfApp1
{
    public class TimerViewModel
    {
        public event EventHandler TimeChanged;
        private DispatcherTimer _dispatcherTimer;

        public TimerViewModel()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherTimer.Tick += TimerElapsed;
        }

        public void Start() => _dispatcherTimer.Start();

        public void Stop() => _dispatcherTimer.Stop();

        private void TimerElapsed(object sender, EventArgs e) => TimeChanged?.Invoke(this, EventArgs.Empty);
    }
}
