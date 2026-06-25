using System.ComponentModel;

namespace OrderSandbox.ViewModels
{
    /// <summary>
    /// Простой примитив для отображения состояния занятости UI во время длительных операций.
    /// Аналог AsyncBase из основного проекта, упрощённый для тестового задания.
    /// </summary>
    public class AsyncBase : INotifyPropertyChanged
    {
        private bool _isBusy;
        private string _description;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                _isBusy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Inactive)));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
        }

        /// <summary>
        /// True, если в данный момент НЕТ длительной операции - используется в CanExecute команд
        /// </summary>
        public bool Inactive => !IsBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Open(string description)
        {
            Description = description;
            IsBusy = true;
        }

        public void Close()
        {
            IsBusy = false;
            Description = null;
        }
    }
}
