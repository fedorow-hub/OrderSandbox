using System;
using System.ComponentModel;

namespace OrderSandbox.Models
{
    /// <summary>
    /// Позиция прайс-листа поставщика
    /// </summary>
    public class SupplierPriceItemModel : INotifyPropertyChanged
    {
        private int _availableQuantity;

        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string SupplierTitle { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// Доступный остаток у поставщика
        /// </summary>
        public int AvailableQuantity
        {
            get => _availableQuantity;
            set
            {
                _availableQuantity = value;
                RaisePropertyChanged(nameof(AvailableQuantity));
            }
        }

        /// <summary>
        /// Размер упаковки. Если > 1, заказ желательно делать кратно этому числу
        /// </summary>
        public int PackageSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
