using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using OrderSandbox.Commands;
using OrderSandbox.Models;
using OrderSandbox.Services;

namespace OrderSandbox.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private readonly IDataLoadingService _loadingService;
        private readonly SearchService _searchService;

        private List<ProductModel> _allProducts = new List<ProductModel>();
        private List<SupplierPriceItemModel> _allPriceItems = new List<SupplierPriceItemModel>();

        private string _searchText = string.Empty;
        private string _defectSearchText = string.Empty;

        private ProductModel _selectedProduct;
        private SupplierPriceItemModel _selectedSupplierItem;
        private DefectItemModel _selectedDefect;

        private int _quantityToAdd = 1;
        private string _errorMessage;

        private ICollectionView _productsView;
        private ICollectionView _defectsView;

        public AsyncBase AsyncBase { get; } = new AsyncBase();

        /// <summary>
        /// Все товары (несортированный список, для подстановки в ICollectionView)
        /// </summary>
        public ObservableCollection<ProductModel> Products { get; } = new ObservableCollection<ProductModel>();

        /// <summary>
        /// Дефектурные позиции
        /// </summary>
        public ObservableCollection<DefectItemModel> Defects { get; } = new ObservableCollection<DefectItemModel>();

        /// <summary>
        /// Предложения поставщиков для выбранного товара
        /// </summary>
        public ObservableCollection<SupplierPriceItemModel> SupplierOffers { get; } = new ObservableCollection<SupplierPriceItemModel>();

        /// <summary>
        /// Текущий заказ
        /// </summary>
        public ObservableCollection<OrderItemModel> OrderItems { get; } = new ObservableCollection<OrderItemModel>();

        public ICollectionView ProductsView => _productsView;

        public ICollectionView DefectsView => _defectsView;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => SearchText);
            }
        }

        public string DefectSearchText
        {
            get => _defectSearchText;
            set
            {
                _defectSearchText = value;
                RaisePropertyChanged(() => DefectSearchText);

                // обновление списка дефектуры по поиску происходит сразу, в отличие
                // от ProductsView (см. SearchCommand) - сделано намеренно по-разному,
                // т.к. в реальном проекте такая несогласованность встречается часто
                _defectsView?.Refresh();
            }
        }

        public ProductModel SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                RaisePropertyChanged(() => SelectedProduct);

                UpdateSupplierOffers();
            }
        }

        public SupplierPriceItemModel SelectedSupplierItem
        {
            get => _selectedSupplierItem;
            set
            {
                _selectedSupplierItem = value;
                RaisePropertyChanged(() => SelectedSupplierItem);
            }
        }

        public DefectItemModel SelectedDefect
        {
            get => _selectedDefect;
            set
            {
                _selectedDefect = value;
                RaisePropertyChanged(() => SelectedDefect);

                if (value != null)
                {
                    var product = _allProducts.FirstOrDefault(p => p.Id == value.ProductId);
                    SelectedProduct = product;
                    QuantityToAdd = value.RequiredQuantity;
                }
            }
        }

        public int QuantityToAdd
        {
            get => _quantityToAdd;
            set
            {
                _quantityToAdd = value;
                RaisePropertyChanged(() => QuantityToAdd);
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }

        public decimal TotalSum => OrderItems.Sum(x => x.Sum);

        public OrderViewModel() : this(new MockDataLoadingService())
        {
        }

        public OrderViewModel(IDataLoadingService loadingService)
        {
            _loadingService = loadingService;
            _searchService = new SearchService();

            _ = LoadDataAsync();
        }

        public ICommand LoadDataCommand =>
            new RelayCommand(async () => await LoadDataAsync(), () => AsyncBase.Inactive);

        public ICommand SearchCommand =>
            new RelayCommand(() => _productsView?.Refresh(), () => true);

        public ICommand ClearSearchCommand =>
            new RelayCommand(() =>
            {
                SearchText = string.Empty;
                _productsView?.Refresh();
            }, () => true);

        /// <summary>
        /// Добавление выбранного товара/предложения в заказ
        /// </summary>
        public ICommand AddToOrderCommand =>
            new RelayCommand(AddToOrderExecute, AddToOrderCanExecute);

        public ICommand RemoveFromOrderCommand =>
            new RelayCommand(RemoveFromOrderExecute, () => true);

        private async Task LoadDataAsync()
        {
            AsyncBase.Open("Загрузка данных");
            ErrorMessage = null;
            try
            {
                var products = await _loadingService.LoadProductsAsync();
                var priceItems = await _loadingService.LoadSupplierPriceListAsync();
                var defects = await _loadingService.LoadDefectsAsync();

                _allProducts = products;
                _allPriceItems = priceItems;

                Products.Clear();
                foreach (var p in _allProducts)
                    Products.Add(p);

                Defects.Clear();
                foreach (var d in defects)
                    Defects.Add(d);

                if (_productsView == null)
                {
                    _productsView = CollectionViewSource.GetDefaultView(Products);
                    _productsView.Filter = ProductFilterPredicate;
                    RaisePropertyChanged(() => ProductsView);
                }

                if (_defectsView == null)
                {
                    _defectsView = CollectionViewSource.GetDefaultView(Defects);
                    _defectsView.Filter = DefectFilterPredicate;
                    RaisePropertyChanged(() => DefectsView);
                }
            }
            finally
            {
                AsyncBase.Close();
            }
        }

        private bool ProductFilterPredicate(object obj)
        {
            return obj is ProductModel product && _searchService.MatchesProduct(product, SearchText);
        }

        private bool DefectFilterPredicate(object obj)
        {
            if (!(obj is DefectItemModel defect))
                return false;

            if (string.IsNullOrWhiteSpace(DefectSearchText))
                return true;

            var product = _allProducts.FirstOrDefault(p => p.Id == defect.ProductId);
            return product != null && _searchService.MatchesProduct(product, DefectSearchText);
        }

        private void UpdateSupplierOffers()
        {
            SupplierOffers.Clear();

            if (SelectedProduct == null)
                return;

            var offers = _allPriceItems
                .Where(x => x.ProductId == SelectedProduct.Id)
                .OrderBy(x => x.Price);

            foreach (var offer in offers)
                SupplierOffers.Add(offer);
        }

        private void AddToOrderExecute()
        {
            ErrorMessage = null;

            // BUG (намеренно): здесь нет проверки SelectedSupplierItem на null,
            // хотя CanExecute это разрешает (см. AddToOrderCanExecute) -
            // нужно привести в соответствие валидацию и условие доступности кнопки,
            // а также проверить остаток и кратность размеру упаковки.

            var existing = OrderItems.FirstOrDefault(x =>
                x.ProductId == SelectedProduct.Id &&
                x.SupplierTitle == SelectedSupplierItem.SupplierTitle);

            if (existing != null)
            {
                existing.Quantity += QuantityToAdd;
            }
            else
            {
                OrderItems.Add(new OrderItemModel
                {
                    ProductId = SelectedProduct.Id,
                    ProductTitle = SelectedProduct.Title,
                    SupplierTitle = SelectedSupplierItem.SupplierTitle,
                    Price = SelectedSupplierItem.Price,
                    Quantity = QuantityToAdd,
                    PackageSize = SelectedSupplierItem.PackageSize,
                    AvailableQuantity = SelectedSupplierItem.AvailableQuantity
                });
            }

            RaisePropertyChanged(() => TotalSum);
        }

        private bool AddToOrderCanExecute()
        {
            //Исправлен баг активной кнопки при не выбранном SelectedSupplierItem и QuantityToAdd <= 0
            return SelectedProduct != null && SelectedSupplierItem != null && QuantityToAdd > 0;
        }

        private void RemoveFromOrderExecute()
        {
            // Метод принимает выбранный элемент через привязку SelectedItem на View;
            // в этой заготовке параметр не передаётся через CommandParameter -
            // кандидату нужно решить, как лучше передать выбранную строку заказа
            // (через CommandParameter, отдельное свойство SelectedOrderItem и т.д.)
        }
    }
}
