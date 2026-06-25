using System;
using System.ComponentModel;

namespace OrderSandbox.Models
{
    /// <summary>
    /// Строка текущего заказа
    /// </summary>
    public class OrderItemModel : INotifyPropertyChanged
    {
        private int _quantity;

        public Guid ProductId { get; set; }

        public string ProductTitle { get; set; }

        public string SupplierTitle { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// Размер упаковки поставщика (для проверки кратности)
        /// </summary>
        public int PackageSize { get; set; }

        /// <summary>
        /// Доступный остаток у поставщика на момент добавления в заказ
        /// </summary>
        public int AvailableQuantity { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                RaisePropertyChanged(nameof(Quantity));
                RaisePropertyChanged(nameof(Sum));
            }
        }

        public decimal Sum => Price * Quantity;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
