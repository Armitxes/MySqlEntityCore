using System;

namespace MySqlEntityCore {
    public class Program
    {
        public static void Main(string[] args)
        {
            new Connection(
                user: "webserver",
                password: "vb!Lck3fCRED"
            );
            new Init();
            Console.WriteLine("Done.");
        }
    }
}
