using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectricalEquipmentStore.Models;
using ElectricalEquipmentStore.Pages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ElectricalEquipmentStore.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _currentUserName;

        [ObservableProperty]
        private bool _canGoBack = false;

        [ObservableProperty]
        private bool _isAdmin = false;

        [ObservableProperty]
        private bool _isEmployee = false;

        [ObservableProperty]
        private bool _isClient = false;

        public MainWindowViewModel(string role, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // Получаем текущего пользователя
            var currentUser = Application.Current.Properties["CurrentUser"] as Models.User;
            CurrentUserName = $"{currentUser?.Name} {currentUser?.Surname}";

            // Устанавливаем видимость элементов по роли
            switch (role)
            {
                case "Администратор":
                    IsAdmin = true;
                    IsEmployee = true; // Админ видит всё
                    break;
                case "Сотрудник":
                    IsEmployee = true;
                    break;
                case "Клиент":
                    IsClient = true;
                    break;
            }

            // Загружаем стартовую страницу
            NavigateToRolePage(role);
        }

        private void NavigateToRolePage(string role)
        {
            // Находим главное окно
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow == null) return;

            switch (role)
            {
                case "Администратор":
                    var adminPage = _serviceProvider.GetRequiredService<AdminPage>();
                    mainWindow.MainFrame.Navigate(adminPage);
                    break;
                case "Сотрудник":
                    var employeePage = _serviceProvider.GetRequiredService<EmployeePage>();
                    mainWindow.MainFrame.Navigate(employeePage);
                    break;
                case "Клиент":
                    var clientPage = _serviceProvider.GetRequiredService<ClientPage>();
                    mainWindow.MainFrame.Navigate(clientPage);
                    break;
            }
        }

        [RelayCommand]
        private void Logout()
        {
            // Очищаем данные пользователя
            Application.Current.Properties.Remove("CurrentUser");
            Application.Current.Properties.Remove("UserRole");

            // Находим главное окно
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                mainWindow.MainFrame.Navigate(loginPage);
            }
        }

        [RelayCommand]
        private void NavigateToCatalog()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                var clientPage = _serviceProvider.GetRequiredService<ClientPage>();
                mainWindow.MainFrame.Navigate(clientPage);
            }
        }

        [RelayCommand]
        private void NavigateToOrders()
        {
            MessageBox.Show("Страница заказов в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void NavigateToCart()
        {
            MessageBox.Show("Страница корзины в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void NavigateToAdmin()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                var adminPage = _serviceProvider.GetRequiredService<AdminPage>();
                mainWindow.MainFrame.Navigate(adminPage);
            }
        }

        [RelayCommand]
        private void NavigateToOrderManagement()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                var employeePage = _serviceProvider.GetRequiredService<EmployeePage>();
                mainWindow.MainFrame.Navigate(employeePage);
            }
        }

        [RelayCommand]
        private void NavigateToSupplies()
        {
            MessageBox.Show("Управление поставками в разработке", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void GoBack()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow?.MainFrame.CanGoBack == true)
            {
                mainWindow.MainFrame.GoBack();
            }
        }
    }
}
