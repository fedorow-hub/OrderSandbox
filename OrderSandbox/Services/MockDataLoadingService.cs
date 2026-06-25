using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderSandbox.Models;

namespace OrderSandbox.Services
{
    /// <summary>
    /// Сервис тестовых данных. Имитирует обращение к БД через задержку.
    /// </summary>
    public class MockDataLoadingService : IDataLoadingService
    {
        private readonly List<ProductModel> _products;
        private readonly List<SupplierPriceItemModel> _priceItems;
        private readonly List<DefectItemModel> _defects;

        public MockDataLoadingService()
        {
            _products = GenerateProducts();
            _priceItems = GeneratePriceItems(_products);
            _defects = GenerateDefects(_products);
        }

        public async Task<List<ProductModel>> LoadProductsAsync()
        {
            try
            {
                await Task.Delay(600);

                // BUG (намеренно): если при загрузке возникнет исключение, оно тихо
                // проглатывается и метод вернёт пустой список. Кандидату нужно
                // убрать пустой catch и корректно сообщить об ошибке вызывающей стороне.
                if (ShouldSimulateFailure())
                    throw new InvalidOperationException("Сервис данных временно недоступен");

                return _products;
            }
            catch
            {
            }

            return new List<ProductModel>();
        }

        public async Task<List<SupplierPriceItemModel>> LoadSupplierPriceListAsync()
        {
            await Task.Delay(400);
            return _priceItems;
        }

        public async Task<List<DefectItemModel>> LoadDefectsAsync()
        {
            await Task.Delay(300);
            return _defects;
        }

        /// <summary>
        /// Включите true, чтобы проверить обработку ошибки загрузки товаров
        /// </summary>
        private bool ShouldSimulateFailure() => false;

        private static List<ProductModel> GenerateProducts()
        {
            var manufacturers = new[] { "Фармстандарт", "Биннофарм", "Озон", "Вертекс", "Велфарм" };
            var titles = new[]
            {
                "Парацетамол", "Ибупрофен", "Анальгин", "Аспирин", "Цитрамон",
                "Но-шпа", "Валидол", "Корвалол", "Йод", "Зелёнка",
                "Активированный уголь", "Лоратадин", "Супрастин", "Амоксициллин", "Азитромицин",
                "Омепразол", "Мезим", "Линекс", "Нурофен", "Терафлю",
                "Каметон", "Фурацилин", "Хлоргексидин", "Перекись водорода", "Бинт стерильный"
            };

            var products = new List<ProductModel>();
            var random = new Random(42);

            for (int i = 0; i < titles.Length; i++)
            {
                products.Add(new ProductModel
                {
                    Id = Guid.NewGuid(),
                    Code = 1000 + i,
                    Title = titles[i],
                    Manufacturer = manufacturers[random.Next(manufacturers.Length)],
                    IsVital = random.Next(0, 2) == 0,
                    IsPrescription = i % 7 == 0
                });
            }

            return products;
        }

        private static List<SupplierPriceItemModel> GeneratePriceItems(List<ProductModel> products)
        {
            var suppliers = new[] { "ПротекФарм", "Катрен", "СИА Интернейшнл", "Пульс", "Роста" };
            var random = new Random(7);
            var items = new List<SupplierPriceItemModel>();

            foreach (var product in products)
            {
                // у каждого товара предложения от 2 до 4 поставщиков
                var supplierCount = random.Next(2, 5);
                var chosenSuppliers = suppliers.OrderBy(_ => random.Next()).Take(supplierCount);

                foreach (var supplier in chosenSuppliers)
                {
                    items.Add(new SupplierPriceItemModel
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        SupplierTitle = supplier,
                        Price = Math.Round((decimal)(random.Next(50, 1500) + random.NextDouble()), 2),
                        AvailableQuantity = random.Next(0, 200),
                        PackageSize = random.Next(0, 3) == 0 ? random.Next(2, 11) : 1
                    });
                }
            }

            return items;
        }

        private static List<DefectItemModel> GenerateDefects(List<ProductModel> products)
        {
            var random = new Random(13);
            var reasons = new[] { "Закончился остаток", "Высокий спрос", "Не привезли по графику", "Возврат от клиента" };

            return products
                .OrderBy(_ => random.Next())
                .Take(8)
                .Select(p => new DefectItemModel
                {
                    ProductId = p.Id,
                    RequiredQuantity = random.Next(1, 30),
                    Reason = reasons[random.Next(reasons.Length)]
                })
                .ToList();
        }
    }
}
