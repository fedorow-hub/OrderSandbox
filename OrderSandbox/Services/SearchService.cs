using System;
using System.Linq;
using OrderSandbox.Models;

namespace OrderSandbox.Services
{
    /// <summary>
    /// Поиск и фильтрация товаров
    /// </summary>
    public class SearchService
    {
        /// <summary>
        /// Проверяет, подходит ли товар под поисковый запрос.
        /// Поддерживает поиск по нескольким словам/фрагментам через пробел,
        /// без учёта регистра, как по коду, так и по названию товара.
        /// </summary>        
        public bool MatchesProduct(ProductModel product, string searchText)
        {
            if (product == null)
                return false;

            // Пустой или пробельный запрос — считаем, что товар подходит
            if (string.IsNullOrWhiteSpace(searchText))
                return true;

            var title = product.Title ?? string.Empty;
            var code = product.Code.ToString();

            // Объединяем код и название в один текст, приводим к верхнему регистру
            var haystack = (title + " " + code).ToUpperInvariant();

            // Разбиваем запрос на слова по пробелам, убираем пустые элементы
            var words = searchText
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.ToUpperInvariant());

            // Каждое слово запроса должно встречаться в коде или названии
            return words.All(word => haystack.Contains(word));
        }
    }
}
