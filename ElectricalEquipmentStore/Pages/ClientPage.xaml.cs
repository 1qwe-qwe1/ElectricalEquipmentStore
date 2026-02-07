using ElectricalEquipmentStore.Data;
using ElectricalEquipmentStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectricalEquipmentStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private readonly AppDbContext _context;
        private List<Category> _categories = new();
        private System.Threading.CancellationTokenSource _searchCts;
        private int? _selectedCategoryId = null;
        private string _currentSearchText = "";

        // Конструктор с внедрением зависимости
        public ClientPage(AppDbContext context)
        {
            _context = context;
            InitializeComponent();
            Loaded += Page_Loaded;
            Unloaded += Page_Unloaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Отменяем все операции при закрытии страницы
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _context?.Dispose(); // Освобождаем контекст при закрытии страницы
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                _categories = await _context.Categories
                    .AsNoTracking() // Только для чтения
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                // Обновляем ComboBox в UI потоке
                await Dispatcher.InvokeAsync(() =>
                {
                    CategoryComboBox.Items.Clear();
                    CategoryComboBox.Items.Add(new { Name = "Все товары", Id = 0 });

                    foreach (var category in _categories)
                    {
                        CategoryComboBox.Items.Add(category);
                    }

                    CategoryComboBox.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    LoadingProgressBar.Visibility = Visibility.Visible;
                    ProductsPanel.Children.Clear();
                    NoProductsPanel.Visibility = Visibility.Collapsed;
                });

                // Используем AsNoTracking() для операций только для чтения
                IQueryable<Product> query = _context.Products
                    .AsNoTracking() // Важно: не отслеживаем изменения
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Status)
                    .Where(p => p.StockQuantity > 0);

                // Фильтрация по категории
                if (_selectedCategoryId.HasValue && _selectedCategoryId > 0)
                {
                    query = query.Where(p => p.CategoryId == _selectedCategoryId.Value);
                }

                // Фильтрация по поиску
                if (!string.IsNullOrWhiteSpace(_currentSearchText))
                {
                    var searchTextLower = _currentSearchText.ToLower();
                    query = query.Where(p =>
                        p.Name.ToLower().Contains(searchTextLower) ||
                        (p.Manufacturer != null && p.Manufacturer.Name.ToLower().Contains(searchTextLower)) ||
                        (p.Category != null && p.Category.Name.ToLower().Contains(searchTextLower)));
                }

                var products = await query
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                // Отображаем товары в UI потоке
                await Dispatcher.InvokeAsync(() =>
                {
                    foreach (var product in products)
                    {
                        var productCard = CreateProductCard(product);
                        ProductsPanel.Children.Add(productCard);
                    }

                    NoProductsPanel.Visibility = products.Count == 0 ?
                        Visibility.Visible : Visibility.Collapsed;
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    LoadingProgressBar.Visibility = Visibility.Collapsed;
                });
            }
        }

        // Остальной код без изменений (CreateProductCard, AddToCart, GetEmojiForCategory)

        private Border CreateProductCard(Product product)
        {
            var border = new Border
            {
                Style = (Style)FindResource("ProductCardStyle"),
                Width = 220,
                Height = 240,
                Margin = new Thickness(8),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            // Эффект при наведении
            border.MouseEnter += (s, e) =>
            {
                border.Background = new SolidColorBrush(Color.FromRgb(245, 249, 255));
                border.BorderBrush = Brushes.RoyalBlue;
            };

            border.MouseLeave += (s, e) =>
            {
                border.Background = Brushes.White;
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            };

            var stackPanel = new StackPanel();

            // Иконка товара
            var iconBorder = new Border
            {
                Height = 90,
                Width = 200,
                Background = new SolidColorBrush(Color.FromRgb(232, 244, 253)),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var iconText = new TextBlock
            {
                Text = GetEmojiForCategory(product.Category?.Name),
                FontSize = 40,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            iconBorder.Child = iconText;
            stackPanel.Children.Add(iconBorder);

            // Название товара
            var nameText = new TextBlock
            {
                Text = product.Name,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 3),
                MaxHeight = 35,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            stackPanel.Children.Add(nameText);

            // Производитель
            if (product.Manufacturer != null)
            {
                var manufacturerText = new TextBlock
                {
                    Text = product.Manufacturer.Name,
                    Foreground = Brushes.DodgerBlue,
                    FontSize = 11,
                    Margin = new Thickness(0, 0, 0, 5),
                    FontStyle = FontStyles.Italic
                };
                stackPanel.Children.Add(manufacturerText);
            }

            // Цена
            var priceText = new TextBlock
            {
                Text = $"{product.Price:N0} ₽",
                FontWeight = FontWeights.Bold,
                FontSize = 15,
                Foreground = Brushes.RoyalBlue,
                Margin = new Thickness(0, 0, 0, 8)
            };
            stackPanel.Children.Add(priceText);

            // Кнопка "В корзину"
            var button = new Button
            {
                Content = "В корзину",
                Style = (Style)FindResource("AddToCartButtonStyle"),
                Tag = product
            };

            button.Click += (s, e) =>
            {
                if (s is Button btn && btn.Tag is Product prod)
                {
                    AddToCart(prod);
                }
            };

            // Эффект при наведении на кнопку
            button.MouseEnter += (s, e) =>
            {
                button.Background = new SolidColorBrush(Color.FromRgb(53, 122, 232));
            };

            button.MouseLeave += (s, e) =>
            {
                button.Background = new SolidColorBrush(Color.FromRgb(74, 144, 226));
            };

            stackPanel.Children.Add(button);

            border.Child = stackPanel;
            return border;
        }

        private void AddToCart(Product product)
        {
            // Здесь будет логика добавления в корзину
            MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!\nЦена: {product.Price:N0} ₽",
                "Товар добавлен",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private string GetEmojiForCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return "🛒";

            return categoryName.ToLower() switch
            {
                string s when s.Contains("розет") || s.Contains("выключатель") => "🔌",
                string s when s.Contains("кабель") || s.Contains("провод") => "⚡",
                string s when s.Contains("светил") || s.Contains("лампа") => "💡",
                string s when s.Contains("щит") || s.Contains("автомат") => "🔧",
                string s when s.Contains("инструмент") => "🛠️",
                _ => "🛒"
            };
        }

        // Обработчики событий
        private async void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem == null) return;

            if (CategoryComboBox.SelectedItem is Category selectedCategory)
            {
                _selectedCategoryId = (int?)selectedCategory.CategoryId;
            }
            else if (CategoryComboBox.SelectedIndex == 0) // "Все товары"
            {
                _selectedCategoryId = null;
            }

            // Загружаем товары с учетом выбранной категории
            await LoadProductsAsync();
        }

        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Отменяем предыдущий поиск
            _searchCts?.Cancel();
            _searchCts?.Dispose();
            _searchCts = new System.Threading.CancellationTokenSource();

            var searchText = SearchTextBox.Text.Trim();
            _currentSearchText = searchText;

            try
            {
                // Ждем 500ms после последнего ввода (дебаунс)
                await Task.Delay(500, _searchCts.Token);

                // Если отмена не была запрошена, выполняем поиск
                if (!_searchCts.Token.IsCancellationRequested)
                {
                    await LoadProductsAsync();
                }
            }
            catch (TaskCanceledException)
            {
                // Поиск был отменен - ничего не делаем
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            CategoryComboBox.SelectedIndex = 0;
            _selectedCategoryId = null;
            _currentSearchText = "";
            await LoadProductsAsync();
        }
    }
}
