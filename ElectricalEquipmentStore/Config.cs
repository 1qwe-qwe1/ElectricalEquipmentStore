using DotNetEnv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ElectricalEquipmentStore
{

    public static class Config
    {
        static Config()
        {
            try
            {
                Console.WriteLine("=== ЗАГРУЗКА КОНФИГУРАЦИИ ===");

                var envPath = ".env";
                if (!File.Exists(envPath))
                {
                    Console.WriteLine(".env файл не найден!");
                    CreateDefaultEnvFile();
                }

                Console.WriteLine("\nСодержимое .env файла (сырое):");
                var rawContent = File.ReadAllText(envPath);
                Console.WriteLine("--- НАЧАЛО ФАЙЛА ---");
                Console.WriteLine(rawContent);
                Console.WriteLine("--- КОНЕЦ ФАЙЛА ---");

                Console.WriteLine("\nКоды символов (первые 100):");
                for (int i = 0; i < Math.Min(100, rawContent.Length); i++)
                {
                    var ch = rawContent[i];
                    Console.WriteLine($"  [{i}] '{ch}' = {(int)ch} ({(ch == ' ' ? "ПРОБЕЛ" : ch == '\r' ? "CR" : ch == '\n' ? "LF" : ch.ToString())})");
                }

                Console.WriteLine("\nЗагружаем через DotNetEnv...");
                Env.Load();
                Console.WriteLine("Загружено");

                Console.WriteLine("\nПроверка загруженных значений:");
                Console.WriteLine($"DB_HOST: '{Env.GetString("DB_HOST")}' (длина: {Env.GetString("DB_HOST")?.Length})");
                Console.WriteLine($"DB_PORT: '{Env.GetString("DB_PORT")}' (длина: {Env.GetString("DB_PORT")?.Length})");
                Console.WriteLine($"DB_NAME: '{Env.GetString("DB_NAME")}' (длина: {Env.GetString("DB_NAME")?.Length})");
                Console.WriteLine($"DB_USER: '{Env.GetString("DB_USER")}' (длина: {Env.GetString("DB_USER")?.Length})");
                var pass = Env.GetString("DB_PASSWORD");
                Console.WriteLine($"DB_PASSWORD: '{new string('*', pass?.Length ?? 0)}' (длина: {pass?.Length})");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки .env: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void CreateDefaultEnvFile()
        {
            try
            {
                var defaultEnv = @"DB_HOST=localhost
DB_PORT=5432
DB_NAME=electricalEqStore
DB_USER=postgres
DB_PASSWORD=782566912";

                File.WriteAllText(".env", defaultEnv, System.Text.Encoding.UTF8);
                Console.WriteLine("Создан новый .env файл");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось создать .env: {ex.Message}");
            }
        }

        public static string ConnectionString
        {
            get
            {
                try
                {
                    var host = Env.GetString("DB_HOST", "localhost");
                    var port = Env.GetString("DB_PORT", "5432");
                    var database = Env.GetString("DB_NAME", "electrical2_clean");
                    var username = Env.GetString("DB_USER", "postgres");
                    var password = Env.GetString("DB_PASSWORD", "");

                    Console.WriteLine("=== ФОРМИРОВАНИЕ СТРОКИ ПОДКЛЮЧЕНИЯ ===");
                    Console.WriteLine($"Host: {host}");
                    Console.WriteLine($"Port: {port}");
                    Console.WriteLine($"Database: {database}");
                    Console.WriteLine($"Username: {username}");

                    var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

                    var safeConnectionString = $"Host={host};Port={port};Database={database};Username={username};Password=***";
                    Console.WriteLine($"Строка подключения: {safeConnectionString}");
                    return connectionString;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Ошибка при формировании строки подключения: {ex.Message}");
                    throw;
                }
            }
        }
    }

    public static class StringExtensions
    {
        public static string Repeat(this char c, int count)
        {
            return new string(c, Math.Max(0, count));
        }
    }

    
}
