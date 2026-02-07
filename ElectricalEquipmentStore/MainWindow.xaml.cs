using ElectricalEquipmentStore.Pages;
using Microsoft.Extensions.DependencyInjection;
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
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Получаем роль пользователя из Application.Properties
            var role = Application.Current.Properties["UserRole"] as string;

            // Создаем ViewModel с передачей IServiceProvider
            var serviceProvider = (Application.Current as App)!.ServiceProvider;
            DataContext = new ViewModels.MainWindowViewModel(role, serviceProvider);
        }

        public Frame MainFrame => MainContentFrame;

        // Обработчики событий навигации (если они у вас есть в XAML)
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainContentFrame.CanGoBack)
                MainContentFrame.GoBack();
        }

       private void MainContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // Обновляем видимость кнопки "Назад"
           // BackButton.Visibility = MainContentFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        // Обработчики для кнопок навигации (если они у вас в XAML)
        private void CatalogButton_Click(object sender, RoutedEventArgs e)
        {
            var serviceProvider = (Application.Current as App)!.ServiceProvider;
            var clientPage = serviceProvider.GetRequiredService<Pages.ClientPage>();
            MainContentFrame.Navigate(clientPage);
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Страница корзины в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Страница заказов в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OrderManagementButton_Click(object sender, RoutedEventArgs e)
        {
            var serviceProvider = (Application.Current as App)!.ServiceProvider;
            var employeePage = serviceProvider.GetRequiredService<Pages.EmployeePage>();
            MainContentFrame.Navigate(employeePage);
        }

        private void SuppliesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление поставками в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AdminPanelButton_Click(object sender, RoutedEventArgs e)
        {
            var serviceProvider = (Application.Current as App)!.ServiceProvider;
            var adminPage = serviceProvider.GetRequiredService<Pages.AdminPage>();
            MainContentFrame.Navigate(adminPage);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties.Remove("CurrentUser");
            Application.Current.Properties.Remove("UserRole");

            var serviceProvider = (Application.Current as App)!.ServiceProvider;
            var loginPage = serviceProvider.GetRequiredService<Pages.LoginPage>();
            MainContentFrame.Navigate(loginPage);
        }
    }
}