using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectricalEquipmentStore.Models;
using ElectricalEquipmentStore.Pages;
using ElectricalEquipmentStore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace ElectricalEquipmentStore.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        // Уберите NavigationService из конструктора

        [ObservableProperty]
        private string _userLogin = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isLoading = false;

        // Конструктор с одним параметром
        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task PerformLogin()
        {
            string login = UserLogin;
            string password = Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Введите логин и пароль";
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await Task.Run(() => _authService.Authenticate(login, password));

                if (user != null)
                {
                    // Успешная авторизация
                    var role = _authService.GetUserRole(user);

                    // Сохраняем информацию о текущем пользователе
                    Application.Current.Properties["CurrentUser"] = user;
                    Application.Current.Properties["UserRole"] = role;

                    // Навигация на главную страницу в зависимости от роли
                    NavigateToMainPage(role);
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка авторизации: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NavigateToMainPage(string role)
        {
            // Используем статический NavigationService из App
            if (Application.Current is App app && App.NavigationService != null)
            {
                switch (role)
                {
                    case "Администратор":
                        App.NavigationService.NavigateTo(new AdminPage());
                        break;
                    case "Сотрудник":
                        App.NavigationService.NavigateTo(new EmployeePage());
                        break;
                    case "Клиент":
                        App.NavigationService.NavigateTo(new ClientPage());
                        break;
                    default:
                        App.NavigationService.NavigateTo(new ClientPage());
                        break;
                }
            }
        }
    }
}

