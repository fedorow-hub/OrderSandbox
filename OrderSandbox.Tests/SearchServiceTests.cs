using OrderSandbox.Models;
using OrderSandbox.Services;

namespace OrderSandbox.Tests;

[TestClass]
public sealed class SearchServiceTests
{
    private readonly SearchService _searchService = new();

    private static ProductModel CreateProduct(int code = 1001, string title = "Парацетамол 500гр.")
    {
        return new ProductModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            Title = title,
            Manufacturer = "Фармстандарт"
        };
    }

    [TestMethod]
    public void MatchesProduct_NullProduct_ReturnsFalse()
    {
        Assert.IsFalse(_searchService.MatchesProduct(null, "парац"));
    }

    [TestMethod]
    public void MatchesProduct_EmptySearchText_ReturnsTrue()
    {
        var product = CreateProduct();

        Assert.IsTrue(_searchService.MatchesProduct(product, string.Empty));
        Assert.IsTrue(_searchService.MatchesProduct(product, "   "));
    }

    [TestMethod]
    public void MatchesProduct_SingleWordInTitle_IgnoresCase()
    {
        var product = CreateProduct();

        Assert.IsTrue(_searchService.MatchesProduct(product, "парац"));
        Assert.IsTrue(_searchService.MatchesProduct(product, "ПАРАЦ"));
    }

    [TestMethod]
    public void MatchesProduct_SingleWordInCode_ReturnsTrue()
    {
        var product = CreateProduct(code: 1234, title: "Ибупрофен");

        Assert.IsTrue(_searchService.MatchesProduct(product, "1234"));
    }

    [TestMethod]
    public void MatchesProduct_MultipleWords_AllWordsMustMatch()
    {
        var product = CreateProduct();

        Assert.IsTrue(_searchService.MatchesProduct(product, "парац 500"));
    }

    [TestMethod]
    public void MatchesProduct_MultipleWords_OneWordMissing_ReturnsFalse()
    {
        var product = CreateProduct(title: "Парацетамол 500гр.");

        Assert.IsFalse(_searchService.MatchesProduct(product, "парац 1000"));
    }

    [TestMethod]
    public void MatchesProduct_UnrelatedSearch_ReturnsFalse()
    {
        var product = CreateProduct(title: "Парацетамол 500гр.");

        Assert.IsFalse(_searchService.MatchesProduct(product, "ибупрофен"));
    }
}
