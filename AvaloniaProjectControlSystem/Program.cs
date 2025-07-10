using AvaloniaTest1.Classes;
using System;
using System.Linq;
using System.Threading;
using static AvaloniaTest1.Classes.Entities;
using static AvaloniaTest1.Classes.Session;
using static AvaloniaTest1.Classes.Operations;

namespace AvaloniaTest1
{
    class Program
    {
        //Инициализация шифрования, основных операций и БД
        private static readonly tDES tdes = new tDES();
        public static Database db;
        private static Operations operations;

        //Точка входа
        static void Main(string[] args)
        {
            //Инициализация логгирования
            var logger = new OperationLogger();
            // Основной цикл первичного этапа работы программы
            while (true) 
            {
                db = JsonDataService.LoadData();
                operations = new Operations(db);

                // Цикл авторизации
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Введите логин, а затем пароль (или Esc для выхода)");

                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("\nВыход из программы...");
                        return; // Полный выход из программы
                    }
                    if (!AuthenticateUser())
                    {
                        Console.WriteLine("\nОшибка: Неверный логин или пароль! Повторите попытку");
                        Thread.Sleep(3000); // Пауза для чтения сообщения
                        continue;
                    }
                    logger.Log($"{userFirstName} {userPatronimic} вошёл в систему");
                    break;
                }
                Console.Clear();
                Console.WriteLine($"\nДобро пожаловать, {userFirstName} {userLastName}!");
                Console.WriteLine($"Ваша роль: {userRole}");

                // Основное меню
                ShowMainMenu();
            }
        }
        //Авторизация(проверка логина и пароля) и Аутентификкация(установление личности пользователя для логгирования) пользователя
        private static bool AuthenticateUser()
        {

            Console.Write("Логин: ");
            string login = tdes.Encrypt(Console.ReadLine(), GetProcessorID.cpuInfo);
            Console.Write("Пароль: ");
            string password = tdes.Encrypt(Console.ReadLine(), GetProcessorID.cpuInfo);

            return LINQs.Authenticate(db, login, password);
        }

        //Главное меню с опциями в зависимости от роли
        private static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("\nГлавное меню:");
                Console.WriteLine("1. Просмотр активных задач");

                if (userRoleID == 1) // Управляющий
                {
                    Console.WriteLine("2. Создать задачу");
                    Console.WriteLine("3. Зарегистрировать сотрудника");
                    Console.WriteLine("4. Просмотр сотрудников");

                }

                else if (userRoleID == 0) //root
                {
                    Console.WriteLine("2. Создать задачу");
                    Console.WriteLine("3. Зарегистрировать сотрудника");
                    Console.WriteLine("4. Просмотр сотрудников");
                    Console.WriteLine("5. Удалить сотрудника");
                }
                else  // Сотрудник
                {
                    Console.WriteLine("2. Изменить статус задачи");
                }
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                var choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Clear();
                            if (userRoleID != 2)
                                ViewTasks();
                            else 
                                TaskEmployeeMenu();
                            break;
                        case "2":
                            Console.Clear();
                            if (userRoleID != 2)
                                CreateTask();
                            else
                                UpdateTaskStatus();
                            break;
                        case "3" when userRoleID != 2:
                            Console.Clear();
                            RegisterEmployee();
                            break;
                        case "4" when userRoleID != 2:
                            Console.Clear();
                            ViewAllEmployees();
                            break;
                        case "5" when userRoleID == 0:
                            Console.Clear();
                            DeleteEmployee();
                            break;
                        case "0":
                            Console.Clear();
                            return;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }
        //Список задач для управляющего
        private static void ViewTasks()
        {
            try
            {
                db = JsonDataService.LoadData();
                var allProjects = LINQs.ViewAllProjets(db);
                    Console.WriteLine("\nВсе задачи:");

                    if (!allProjects.Any())
                    {
                        Console.WriteLine("Нет задач.");
                        return;
                    }
                    var activeTasks = allProjects.Where(p => p.Status != 3).ToList();
                    var completedTasks = allProjects.Where(p => p.Status == 3).ToList();
                    if (activeTasks.Any())
                    {
                        Console.WriteLine("\nАктивные задачи:");
                        foreach (var project in activeTasks)
                        {
                            operations.PrintTaskDetails(project);
                        }
                    }
                    if (completedTasks.Any())
                    {
                        Console.WriteLine($"\nЗавершённые задачи ({completedTasks.Count}):");
                        foreach (var project in completedTasks)
                        {
                            operations.PrintTaskDetails(project);
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
            }
        }
        //Список задач сотрудника с выводом по различным параметрам
        private static void TaskEmployeeMenu()
        {
            try
            {
                Console.WriteLine("1. Все активные задачи");
                Console.WriteLine("2. Назначенные задачи");
                Console.WriteLine("3. Задачи 'В работе'");
                Console.WriteLine("4. Завершённые задачи");
                Console.WriteLine("0. Вернуться в меню");
                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Clear();
                            var userProjects = LINQs.ViewProjects(db); // Только задачи сотрудника
                            var activeTasks = userProjects.Where(p => p.Status != 3).ToList();
                            var completedCount = userProjects.Count(p => p.Status == 3);
                            Console.WriteLine("\nВаши активные задачи:");
                            if (!activeTasks.Any() && completedCount == 0)
                            {
                                Console.WriteLine("У вас нет задач.");
                                return;
                            }
                            else
                            {
                                foreach (var project in activeTasks)
                                {
                                    operations.PrintTaskDetails(project);
                                }
                            }
                            break;
                        case "2":
                            Console.Clear();
                            Console.WriteLine("\n Назначенные задачи\n");
                            var toWork = LINQs.ToWorkTasks(db);
                            foreach (var project in toWork)
                            {
                                operations.PrintTaskDetails(project);
                            }

                            break;
                        case "3":
                            Console.Clear();
                            Console.WriteLine("\n Задачи в работе\n");
                            var inProgress = LINQs.InProgressTasks(db);
                            foreach (var project in inProgress)
                            {
                                operations.PrintTaskDetails(project);
                            }
                            break;
                        case "4":
                            Console.Clear();
                            Console.WriteLine("\n Завершённые задачи\n");
                            var Completed = LINQs.CompletedTasks(db);
                            foreach (var project in Completed)
                            {
                                operations.PrintTaskDetails(project);
                            }
                            break;
                        case "0":
                            Console.Clear();
                            return;
                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
            }
        }
   
        //Создание новой задачи
        private static void CreateTask()
        {
            try
            {
                Console.WriteLine("\nСоздание новой задачи:");
                Console.Write("Название: ");
                string name = Console.ReadLine();
                Console.Write("Описание: ");
                string description = Console.ReadLine();

                ViewEmployees();
                Console.Write("ID сотрудника для назначения задачи: ");
                if (!int.TryParse(Console.ReadLine(), out int employeeId))
                {
                    Console.WriteLine("Ошибка: Некорректный ID сотрудника");
                    return;
                }

                operations.createTask(name, description, employeeId);
                db = JsonDataService.LoadData();
                Console.WriteLine("\nЗадача успешно создана!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании задачи: {ex.Message}");
            }
        }
        //Обновление статуса задачи
        private static void UpdateTaskStatus()
        {
            try
            {
                var userProjects = LINQs.ViewProjects(db);
                var activeTasks = userProjects.Where(p => p.Status != 3).ToList();
                Console.WriteLine("\nВаши активные задачи:");

                if (!activeTasks.Any())
                {
                    Console.WriteLine("Нет активных задач для изменения статуса.");
                }
                else
                {
                    foreach (var project in activeTasks)
                    {
                        Console.WriteLine($"{project.Id}: {project.Name} - {project.Description} | Текущий статус: {operations.GetStatusName(project.Status)}");
                    }
                }

                if (activeTasks.Any())
                {
                    Console.Write("\nВыберите ID задачи для изменения статуса (или 0 для отмены): ");
                    if (!int.TryParse(Console.ReadLine(), out int taskId) || taskId == 0)
                    {
                        return;
                    }

                    // Получаем задачу из базы данных
                    var task = db.Projects.FirstOrDefault(p => p.Id == taskId);

                    
                    if (task == null || ( task.AssignedEmployeeID != userID))
                    {
                        Console.WriteLine("Ошибка: Вы не можете изменить эту задачу");
                        return;
                    }

                    Console.WriteLine("Выберите новый статус:");
                    Console.WriteLine("1. Назначен");
                    Console.WriteLine("2. Выполняется");
                    Console.WriteLine("3. Завершено");
                    Console.Write("Ваш выбор: ");
                    if (!int.TryParse(Console.ReadLine(), out int newStatus) || newStatus < 1 || newStatus > 3)
                    {
                        Console.WriteLine("Ошибка: Некорректный статус");
                        return;
                    }

                    operations.updateProjectStatus(taskId, newStatus);
                    db = JsonDataService.LoadData();
                    Console.WriteLine("Статус задачи успешно обновлен!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        //Список всех сотрудников вне зависимости от их ролей
        private static void ViewAllEmployees()
        {
            try
            {
                var employees = LINQs.ViewAllEmployees(db);
                Console.WriteLine("\nСписок сотрудников:");
                foreach (var employee in employees)
                {
                    string role = employee.Role == 1 ? "Управляющий" : "Сотрудник";
                    Console.WriteLine($"{employee.Id}: {employee.LastName} {employee.FirstName} {employee.Patronimic} ({role})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        //Список всех сотрудников с ролью "сотрудник"
        private static void ViewEmployees()
        {
            try
            {
                var employees = LINQs.ViewEmployees(db);
                Console.WriteLine("\nСписок сотрудников:");
                foreach (var employee in employees)
                {
                    string role = "Сотрудник";
                    Console.WriteLine($"{employee.Id}: {employee.LastName} {employee.FirstName} {employee.Patronimic} ({role})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        //Регистрация нового сотрудника.  
        private static void RegisterEmployee()
        {
            try
            {
                Console.WriteLine("\nРегистрация нового сотрудника:");

                Console.Write("Логин: ");
                string login = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(login))
                {
                    Console.WriteLine("Логин не может быть пустым!");
                    return;
                }

                Console.Write("Пароль: ");
                string password = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Пароль не может быть пустым!");
                    return;
                }

                Console.Write("Фамилия: ");
                string lastName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    Console.WriteLine("Фамилия не может быть пустой!");
                    return;
                }

                Console.Write("Имя: ");
                string firstName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    Console.WriteLine("Имя не может быть пустым!");
                    return;
                }

                Console.Write("Отчество: ");
                string patronymic = Console.ReadLine();

                Console.Write("Роль (1 - Управляющий, 2 - Сотрудник): ");
                int role = Convert.ToInt32(Console.ReadLine());
                if (role < 1 || role > 2)
                {
                    Console.WriteLine("Роль не может быть пустой или иметь значение, отличное от 1 или 2!");
                    return;
                }
                // Пробуем создать пользователя
                operations.createUser(login, password, firstName, lastName, patronymic, role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        //Удаление сотрудника
        private static void DeleteEmployee()
        {
            try
            {
                ViewAllEmployees();
                Console.Write("Введите ID сотрудника для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Ошибка: Некорректный ID");
                    return;
                }

                operations.deleteUserByID(id);
                db = JsonDataService.LoadData();
                Console.WriteLine("Сотрудник успешно удален!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        } 
    }
}