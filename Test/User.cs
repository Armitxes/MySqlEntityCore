using System;

namespace MySqlEntityCore.Test
{
    [Model]
    public class User : MySqlEntityCore.Template.DefaultModel
    {
        [Field(Size = 45, Unique = true, Required = true)]
        public string Username { get; set; }

        [Field(Size = 64)]
        public string Password { get; set; }

        public static bool Test()
        {
            User user = new User();
            user.Username = "User1";
            user.Password = "12345";
            Console.Write("[TEST] Creating simple default record: ");
            user.Create();

            TableInfo.Get("mysqlentitycore");

            bool success = user.Id == 1;  // We are on a new DB with no records. Must be 1.
            Console.WriteLine(success);
            return success;
        }
    }
}
