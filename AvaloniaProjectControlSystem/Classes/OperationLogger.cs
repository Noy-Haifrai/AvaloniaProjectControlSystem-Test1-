using System;
using System.IO;

namespace AvaloniaTest1.Classes
{
    public class OperationLogger
    {

        private readonly string _filePath;

        public OperationLogger(string fileName = "log.txt")
        {
            string projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(projectRoot, fileName);
        }

        public void Log(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_filePath, true))
                {
                    writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm} - {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в лог: {ex.Message}");
            }
        }

    }
}
