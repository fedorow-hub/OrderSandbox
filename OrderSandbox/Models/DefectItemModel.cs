using System;

namespace OrderSandbox.Models
{
    /// <summary>
    /// Дефектурная позиция (товар, который нужно заказать из-за нехватки)
    /// </summary>
    public class DefectItemModel
    {
        public Guid ProductId { get; set; }

        /// <summary>
        /// Требуемое количество
        /// </summary>
        public int RequiredQuantity { get; set; }

        public string Reason { get; set; }
    }
}
