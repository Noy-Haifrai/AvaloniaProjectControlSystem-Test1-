using System;
using System.Linq;
using static AvaloniaTest1.Classes.Entities;
using static AvaloniaTest1.Classes.JsonDataService;
using static AvaloniaTest1.Classes.LINQs;
using static AvaloniaTest1.Classes.Session;

namespace AvaloniaTest1.Classes
{
    //Класс типовых и основных операций программы
    public class Operations
    {
        //Инициализация шифрования, логгера и бд
        tDES tdes = new tDES();
        OperationLogger logger = new OperationLogger();
        private readonly Database _db;

        internal Operations(Database db)
        {
            _db = db;
        }

        //Создание пользователя
        public void createUser(string login, string password, string firstName, string lastName, string patronimic, int role)
        {
            // Проверяем, не зашифрован ли уже логин (на случай повторного вызова)
            string encryptedLogin = login.StartsWith("encrypted:") ? login : tdes.Encrypt(login, GetProcessorID.cpuInfo);

            if (LoginExist(_db, encryptedLogin))
            {
                Console.WriteLine("Ошибка: Введённый логин уже занят");
                return; // Прерываем выполнение метода
            }

            var newUser = new Users
            {
                Id = _db.Users.Count > 0 ? _db.Users.Max(u => u.Id) + 1 : 1,
                Login = tdes.Encrypt(login, GetProcessorID.cpuInfo),
                Password = tdes.Encrypt(password, GetProcessorID.cpuInfo),
                FirstName = firstName,
                LastName = lastName,
                Patronimic = patronimic,
                Role = role
            };

            _db.Users.Add(newUser);
            SaveData(_db);
            Console.WriteLine("Сотрудник успешно зарегистрирован!");

            // Логирование (если logger доступен)
            logger?.Log($"Создан новый пользователь: {firstName} {patronimic} (ID: {newUser.Id}, Роль: {(role == 1 ? "Управляющий" : "Сотрудник")})");
        }
        //Удаление пользователя
        public void deleteUserByID(int id)
        {
            LoadData();
            DeleteUser(_db, id);
            SaveData(_db);
            logger.Log($"{userFirstName} {userPatronimic} ({userRole}) удалил пользователя. ID удалённого пользователя - {id}");
        }
        //Создание задачи
        public void createTask(string name, string description, int employeeID)
        {
            LoadData();
            var newId = _db.Projects.Count > 0 ? _db.Projects.Max(p => p.Id) + 1 : 1;

            var newProject = new Projects
            {
                Id = newId,
                Name = name,
                Description = description,
                Status = 1,
                AssignedEmployeeID = employeeID
            };

            _db.Projects.Add(newProject);
            SaveData(_db);
            logger.Log($"{userFirstName} {userPatronimic} ({userRole}) создал новую задачу. ID - {newId} '{newProject.Name}'");
        }
        //Обновление статуса задачи
        public void updateProjectStatus(int id, int status)
        {
            LoadData();
            // Находим задачу по ID
            var project = _db.Projects.FirstOrDefault(p => p.Id == id);

            if (project != null)
            {
                logger.Log($"{userFirstName} {userPatronimic} ({userRole}) Обновил статус задачи. ID задачи - {id}. Статус: {project.Status} --> {status}");
                // Обновляем статус
                project.Status = status;
                // Сохраняем изменения
                SaveData(_db);
            }
            else
            {
                throw new ArgumentException($"Задача с ID {id} не найдена");
            }

        }
        //Тповой вывод задачи в зависимости от роли
        internal void PrintTaskDetails(Projects project)
        {
            var employee = LINQs.ViewEmployees(_db).FirstOrDefault(e => e.Id == project.AssignedEmployeeID);
            Console.WriteLine($"ID: {project.Id}");
            Console.WriteLine($"Название: {project.Name}");
            Console.WriteLine($"Описание: {project.Description}");
            Console.WriteLine($"Статус: {GetStatusName(project.Status)}");
            if (Session.userRoleID == 1)
                Console.WriteLine($"Исполнитель: {employee?.LastName} {employee?.FirstName}");
            Console.WriteLine("-----------------------");
        }
        //Конвертация номера статуса из бд в текст
        internal string GetStatusName(int status)
        {
            return status switch
            {
                1 => "Назначен",
                2 => "Выполняется",
                3 => "Завершено",
                _ => "Неизвестный статус"
            };
        }
    }
}
