using System.Collections.Generic;
using System.Threading.Tasks;
using OrderSandbox.Models;

namespace OrderSandbox.Services
{
    public interface IDataLoadingService
    {
        Task<List<ProductModel>> LoadProductsAsync();

        Task<List<SupplierPriceItemModel>> LoadSupplierPriceListAsync();

        Task<List<DefectItemModel>> LoadDefectsAsync();
    }
}
