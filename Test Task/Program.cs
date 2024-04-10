using Serilog.Sinks;
using Serilog.Events;
using System.IO;
using Serilog;

namespace Test_Task
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Запуск программы...");
                string query = SqlFunctions.ReadQueryFromFile("input.txt");
                string connectionString = SqlFunctions.ReadConnectionStringFromConfig();

                SqlFunctions.ExecuteQueryAndWriteToFile(query, connectionString);
                Log.Information("Запрос успешно выполнен. Результаты записаны в файл output.csv");
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка: {ex.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

    }
}
