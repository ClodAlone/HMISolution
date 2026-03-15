using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp3.ViewModels
{
    public class TaskItemViewModel : ViewModelBase
    {
        private string _name = string.Empty;
        private int _progress;
        private string _status = "Pending";
        private bool _isRunning;
        private CancellationTokenSource? _cts;

        public TaskItemViewModel(string name)
        {
            Name = name;
            StartCommand = new RelayCommand(async _ => await StartAsync(), _ => !IsRunning);
            CancelCommand = new RelayCommand(_ => Cancel(), _ => IsRunning);
        }

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public int Progress { get => _progress; set => SetProperty(ref _progress, value); }
        public string Status { get => _status; set => SetProperty(ref _status, value); }
        public bool IsRunning 
        { 
            get => _isRunning; 
            set 
            {
                if (SetProperty(ref _isRunning, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand StartCommand { get; }
        public ICommand CancelCommand { get; }

        public async Task StartAsync()
        {
            if (IsRunning) return;

            IsRunning = true;
            Status = "Running...";
            Progress = 0;
            _cts = new CancellationTokenSource();

            try
            {
                var token = _cts.Token;
                // Simulate work
                for (int i = 0; i <= 100; i++)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(50, token); // 5 seconds total
                    Progress = i;
                }
                Status = "Completed";
            }
            catch (OperationCanceledException)
            {
                Status = "Cancelled";
                Progress = 0;
            }
            catch (Exception ex)
            {
                Status = $"Error: {ex.Message}";
            }
            finally
            {
                IsRunning = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        public void Cancel()
        {
            _cts?.Cancel();
        }
    }
}
