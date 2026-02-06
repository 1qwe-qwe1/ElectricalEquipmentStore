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
    /// Логика взаимодействия для EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : Page
    {
        private readonly AppDbContext _context;

        public EmployeePage(AppDbContext context)
        {
            _context = context;
            InitializeComponent();
            Loaded += Page_Loaded;
            Unloaded += Page_Unloaded;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStatisticsAsync();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                // Загружаем статистику заказов (используем имя таблицы statuses)
                var activeOrdersCount = await _context.Orders
                    .Include(o => o.Status)
                    .Where(o => o.Status.Name != "Доставлен" && o.Status.Name != "Отменен")
                    .CountAsync();

                // Загружаем новые вопросы (в реальном приложении нужна таблица Questions)
                var newQuestionsCount = 5; // Примерное значение

                Dispatcher.Invoke(() =>
                {
                    OrdersCountText.Text = activeOrdersCount.ToString();
                    QuestionsCountText.Text = newQuestionsCount.ToString();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Обработчики событий для карточек разделов

        private void SectionCard_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(245, 249, 255));
                border.BorderBrush = Brushes.RoyalBlue;
                border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    BlurRadius = 15,
                    Opacity = 0.2,
                    ShadowDepth = 3
                };
            }
        }

        private void SectionCard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = Brushes.White;
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                border.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    BlurRadius = 10,
                    Opacity = 0.1,
                    ShadowDepth = 2
                };
            }
        }

        private void OrdersSection_Click(object sender, RoutedEventArgs e)
        {
            ShowSection(OrdersView);
            LoadOrdersAsync();
        }

        private void ClientsSection_Click(object sender, RoutedEventArgs e)
        {
            ShowSection(ClientsView);
            LoadClientsAsync();
        }

        private void QuestionsSection_Click(object sender, RoutedEventArgs e)
        {
            ShowSection(QuestionsView);
            LoadQuestionsAsync();
        }

        private void StatisticsSection_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел статистики в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SuppliesSection_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел поставок в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ReportsSection_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел отчетов в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            ShowSection(MainView);
        }

        #endregion

        #region Методы для отображения разделов

        private void ShowSection(UIElement section)
        {
            MainView.Visibility = Visibility.Collapsed;
            OrdersView.Visibility = Visibility.Collapsed;
            ClientsView.Visibility = Visibility.Collapsed;
            QuestionsView.Visibility = Visibility.Collapsed;

            section.Visibility = Visibility.Visible;
        }

        #endregion

        #region Работа с заказами

        private async Task LoadOrdersAsync()
        {
            try
            {
                // Загружаем статусы заказов для фильтра (из таблицы statuses)
                var statuses = await _context.OrderStatuses.ToListAsync();
                Dispatcher.Invoke(() =>
                {
                    OrderStatusFilter.ItemsSource = statuses;
                    OrderStatusFilter.SelectedIndex = 0;
                });

                // Загружаем заказы с клиентами и статусами
                var orders = await _context.Orders
                    .Include(o => o.Client)
                    .ThenInclude(c => c.User)
                    .Include(o => o.Status)
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .OrderByDescending(o => o.CreatedAt) // Используем OrderDate вместо CreatedAt
                    .ToListAsync();

                // Вычисляем общую сумму для каждого заказа
               /* foreach (var order in orders)
                {
                    if (order.OrderProducts != null && order.OrderProducts.Any())
                    {
                        order.TotalAmount = order.OrderProducts.Sum(op => op.Quantity * op.UnitPrice);
                    }
                }*/

                Dispatcher.Invoke(() =>
                {
                    OrdersDataGrid.ItemsSource = orders;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Order order)
            {
                ShowOrderDetailsDialog(order);
            }
        }

        private void ChangeOrderStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Order order)
            {
                ShowChangeStatusDialog(order);
            }
        }

        private void ApplyOrderFilters_Click(object sender, RoutedEventArgs e)
        {
            // Реализация фильтрации заказов
            MessageBox.Show("Фильтрация заказов в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetOrderFilters_Click(object sender, RoutedEventArgs e)
        {
            OrderStatusFilter.SelectedIndex = 0;
            OrderDateFrom.SelectedDate = null;
            OrderDateTo.SelectedDate = null;
            OrderSearchBox.Text = "";
        }

        private void RefreshOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrdersAsync();
        }

        private void ExportOrdersToExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Экспорт в Excel в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowOrderDetailsDialog(Order order)
        {
            var detailsWindow = new Window
            {
                Title = $"Детали заказа №{order.OrderId}",
                Width = 600,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            // Информация о заказе
            stackPanel.Children.Add(CreateDetailRow("Номер заказа:", order.OrderId.ToString()));
            stackPanel.Children.Add(CreateDetailRow("Клиент:", $"{order.Client?.User?.Name} {order.Client?.User?.Surname}"));
            stackPanel.Children.Add(CreateDetailRow("Дата заказа:", order.CreatedAt.ToString("dd.MM.yyyy HH:mm")));
            stackPanel.Children.Add(CreateDetailRow("Статус:", order.Status?.Name ?? "Не указан"));
            stackPanel.Children.Add(CreateDetailRow("Сумма:", $"{order.TotalAmount:N0} ₽"));

            // Формируем адрес доставки
            var deliveryAddress = $"{order.DeliveryCity}, {order.DeliveryStreet}, д. {order.DeliveryBuilding}" +
                                 (order.DeliveryApartment.HasValue ? $", кв. {order.DeliveryApartment.Value}" : "");
            stackPanel.Children.Add(CreateDetailRow("Адрес доставки:", deliveryAddress));

            // Товары в заказе
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Товары в заказе:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            });

            if (order.OrderProducts != null && order.OrderProducts.Any())
            {
                var productsGrid = new Grid();
                productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                productsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                int row = 0;
                foreach (var op in order.OrderProducts)
                {
                    productsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    productsGrid.Children.Add(new TextBlock
                    {
                        Text = op.Product?.Name ?? "Неизвестный товар",
                        Margin = new Thickness(0, 5, 0, 0)
                    });
                    Grid.SetColumn(productsGrid.Children[^1], 0);
                    Grid.SetRow(productsGrid.Children[^1], row);

                    productsGrid.Children.Add(new TextBlock
                    {
                        Text = $"{op.Quantity} шт.",
                        Margin = new Thickness(10, 5, 0, 0)
                    });
                    Grid.SetColumn(productsGrid.Children[^1], 1);
                    Grid.SetRow(productsGrid.Children[^1], row);

                    productsGrid.Children.Add(new TextBlock
                    {
                       // Text = $"{op.UnitPrice:N0} ₽",
                        Margin = new Thickness(10, 5, 0, 0)
                    });
                    Grid.SetColumn(productsGrid.Children[^1], 2);
                    Grid.SetRow(productsGrid.Children[^1], row);

                    productsGrid.Children.Add(new TextBlock
                    {
                       // Text = $"{(op.Quantity * op.UnitPrice):N0} ₽",
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(10, 5, 0, 0)
                    });
                    Grid.SetColumn(productsGrid.Children[^1], 3);
                    Grid.SetRow(productsGrid.Children[^1], row);

                    row++;
                }

                stackPanel.Children.Add(productsGrid);
            }

            detailsWindow.Content = stackPanel;
            detailsWindow.ShowDialog();
        }

        private StackPanel CreateDetailRow(string label, string value)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
            stackPanel.Children.Add(new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.Bold,
                Width = 150
            });
            stackPanel.Children.Add(new TextBlock { Text = value });
            return stackPanel;
        }

        private async void ShowChangeStatusDialog(Order order)
        {
            try
            {
                var statuses = await _context.OrderStatuses.ToListAsync();

                var dialog = new Window
                {
                    Title = $"Изменение статуса заказа №{order.OrderId}",
                    Width = 400,
                    Height = 250,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                var stackPanel = new StackPanel { Margin = new Thickness(20) };

                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Текущий статус: {order.Status?.Name}",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 20)
                });

                var statusComboBox = new ComboBox
                {
                    ItemsSource = statuses,
                    DisplayMemberPath = "Name",
                    SelectedValuePath = "StatusId", // Исправлено с OrderStatusId на StatusId
                    SelectedValue = order.StatusId,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                stackPanel.Children.Add(statusComboBox);

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
                var saveButton = new Button
                {
                    Content = "Сохранить",
                    Style = (Style)FindResource("SectionButtonStyle"),
                    Margin = new Thickness(0, 0, 10, 0)
                };

                saveButton.Click += async (s, e) =>
                {
                    try
                    {
                        var selectedStatus = (OrderStatus)statusComboBox.SelectedItem;
                        order.StatusId = selectedStatus.StatusId; // Исправлено с OrderStatusId на StatusId

                        await _context.SaveChangesAsync();
                        await LoadOrdersAsync();

                        dialog.Close();
                        MessageBox.Show("Статус заказа успешно изменен", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                var cancelButton = new Button
                {
                    Content = "Отмена",
                    Style = (Style)FindResource("SectionButtonStyle"),
                    Background = Brushes.Gray
                };
                cancelButton.Click += (s, e) => dialog.Close();

                buttonPanel.Children.Add(saveButton);
                buttonPanel.Children.Add(cancelButton);
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

        #endregion

        #region Работа с клиентами

        private async Task LoadClientsAsync()
        {
            try
            {
                var clients = await _context.Clients
                    .Include(c => c.User)
                    .Include(c => c.Orders)
                        .ThenInclude(o => o.OrderProducts)
                    .ToListAsync();

                // Вычисляем статистику для каждого клиента
                /*foreach (var client in clients)
                {
                    client.TotalOrders = client.Orders?.Count ?? 0;
                    client.TotalSpent = client.Orders?
                        .Sum(o => o.OrderProducts?
                            .Sum(op => op.Quantity * op.UnitPrice) ?? 0) ?? 0;
                }*/

                Dispatcher.Invoke(() =>
                {
                    ClientsDataGrid.ItemsSource = clients;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewClientDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Client client)
            {
                ShowClientDetailsDialog(client);
            }
        }

        private void ShowClientDetailsDialog(Client client)
        {
            var detailsWindow = new Window
            {
                Title = $"Информация о клиенте: {client.User?.Name} {client.User?.Surname}",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            // Информация о клиенте
            stackPanel.Children.Add(CreateDetailRow("ФИО:", $"{client.User?.Name} {client.User?.Surname}"));
            stackPanel.Children.Add(CreateDetailRow("Email:", client.User?.Email ?? "Не указан"));
            stackPanel.Children.Add(CreateDetailRow("Телефон:", client.PhoneNumber ?? "Не указан"));

            // Формируем адрес клиента
            var address = $"{client.City ?? ""}, {client.Street ?? ""}" +
                         (!string.IsNullOrEmpty(client.Building) ? $", д. {client.Building}" : "") +
                         (client.Apartment.HasValue ? $", кв. {client.Apartment.Value}" : "");
            if (string.IsNullOrWhiteSpace(address.Replace(",", "").Trim()))
                address = "Не указан";

            stackPanel.Children.Add(CreateDetailRow("Адрес:", address));
          //  stackPanel.Children.Add(CreateDetailRow("Дата регистрации:", client.RegistrationDate.ToString("dd.MM.yyyy")));

            // Статистика
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Статистика:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            });

           // stackPanel.Children.Add(CreateDetailRow("Всего заказов:", client.TotalOrders.ToString()));
            //stackPanel.Children.Add(CreateDetailRow("Общая сумма:", $"{client.TotalSpent:N0} ₽"));

            detailsWindow.Content = stackPanel;
            detailsWindow.ShowDialog();
        }

        #endregion

        #region Работа с вопросами

        private void LoadQuestionsAsync()
        {
            // В реальном приложении здесь будет загрузка вопросов из базы данных
            // Пока используем тестовые данные
            var testQuestions = new List<QuestionModel>
            {
                new QuestionModel { Question = "Когда будет доставлен мой заказ №12345?", ClientName = "Иван Иванов", Date = DateTime.Now.AddHours(-2) },
                new QuestionModel { Question = "Есть ли скидки на оптовые закупки?", ClientName = "Петр Петров", Date = DateTime.Now.AddHours(-5) },
                new QuestionModel { Question = "Могу ли я изменить адрес доставки?", ClientName = "Сергей Сергеев", Date = DateTime.Now.AddDays(-1) },
                new QuestionModel { Question = "Когда поступит товар в наличии?", ClientName = "Анна Смирнова", Date = DateTime.Now.AddDays(-2) },
                new QuestionModel { Question = "Как оформить возврат товара?", ClientName = "Мария Козлова", Date = DateTime.Now.AddDays(-3) }
            };

            Dispatcher.Invoke(() =>
            {
                QuestionsList.ItemsSource = testQuestions;
            });
        }

        // Модель для вопросов (временная)
        private class QuestionModel
        {
            public string Question { get; set; }
            public string ClientName { get; set; }
            public DateTime Date { get; set; }
        }

        #endregion
    }
}
