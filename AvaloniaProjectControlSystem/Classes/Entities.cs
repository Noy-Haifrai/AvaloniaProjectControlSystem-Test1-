using System.Collections.Generic;

namespace AvaloniaTest1.Classes
{
    //Сущности для JSON файла
    class Entities
    {
        public class Users
        {
            public int Id { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Patronimic { get; set; }
            public int Role { get; set; }
        }

        public class Projects
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Status { get; set; }
            public int AssignedEmployeeID { get; set; }
        }


        public class Database
        {
            public List<Users> Users { get; set; } = new List<Users>();
            public List<Projects> Projects { get; set; } = new List<Projects>();

        }
    }
}
