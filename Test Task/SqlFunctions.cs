using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Serilog;

namespace Test_Task
{
    internal class SqlFunctions
    {
        public static string ReadConnectionStringFromConfig()
        {
            string connectionString = "";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            connectionString = connectionString.Replace("|DataDirectory|", baseDirectory);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException("Строка подключения не найдена в файле конфигурации.");
            }

            return connectionString;
        }

        public static string ReadQueryFromFile(string filePath)
        {
            string query = "";

            if (File.Exists(filePath))
            {
                query = File.ReadAllText(filePath);
            }
            else
            {
                throw new FileNotFoundException("Файл с SQL запросом не найден");
            }

            return query;
        }

        public static void ExecuteQueryAndWriteToFile(string query, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    Log.Debug($"Выполнение SQL запроса: {query}");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        using (StreamWriter writer = new StreamWriter("output.csv"))
                        {
                            // Запись заголовков столбцов таблицы
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                writer.Write(reader.GetName(i));
                                if (i < reader.FieldCount - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();

                            //Запись данных из таблицы
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    writer.Write(reader[i]);
                                    if (i < reader.FieldCount - 1)
                                        writer.Write(",");
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }
        }
    }
}
