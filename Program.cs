using System;

namespace MySqlEntityCore {
    public class Program
    {
        public static void Main(string[] args)
        {
            new Connection(
                user: "web"
            );
            new Init();
            Console.WriteLine("Done.");
        }
    }
}
