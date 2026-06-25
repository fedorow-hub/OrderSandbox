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
        /// Должен поддерживать поиск по нескольким словам/фрагментам через пробел,
        /// без учёта регистра, как по коду, так и по названию товара.
        /// </summary>
        /// <remarks>
        /// BUG (намеренно): текущая реализация ищет только полное совпадение всей
        /// строки запроса как одной подстроки. Запрос "парац 500" не найдёт товар
        /// "Парацетамол 500мг", хотя по требованиям ТЗ должен.
        /// Кандидату нужно переписать метод так, чтобы каждое слово запроса
        /// проверялось отдельно (логика "И"), без учёта регистра.
        /// </remarks>
        public bool MatchesProduct(ProductModel product, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return true;

            if (product == null)
                return false;

            var title = product.Title ?? string.Empty;
            var code = product.Code.ToString();

            return title.Contains(searchText) || code.Contains(searchText);
        }
    }
}
