using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace WpfApp3.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();

        public ICommand AddTaskCommand { get; }
        public ICommand RunAllCommand { get; }
        public ICommand CancelAllCommand { get; }

        public MainWindowViewModel()
        {
            AddTaskCommand = new RelayCommand(_ => AddTask());
            RunAllCommand = new RelayCommand(_ => RunAll(), _ => Tasks.Any(t => !t.IsRunning));
            CancelAllCommand = new RelayCommand(_ => CancelAll(), _ => Tasks.Any(t => t.IsRunning));
            
            // Add some initial tasks
            AddTask();
            AddTask();
        }

        private void AddTask()
        {
            Tasks.Add(new TaskItemViewModel($"Background Task {Tasks.Count + 1}"));
        }

        private async void RunAll()
        {
            var tasksToRun = Tasks.Where(t => !t.IsRunning).ToList();
            foreach (var task in tasksToRun)
            {
                // Fire and forget for individual tasks in this context, 
                // or await them all if we want to block the command (but better not to block UI thread).
                // Since TaskItemViewModel.StartAsync is async but we call it from void command...
                // Ideally we should track them. 
                _ = task.StartAsync(); 
            }
        }

        private void CancelAll()
        {
            foreach (var task in Tasks.Where(t => t.IsRunning))
            {
                task.Cancel();
            }
        }
    }
}
