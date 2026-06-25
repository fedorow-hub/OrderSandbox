using System;

namespace OrderSandbox.Models
{
    /// <summary>
    /// Позиция прайс-листа поставщика
    /// </summary>
    public class SupplierPriceItemModel
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string SupplierTitle { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// Доступный остаток у поставщика
        /// </summary>
        public int AvailableQuantity { get; set; }

        /// <summary>
        /// Размер упаковки. Если > 1, заказ желательно делать кратно этому числу
        /// </summary>
        public int PackageSize { get; set; }
    }
}
