using ElectricalEquipmentStore.Pages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectricalEquipmentStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Frame MainFrame => MainContentFrame;
        public MainWindow()
        {
            InitializeComponent();
        }

        // Обновляем видимость кнопок при навигации
        private void MainContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Скрываем кнопку "Назад" на странице логина
            if (e.Content is LoginPage)
            {
                BackButton.Visibility = Visibility.Collapsed;
                HideAllNavigationButtons();
            }
            else
            {
                BackButton.Visibility = MainContentFrame.CanGoBack ?
                    Visibility.Visible : Visibility.Collapsed;

                // Показываем кнопки навигации в зависимости от роли
                ShowNavigationButtonsForCurrentRole();

                // Обновляем имя пользователя
                UpdateUserInfo();
            }
        }

        private void HideAllNavigationButtons()
        {
            CatalogButton.Visibility = Visibility.Collapsed;
            CartButton.Visibility = Visibility.Collapsed;
            OrdersButton.Visibility = Visibility.Collapsed;
            OrderManagementButton.Visibility = Visibility.Collapsed;
            SuppliesButton.Visibility = Visibility.Collapsed;
            AdminPanelButton.Visibility = Visibility.Collapsed;
            LogoutButton.Visibility = Visibility.Collapsed;
            UserNameText.Visibility = Visibility.Collapsed;
        }

        private void ShowNavigationButtonsForCurrentRole()
        {
            // Получаем роль пользователя
            if (Application.Current.Properties.Contains("UserRole"))
            {
                var role = Application.Current.Properties["UserRole"] as string;

                // Базовые кнопки для всех авторизованных пользователей
                CatalogButton.Visibility = Visibility.Visible;
                CartButton.Visibility = Visibility.Visible;
                OrdersButton.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Visible;
                UserNameText.Visibility = Visibility.Visible;

                // Кнопки для сотрудников
                bool isEmployeeOrAdmin = role == "Сотрудник" || role == "Администратор";
                OrderManagementButton.Visibility = isEmployeeOrAdmin ?
                    Visibility.Visible : Visibility.Collapsed;
                SuppliesButton.Visibility = isEmployeeOrAdmin ?
                    Visibility.Visible : Visibility.Collapsed;

                // Кнопки для администраторов
                AdminPanelButton.Visibility = role == "Администратор" ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                // Если роль не определена, скрываем все кнопки
                HideAllNavigationButtons();
            }
        }

        private void UpdateUserInfo()
        {
            if (Application.Current.Properties.Contains("CurrentUser"))
            {
                var user = Application.Current.Properties["CurrentUser"] as Models.User;
                if (user != null)
                {
                    UserNameText.Text = $"{user.Name} {user.Surname}";
                }
            }
        }

        // Обработчики кнопок навигации
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainContentFrame.CanGoBack)
            {
                MainContentFrame.GoBack();
            }
        }

        private void CatalogButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу каталога
            var catalogPage = new CatalogPage();
            MainContentFrame.Navigate(catalogPage);
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу корзины
            MessageBox.Show("Страница корзины в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу заказов
            MessageBox.Show("Страница заказов в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OrderManagementButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на управление заказами (для сотрудников)
            MessageBox.Show("Управление заказами в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SuppliesButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на управление поставками (для сотрудников)
            MessageBox.Show("Управление поставками в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AdminPanelButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на админ панель
            MainContentFrame.Navigate(new AdminPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Выход из системы
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Очищаем данные пользователя
                if (Application.Current.Properties.Contains("CurrentUser"))
                    Application.Current.Properties.Remove("CurrentUser");
                if (Application.Current.Properties.Contains("UserRole"))
                    Application.Current.Properties.Remove("UserRole");

                // Переходим на страницу логина
                MainContentFrame.Navigate(new LoginPage());
            }
        }
    }
}