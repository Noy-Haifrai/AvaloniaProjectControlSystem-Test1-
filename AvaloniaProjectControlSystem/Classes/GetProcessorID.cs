using System.Management;

namespace AvaloniaTest1.Classes
{
    //Серийный номер процессора, который будет использоваться как уникальный ключь шифрования.
    public static class GetProcessorID
    {
        public static string cpuInfo { get; }
        static GetProcessorID()
        {
            // Автоматически получаем ID процессора при первом обращении
            using (var mc = new ManagementClass("win32_processor"))
            using (var moc = mc.GetInstances())
            {
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["processorID"].Value?.ToString();
                    break;
                }
            }
        }
    }
}
