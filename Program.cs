#if DEBUG
using System;

namespace MySqlEntityCore
{
    // Only available if not release build.
    public class Program
    {


        public static void Main(string[] args)
        {
            Console.Write("[TEST] Establishing initial connection: ");
            new Connection(
                database: "mysqlentitycore",
                user: "webserver",
                password: "vb!Lck3fCRED"  // Me lazy on my local debug server ;)
            ).RecreateDatabase();
            Console.WriteLine(true);
            new MySqlEntityCore.Test.Init().Start();
        }
    }
}
#endif
