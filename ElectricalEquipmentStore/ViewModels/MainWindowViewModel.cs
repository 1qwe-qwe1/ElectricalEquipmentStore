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
        private Type _currentPageType;

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
        [ObservableProperty]
        private bool _isHeaderVisible = false;

        [ObservableProperty]
        private bool _areClientButtonsVisible = false;

        public MainWindowViewModel(string role, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var currentUser = Application.Current.Properties["CurrentUser"] as Models.User;
            CurrentUserName = $"{currentUser?.Name} {currentUser?.Surname}";

            switch (role)
            {
                case "Администратор":
                    IsAdmin = true;
                    IsEmployee = true;
                    break;
                case "Сотрудник":
                    IsEmployee = true;
                    break;
                case "Клиент":
                    IsClient = true;
                    break;
            }

            IsHeaderVisible = false;
            AreClientButtonsVisible = false;

            if (!string.IsNullOrEmpty(role))
            {
                NavigateToRolePage(role);
            }
        }

        private void NavigateToRolePage(string role)
        {
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

        public void UpdateNavigationState(Type pageType)
        {
            _currentPageType = pageType;

            IsHeaderVisible = pageType != typeof(LoginPage);

            AreClientButtonsVisible = pageType == typeof(ClientPage);

            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            CanGoBack = mainWindow?.MainFrame.CanGoBack == true;
        }

        [RelayCommand]
        private void Logout()
        {
            Application.Current.Properties.Remove("CurrentUser");
            Application.Current.Properties.Remove("UserRole");

            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                mainWindow.MainFrame.Navigate(loginPage);

                IsHeaderVisible = false;
                AreClientButtonsVisible = false;
                IsAdmin = false;
                IsEmployee = false;
                IsClient = false;
                CurrentUserName = string.Empty;
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
