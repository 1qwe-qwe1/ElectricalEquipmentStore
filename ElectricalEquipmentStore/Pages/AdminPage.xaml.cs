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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;


namespace ElectricalEquipmentStore.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private readonly AppDbContext _context;
        private Product _selectedProduct;
        private Supplier _selectedSupplier;

        public AdminPage(AppDbContext context)
        {
            _context = context;
            InitializeComponent();
            Loaded += Page_Loaded;
            Unloaded += Page_Unloaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAllDataAsync();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }

        private async Task LoadAllDataAsync()
        {
            try
            {
                await LoadProductsAsync();
                await LoadOrdersAsync();
                await LoadSuppliersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Вкладки

        private void Tab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton tab)
            {
                ProductsPanel.Visibility = Visibility.Collapsed;
                OrdersPanel.Visibility = Visibility.Collapsed;
                SuppliersPanel.Visibility = Visibility.Collapsed;

                switch (tab.Content.ToString())
                {
                    case "Товары":
                        ProductsPanel.Visibility = Visibility.Visible;
                        break;
                    case "Заказы":
                        OrdersPanel.Visibility = Visibility.Visible;
                        break;
                    case "Поставщики":
                        SuppliersPanel.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAllDataAsync();
        }

        #endregion

        #region Товары (CRUD)

        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Status)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                ProductsGrid.ItemsSource = products;

                // Загружаем данные для ComboBox
                var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                var manufacturers = await _context.Manufacturers.OrderBy(m => m.Name).ToListAsync();
                var statuses = await _context.ProductStatuses.OrderBy(s => s.Name).ToListAsync();

                Dispatcher.Invoke(() =>
                {
                    ProductCategory.ItemsSource = categories;
                    ProductManufacturer.ItemsSource = manufacturers;
                    ProductStatus.ItemsSource = statuses;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ProductsGrid.SelectedItem as Product;
            EditProductBtn.IsEnabled = _selectedProduct != null;
            DeleteProductBtn.IsEnabled = _selectedProduct != null;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ClearProductForm();
            ProductFormPanel.Visibility = Visibility.Visible;
            SaveProductBtn.Content = "💾 Добавить";
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct != null)
            {
                LoadProductToForm(_selectedProduct);
                ProductFormPanel.Visibility = Visibility.Visible;
                SaveProductBtn.Content = "💾 Сохранить изменения";
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{_selectedProduct.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Products.Remove(_selectedProduct);
                        await _context.SaveChangesAsync();
                        await LoadProductsAsync();
                        MessageBox.Show("Товар успешно удален", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления товара: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RefreshProducts_Click(object sender, RoutedEventArgs e)
        {
            LoadProductsAsync();
        }

        private async void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateProductForm())
                return;

            try
            {
                if (_selectedProduct == null) // Добавление нового товара
                {
                    var newProduct = new Product
                    {
                        Sku = ProductSku.Text.Trim(),
                        Name = ProductName.Text.Trim(),
                        Price = decimal.Parse(ProductPrice.Text),
                        CategoryId = ((Category)ProductCategory.SelectedItem).CategoryId,
                        ManufacturerId = ((Manufacturer)ProductManufacturer.SelectedItem).ManufacturerId,
                        StatusId = ((ProductStatus)ProductStatus.SelectedItem).StatusProductId,
                        StockQuantity = 0, // По умолчанию
                        CreatedAt = DateTime.Now,
                        Image = "default.png", // Заглушка
                        Description = ""
                    };

                    _context.Products.Add(newProduct);
                }
                else // Редактирование существующего товара
                {
                    _selectedProduct.Sku = ProductSku.Text.Trim();
                    _selectedProduct.Name = ProductName.Text.Trim();
                    _selectedProduct.Price = decimal.Parse(ProductPrice.Text);
                    _selectedProduct.CategoryId = ((Category)ProductCategory.SelectedItem).CategoryId;
                    _selectedProduct.ManufacturerId = ((Manufacturer)ProductManufacturer.SelectedItem).ManufacturerId;
                    _selectedProduct.StatusId = ((ProductStatus)ProductStatus.SelectedItem).StatusProductId;
                    _selectedProduct.UpdatedAt = DateTime.Now;

                    _context.Products.Update(_selectedProduct);
                }

                await _context.SaveChangesAsync();
                ProductFormPanel.Visibility = Visibility.Collapsed;
                await LoadProductsAsync();

                MessageBox.Show("Товар успешно сохранен", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения товара: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductFormPanel.Visibility = Visibility.Collapsed;
            _selectedProduct = null;
        }

        private void ClearProductForm()
        {
            ProductSku.Text = "";
            ProductName.Text = "";
            ProductPrice.Text = "";
            ProductCategory.SelectedIndex = -1;
            ProductManufacturer.SelectedIndex = -1;
            ProductStatus.SelectedIndex = -1;
            _selectedProduct = null;
        }

        private void LoadProductToForm(Product product)
        {
            ProductSku.Text = product.Sku;
            ProductName.Text = product.Name;
            ProductPrice.Text = product.Price.ToString();

            if (ProductCategory.ItemsSource is List<Category> categories)
                ProductCategory.SelectedItem = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);

            if (ProductManufacturer.ItemsSource is List<Manufacturer> manufacturers)
                ProductManufacturer.SelectedItem = manufacturers.FirstOrDefault(m => m.ManufacturerId == product.ManufacturerId);

            if (ProductStatus.ItemsSource is List<ProductStatus> statuses)
                ProductStatus.SelectedItem = statuses.FirstOrDefault(s => s.StatusProductId == product.StatusId);
        }

        private bool ValidateProductForm()
        {
            if (string.IsNullOrWhiteSpace(ProductSku.Text))
            {
                MessageBox.Show("Введите артикул товара", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(ProductName.Text))
            {
                MessageBox.Show("Введите название товара", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(ProductPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ProductCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ProductManufacturer.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ProductStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion

        #region Заказы (Чтение и обновление статуса)

        private async Task LoadOrdersAsync()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Client)
                        .ThenInclude(c => c.User)
                    .Include(o => o.Status)
                    .Include(o => o.PaymentStatus)
                    .Include(o => o.DeliveryMethod)
                    .Include(o => o.OrderProducts)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();

                OrdersGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditOrderBtn.IsEnabled = OrdersGrid.SelectedItem != null;
        }

        private async void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                try
                {
                    var statuses = await _context.OrderStatuses.ToListAsync();
                    var paymentStatuses = await _context.PaymentStatuses.ToListAsync();

                    var dialog = new Window
                    {
                        Title = $"Редактирование заказа №{selectedOrder.OrderId}",
                        Width = 400,
                        Height = 350,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };

                    var stackPanel = new StackPanel { Margin = new Thickness(20) };

                    // Статус заказа
                    stackPanel.Children.Add(new TextBlock
                    {
                        Text = "Статус заказа:",
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 0, 5)
                    });

                    var statusCombo = new ComboBox
                    {
                        ItemsSource = statuses,
                        DisplayMemberPath = "Name",
                        SelectedValuePath = "StatusId",
                        SelectedValue = selectedOrder.StatusId,
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    stackPanel.Children.Add(statusCombo);

                    // Статус оплаты
                    stackPanel.Children.Add(new TextBlock
                    {
                        Text = "Статус оплаты:",
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 0, 5)
                    });

                    var paymentCombo = new ComboBox
                    {
                        ItemsSource = paymentStatuses,
                        DisplayMemberPath = "Name",
                        SelectedValuePath = "PaymentStatusId",
                        SelectedValue = selectedOrder.PaymentStatusId,
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    stackPanel.Children.Add(paymentCombo);

                    // Комментарий
                    stackPanel.Children.Add(new TextBlock
                    {
                        Text = "Комментарий:",
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 0, 5)
                    });

                    var notesBox = new TextBox
                    {
                        Text = selectedOrder.ClientNotes ?? "",
                        Height = 60,
                        TextWrapping = TextWrapping.Wrap,
                        AcceptsReturn = true,
                        Margin = new Thickness(0, 0, 0, 15)
                    };
                    stackPanel.Children.Add(notesBox);

                    // Кнопки
                    var buttonPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right
                    };

                    var saveBtn = new Button
                    {
                        Content = "Сохранить",
                        Style = (Style)FindResource("ActionButtonStyle"),
                        Margin = new Thickness(0, 0, 10, 0)
                    };

                    saveBtn.Click += async (s, ev) =>
                    {
                        try
                        {
                            selectedOrder.StatusId = (long)statusCombo.SelectedValue;
                            selectedOrder.PaymentStatusId = (long)paymentCombo.SelectedValue;
                            selectedOrder.ClientNotes = notesBox.Text.Trim();

                            await _context.SaveChangesAsync();
                            await LoadOrdersAsync();

                            dialog.Close();
                            MessageBox.Show("Заказ успешно обновлен", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };

                    var cancelBtn = new Button
                    {
                        Content = "Отмена",
                        Style = (Style)FindResource("ActionButtonStyle"),
                        Background = Brushes.Gray
                    };
                    cancelBtn.Click += (s, ev) => dialog.Close();

                    buttonPanel.Children.Add(saveBtn);
                    buttonPanel.Children.Add(cancelBtn);
                    stackPanel.Children.Add(buttonPanel);

                    dialog.Content = stackPanel;
                    dialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrdersAsync();
        }

        #endregion

        #region Поставщики (CRUD)

        private async Task LoadSuppliersAsync()
        {
            try
            {
                var suppliers = await _context.Suppliers
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                SuppliersGrid.ItemsSource = suppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки поставщиков: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SuppliersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSupplier = SuppliersGrid.SelectedItem as Supplier;
            EditSupplierBtn.IsEnabled = _selectedSupplier != null;
            DeleteSupplierBtn.IsEnabled = _selectedSupplier != null;
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            ClearSupplierForm();
            SupplierFormPanel.Visibility = Visibility.Visible;
            SaveSupplierBtn.Content = "💾 Добавить";
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSupplier != null)
            {
                LoadSupplierToForm(_selectedSupplier);
                SupplierFormPanel.Visibility = Visibility.Visible;
                SaveSupplierBtn.Content = "💾 Сохранить изменения";
            }
        }

        private async void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSupplier != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить поставщика '{_selectedSupplier.Name}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Suppliers.Remove(_selectedSupplier);
                        await _context.SaveChangesAsync();
                        await LoadSuppliersAsync();
                        MessageBox.Show("Поставщик успешно удален", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления поставщика: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RefreshSuppliers_Click(object sender, RoutedEventArgs e)
        {
            LoadSuppliersAsync();
        }

        private async void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSupplierForm())
                return;

            try
            {
                if (_selectedSupplier == null) // Добавление нового поставщика
                {
                    var newSupplier = new Supplier
                    {
                        Name = SupplierName.Text.Trim(),
                        Phone = SupplierPhone.Text.Trim(),
                        Email = SupplierEmail.Text.Trim(),
                        BankDetails = SupplierBankDetails.Text.Trim(),
                        IsActive = true
                    };

                    _context.Suppliers.Add(newSupplier);
                }
                else // Редактирование существующего поставщика
                {
                    _selectedSupplier.Name = SupplierName.Text.Trim();
                    _selectedSupplier.Phone = SupplierPhone.Text.Trim();
                    _selectedSupplier.Email = SupplierEmail.Text.Trim();
                    _selectedSupplier.BankDetails = SupplierBankDetails.Text.Trim();

                    _context.Suppliers.Update(_selectedSupplier);
                }

                await _context.SaveChangesAsync();
                SupplierFormPanel.Visibility = Visibility.Collapsed;
                await LoadSuppliersAsync();

                MessageBox.Show("Поставщик успешно сохранен", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения поставщика: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelSupplier_Click(object sender, RoutedEventArgs e)
        {
            SupplierFormPanel.Visibility = Visibility.Collapsed;
            _selectedSupplier = null;
        }

        private void ClearSupplierForm()
        {
            SupplierName.Text = "";
            SupplierPhone.Text = "";
            SupplierEmail.Text = "";
            SupplierBankDetails.Text = "";
            _selectedSupplier = null;
        }

        private void LoadSupplierToForm(Supplier supplier)
        {
            SupplierName.Text = supplier.Name;
            SupplierPhone.Text = supplier.Phone;
            SupplierEmail.Text = supplier.Email;
            SupplierBankDetails.Text = supplier.BankDetails;
        }

        private bool ValidateSupplierForm()
        {
            if (string.IsNullOrWhiteSpace(SupplierName.Text))
            {
                MessageBox.Show("Введите название поставщика", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SupplierPhone.Text))
            {
                MessageBox.Show("Введите телефон поставщика", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(SupplierEmail.Text) || !IsValidEmail(SupplierEmail.Text))
            {
                MessageBox.Show("Введите корректный email", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
