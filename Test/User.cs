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

        public User() : base() { }
        public User(uint id) : base(id) { }

        public static void Test()
        {
            TestCreate();
            TestTableInfo();
        }

        private static void TestCreate()
        {
            Console.WriteLine("[TEST] Creating simple default record.");
            User user = new User(1);
            user.Username = "User1";
            user.Password = "12345";
            user.Create();

            if (user.Id != 1)
                throw new SystemException(
                    "[TEST] User create test failed."
                );
        }

        private static void TestTableInfo()
        {
            Console.WriteLine("[TEST] User table info.");
            TableInfo.Get("mysqlentitycore");
        }

    }
}
