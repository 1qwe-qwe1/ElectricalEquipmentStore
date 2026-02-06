using ElectricalEquipmentStore.Data;
using ElectricalEquipmentStore.Models;
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
using Microsoft.EntityFrameworkCore;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ElectricalEquipmentStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private AppDbContext _context;
        private List<Product> _allProducts = new();
        private List<Category> _allCategories = new();

        public CatalogPage()
        {
            InitializeComponent();

            // Создаем контекст БД
            _context = new AppDbContext();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                NoProductsText.Visibility = Visibility.Collapsed;

                // Загружаем категории
                _allCategories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                // Обновляем ComboBox
                CategoryComboBox.Items.Clear();
                CategoryComboBox.Items.Add(new { Name = "Все категории" });
                foreach (var category in _allCategories)
                {
                    CategoryComboBox.Items.Add(category);
                }
                CategoryComboBox.SelectedIndex = 0;

                // Загружаем все товары
                await LoadProductsAsync();

                LoadingProgressBar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                LoadingProgressBar.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProductsAsync(int? categoryId = null)
        {
            try
            {
                ProductsPanel.Children.Clear();

                IQueryable<Product> query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Status)
                    .Where(p => p.Status.Name == "В наличии" ||
                               p.Status.Name == "Доступен");

                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }

                _allProducts = await query
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                // Отображаем товары
                foreach (var product in _allProducts)
                {
                    var productCard = CreateProductCard(product);
                    ProductsPanel.Children.Add(productCard);
                }

                // Показываем сообщение, если товаров нет
                NoProductsText.Visibility = _allProducts.Count == 0 ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateProductCard(Product product)
        {
            var border = new Border
            {
                Style = (Style)FindResource("ProductCardStyle"),
                Width = 250
            };

            var stackPanel = new StackPanel();

            // Изображение (заглушка)
            var imageBorder = new Border
            {
                Height = 180,
                Background = new SolidColorBrush(Color.FromRgb(232, 244, 253)),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var textBlock = new TextBlock
            {
                Text = GetEmojiForCategory(product.Category?.Name),
                FontSize = 80,
                Opacity = 0.5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            imageBorder.Child = textBlock;
            stackPanel.Children.Add(imageBorder);

            // Название
            stackPanel.Children.Add(new TextBlock
            {
                Text = product.Name,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            });

            // Описание
            if (!string.IsNullOrEmpty(product.Description))
            {
                stackPanel.Children.Add(new TextBlock
                {
                    Text = product.Description.Length > 100 ?
                        product.Description.Substring(0, 100) + "..." : product.Description,
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 5)
                });
            }

            // Производитель
            if (product.Manufacturer != null)
            {
                stackPanel.Children.Add(new TextBlock
                {
                    Text = product.Manufacturer.Name,
                    Foreground = Brushes.DodgerBlue,
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(0, 0, 0, 5)
                });
            }

            // Цена и количество
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var priceText = new TextBlock
            {
                Text = $"{product.Price:N0} ₽",
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                Foreground = Brushes.RoyalBlue
            };
            Grid.SetColumn(priceText, 0);

            var quantityText = new TextBlock
            {
                Text = $"Остаток: {product.StockQuantity} шт.",
                Foreground = Brushes.LimeGreen,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(10, 0, 0, 0)
            };
            Grid.SetColumn(quantityText, 1);

            grid.Children.Add(priceText);
            grid.Children.Add(quantityText);
            stackPanel.Children.Add(grid);

            // Кнопка "В корзину"
            var button = new Button
            {
                Content = "В корзину",
                Style = (Style)FindResource("CatalogButtonStyle"),
                Margin = new Thickness(0, 10, 0, 0),
                Tag = product // Сохраняем товар в Tag
            };
            button.Click += AddToCartButton_Click;
            stackPanel.Children.Add(button);

            border.Child = stackPanel;
            return border;
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
                _ => "🛒"
            };
        }

        private async void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem is Category selectedCategory)
            {
                await LoadProductsAsync((int?)selectedCategory.CategoryId);
            }
            else if (CategoryComboBox.SelectedIndex == 0) // "Все категории"
            {
                await LoadProductsAsync();
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            await SearchProductsAsync();
        }

        private async void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await SearchProductsAsync();
            }
        }

        private async Task SearchProductsAsync()
        {
            try
            {
                var searchText = SearchTextBox.Text.Trim();

                if (string.IsNullOrEmpty(searchText))
                {
                    await LoadProductsAsync();
                    return;
                }

                ProductsPanel.Children.Clear();

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Status)
                    .Where(p => (p.Name.Contains(searchText) ||
                                p.Description.Contains(searchText) ||
                                (p.Manufacturer != null && p.Manufacturer.Name.Contains(searchText))) &&
                               (p.Status.Name == "В наличии" || p.Status.Name == "Доступен"))
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                foreach (var product in products)
                {
                    var productCard = CreateProductCard(product);
                    ProductsPanel.Children.Add(productCard);
                }

                NoProductsText.Visibility = products.Count == 0 ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Product product)
            {
                MessageBox.Show($"Товар '{product.Name}' добавлен в корзину!", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            CategoryComboBox.SelectedIndex = 0;
            await LoadProductsAsync();
        }

        // Не забываем освободить ресурсы
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
    }
}
