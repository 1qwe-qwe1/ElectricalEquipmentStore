using ElectricalEquipmentStore.Data;
using ElectricalEquipmentStore.Pages;
using ElectricalEquipmentStore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ElectricalEquipmentStore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public static NavigationService NavigationService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Console.WriteLine("=== ЗАПУСК ПРИЛОЖЕНИЯ ===");

                // 1. Проверяем подключение к БД
                if (!CheckDatabaseConnection())
                {
                    Shutdown();
                    return;
                }

                // 2. Настройка DI контейнера
                var services = new ServiceCollection();
                ConfigureServices(services);
                ServiceProvider = services.BuildServiceProvider();

                // 3. Создаем и показываем главное окно
                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

                // 4. Инициализируем NavigationService
                NavigationService = new NavigationService(mainWindow.MainFrame);

                // 5. Загружаем стартовую страницу
                NavigationService.NavigateTo(new LoginPage());

                Console.WriteLine("✅ Приложение успешно запущено");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска приложения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(Config.ConnectionString),
                ServiceLifetime.Transient);

            // Сервисы
            services.AddTransient<AuthService>();

            // Окна и страницы
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginPage>();
            services.AddTransient<AdminPage>();
            services.AddTransient<EmployeePage>();
            services.AddTransient<ClientPage>();

            // ViewModels
            services.AddTransient<ViewModels.LoginViewModel>();
        }

        private bool CheckDatabaseConnection()
        {
            try
            {
                Console.WriteLine("\n=== ПРОВЕРКА ПОДКЛЮЧЕНИЯ К БАЗЕ ДАННЫХ ===");

                var connectionString = Config.ConnectionString;
                Console.WriteLine($"Используем строку подключения: {GetSafeConnectionString(connectionString)}");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    MessageBox.Show("Строка подключения к БД пустая!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                Console.WriteLine("Пытаемся подключиться к PostgreSQL...");
                using var connection = new NpgsqlConnection(connectionString);

                try
                {
                    connection.Open();
                    Console.WriteLine("✅ Подключение к PostgreSQL установлено");

                    using var cmd = new NpgsqlCommand("SELECT version();", connection);
                    var version = cmd.ExecuteScalar()?.ToString();
                    Console.WriteLine($"Версия PostgreSQL: {version}");

                    using var cmd2 = new NpgsqlCommand("SELECT current_database();", connection);
                    var dbName = cmd2.ExecuteScalar()?.ToString();
                    Console.WriteLine($"Текущая база данных: {dbName}");

                    using var cmd3 = new NpgsqlCommand(
                        "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';",
                        connection);
                    using var reader = cmd3.ExecuteReader();

                    Console.WriteLine("Таблицы в базе данных:");
                    int tableCount = 0;
                    while (reader.Read())
                    {
                        Console.WriteLine($"  - {reader.GetString(0)}");
                        tableCount++;
                    }
                    Console.WriteLine($"Всего таблиц: {tableCount}");

                    connection.Close();
                    return true;
                }
                catch (NpgsqlException ex)
                {
                    Console.WriteLine($"❌ Ошибка PostgreSQL: {ex.Message}");

                    var errorMessage = $"Не удалось подключиться к базе данных:\n\n{ex.Message}\n\n" +
                                     $"Проверьте:\n" +
                                     $"1. Запущен ли сервер PostgreSQL\n" +
                                     $"2. Правильный ли пароль в .env файле\n" +
                                     $"3. Существует ли база данных 'electrical2_clean'\n\n" +
                                     $"Строка подключения: {GetSafeConnectionString(connectionString)}";

                    MessageBox.Show(errorMessage,
                        "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Непредвиденная ошибка: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                MessageBox.Show($"Непредвиденная ошибка при проверке подключения:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private string GetSafeConnectionString(string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString))
                    return "пустая строка";

                var startIndex = connectionString.IndexOf("Password=", StringComparison.OrdinalIgnoreCase);
                if (startIndex >= 0)
                {
                    var endIndex = connectionString.IndexOf(';', startIndex);
                    if (endIndex < 0) endIndex = connectionString.Length;

                    var beforePassword = connectionString.Substring(0, startIndex + 9);
                    var afterPassword = connectionString.Substring(endIndex);

                    return beforePassword + "***" + afterPassword;
                }

                return connectionString;
            }
            catch
            {
                return "не удалось обработать строку подключения";
            }
        }
    }

}
