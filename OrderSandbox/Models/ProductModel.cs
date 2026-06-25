using System;

namespace OrderSandbox.Models
{
    /// <summary>
    /// Товар
    /// </summary>
    public class ProductModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Внутренний код товара
        /// </summary>
        public int Code { get; set; }

        public string Title { get; set; }

        public string Manufacturer { get; set; }

        /// <summary>
        /// Жизненно важный (ЖНВЛП)
        /// </summary>
        public bool IsVital { get; set; }

        /// <summary>
        /// Рецептурный
        /// </summary>
        public bool IsPrescription { get; set; }
    }
}
