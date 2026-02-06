using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectricalEquipmentStore.Models;
using ElectricalEquipmentStore.Pages;
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

        public MainWindowViewModel(string role)
        {
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
            switch (role)
            {
                case "Администратор":
                    // Показываем страницу админа
                    // Application.Current.MainWindow.MainFrame.Navigate(new AdminPage());
                    break;
                case "Сотрудник":
                    // Показываем страницу сотрудника
                    break;
                case "Клиент":
                    // Показываем страницу клиента
                    break;
            }
        }

        [RelayCommand]
        private void Logout()
        {
            // Очищаем данные пользователя
            Application.Current.Properties.Remove("CurrentUser");
            Application.Current.Properties.Remove("UserRole");


            App.NavigationService?.NavigateTo(new LoginPage());
        }
        

        // Команды навигации (пока заглушки)
        [RelayCommand]
        private void NavigateToCatalog() { }

        [RelayCommand]
        private void NavigateToOrders() { }

        [RelayCommand]
        private void NavigateToCart() { }

        [RelayCommand]
        private void NavigateToAdmin() { }

        [RelayCommand]
        private void NavigateToOrderManagement() { }

        [RelayCommand]
        private void NavigateToSupplies() { }

        [RelayCommand]
        private void GoBack() { }
    }
}
