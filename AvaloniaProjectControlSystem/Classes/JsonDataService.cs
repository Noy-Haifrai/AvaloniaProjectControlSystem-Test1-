using System.IO;
using System.Text.Json;
using static AvaloniaTest1.Classes.Entities;

namespace AvaloniaTest1.Classes
{
    //Чтение и запись json файла
    public static class JsonDataService
    {
        private static string filePath = "data.json";

        internal static Database LoadData()
        {
            if (!File.Exists(filePath))
                return new Database();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Database>(json) ?? new Database();
        }
        internal static void SaveData(Database data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, json);
        }
    }
}