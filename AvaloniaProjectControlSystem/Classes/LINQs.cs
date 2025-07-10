using System;
using System.Collections.Generic;
using System.Linq;
using static AvaloniaTest1.Classes.Entities;
using static AvaloniaTest1.Classes.Session;

namespace AvaloniaTest1.Classes
{
    //линк запросы
    internal class LINQs
    {
        //Авторизация и аутентификация пользователя. Заполнение основной информации о пользователи сессии
        public static bool Authenticate(Database db, string login, string password)
        {
            tDES tdes = new tDES();
            var user = db.Users.FirstOrDefault(u => (u.Login == login && u.Password == password));

            if (user != null)
            {
                userID = user.Id;
                userFirstName = user.FirstName;
                userLastName = user.LastName;
                userPatronimic = user.Patronimic;
                userRoleID = user.Role;
                if (user.Role == 1) { userRole = "Управляющий"; } else { userRole = "Сотрудник"; }

                return true;
            }

            return false;
        }
        //Проверяем, свободен ли логин
        public static bool LoginExist(Database db, string login)
        {
            return db.Users.Any(u => u.Login == login);
        }
        //Список ВСЕХ сотрудников
        public static List<Users> ViewAllEmployees(Database db)
        {
            return db.Users.Where(u => u.Role!=0).ToList();
        }
        //Список работников
        public static List<Users> ViewEmployees(Database db)
        {
            return db.Users.Where(u => u.Role == 2).ToList();
        }

        //Поиск сотрудника по ID
        public static Users GetEmployeeById(Database db, int employeeId)
        {
            return db.Users.FirstOrDefault(e => e.Id == employeeId);
        }
        //Получение списка задач по ID пользователя из Session)
        public static List<Projects> ViewProjects(Database db)
        {
            return db.Projects.Where(t => t.AssignedEmployeeID == userID).ToList();
        }

        //Удаление пользователя по ID
        public static bool DeleteUser(Database db, int id)
        { db.Users.Remove(db.Users.FirstOrDefault(u => u.Id == id)); return true; }

        //Список ВСЕХ задач
        public static List<Projects> ViewAllProjets(Database db)
        {
            return db.Projects.ToList();
        }
        //Список задач "Назначено"
        public static List<Projects> ToWorkTasks(Database db, int? employeeId = null)
        {
            var query = db.Projects.Where(p => p.Status == 1);
            if (employeeId.HasValue)
            {
                query = query.Where(p => p.AssignedEmployeeID == employeeId.Value);
            }
            return query.ToList();
        }
        //Список задач "Выполняется"
        public static List<Projects> InProgressTasks(Database db, int? employeeId = null)
        {
            var query = db.Projects.Where(p => p.Status == 2);
            if (employeeId.HasValue)
            {
                query = query.Where(p => p.AssignedEmployeeID == employeeId.Value);
            }
            return query.ToList();
        }
        //Список задач "Выполнено"
        public static List<Projects> CompletedTasks(Database db, int? employeeId = null)
        {
            var query = db.Projects.Where(p => p.Status == 3);
            if (employeeId.HasValue)
            {
                query = query.Where(p => p.AssignedEmployeeID == employeeId.Value);
            }
            return query.ToList();
        }
        //Список всех задач, у которых нет статуса "Выполнено"
        public static List<Projects> GetActiveProjects(Database db, int? employeeId = null)
        {
            var query = db.Projects.Where(p => p.Status != 3);
            if (employeeId.HasValue)
            {
                query = query.Where(p => p.AssignedEmployeeID == employeeId.Value);
            }
            return query.ToList();
        }
        //Счётчик выполненых задач
        public static int GetCompletedTasksCount(Database db, int? employeeId = null)
        {
            var query = db.Projects.Where(p => p.Status == 3);
            if (employeeId.HasValue)
            {
                query = query.Where(p => p.AssignedEmployeeID == employeeId.Value);
            }
            return query.Count();
        }   
    }
}
