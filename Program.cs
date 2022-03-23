using System;

namespace MySqlEntityCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Connection(
                user: "webserver",
                password: "vb!Lck3fCRED"  // Me lazy on my local debug server ;)
            );
            new Init(dropColumn: true);
            Console.WriteLine("Done.");
        }
    }
}
