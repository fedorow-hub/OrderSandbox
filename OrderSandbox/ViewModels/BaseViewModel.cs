using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace OrderSandbox.ViewModels
{
    /// <summary>
    /// Базовый класс для всех ViewModel
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Уведомление об изменении свойства по имени
        /// </summary>
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Уведомление об изменении свойства через expression, чтобы не дублировать строки вручную
        /// </summary>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            if (property.Body is MemberExpression memberExpression)
                RaisePropertyChanged(memberExpression.Member.Name);
        }
    }
}
