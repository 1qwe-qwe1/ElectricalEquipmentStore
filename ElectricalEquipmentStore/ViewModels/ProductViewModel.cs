using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectricalEquipmentStore.Models;
using ElectricalEquipmentStore.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace ElectricalEquipmentStore.ViewModels
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        private readonly ProductService _productService;

        private ObservableCollection<Product> _products = new();
        private ObservableCollection<Category> _categories = new();
        private Category _selectedCategory;
        private string _searchText = string.Empty;
        private bool _isLoading = false;
        private Product _selectedProduct;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set { _categories = value; OnPropertyChanged(); }
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                _ = CategorySelected();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public ICommand CategorySelectedCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand ViewProductDetailsCommand { get; }

        public ProductViewModel(ProductService productService)
        {
            _productService = productService;

            CategorySelectedCommand = new RelayCommand(async () => await CategorySelected());
            SearchCommand = new RelayCommand(async () => await Search());
            AddToCartCommand = new RelayCommand<Product>(AddToCart);
            ViewProductDetailsCommand = new RelayCommand<Product>(ViewProductDetails);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                var categories = await _productService.GetAllCategoriesAsync();
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                await LoadProductsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadProductsAsync(int? categoryId = null)
        {
            try
            {
                IsLoading = true;

                var products = categoryId.HasValue
                    ? await _productService.GetProductsByCategoryAsync(categoryId.Value)
                    : await _productService.GetAllProductsAsync();

                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CategorySelected()
        {
            if (SelectedCategory != null)
            {
                await LoadProductsAsync((int?)SelectedCategory.CategoryId);
            }
            else
            {
                await LoadProductsAsync();
            }
        }

        private async Task Search()
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                try
                {
                    IsLoading = true;
                    var products = await _productService.SearchProductsAsync(SearchText);

                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
            else
            {
                await LoadProductsAsync();
            }
        }

        private void AddToCart(Product product)
        {
            if (product != null)
            {
                MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ViewProductDetails(Product product)
        {
            if (product != null)
            {
                SelectedProduct = product;
                MessageBox.Show($"Детали товара:\n\n" +
                              $"Название: {product.Name}\n" +
                              $"Описание: {product.Description}\n" +
                              $"Цена: {product.Price} ₽\n" +
                              $"Количество: {product.StockQuantity}\n" +
                              $"Производитель: {product.Manufacturer?.Name}\n" +
                              $"Категория: {product.Category?.Name}",
                              "Информация о товаре",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
