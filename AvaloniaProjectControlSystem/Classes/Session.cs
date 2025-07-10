namespace AvaloniaTest1.Classes
{
    //Нужен чтобы постоянно не обращаться к бд при логгировании
    class Session
    {
        public static string userFirstName, userLastName, userPatronimic, userRole;
        public static int userID, userRoleID;
    }
}
