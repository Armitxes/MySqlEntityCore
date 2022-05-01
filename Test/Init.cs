#if DEBUG
using System;

namespace MySqlEntityCore.Test
{
    public class Init
    {
        public void Start()
        {
            InitEntityCore();
            Test.Contact.Test();
            Test.User.Test();
        }

        private void InitEntityCore()
        {
            Console.Write("[TEST] Initialize MySQL Entity Core: ");
            new MySqlEntityCore.Init(dropColumn: true);
            Console.WriteLine(true);
        }
    }
}
#endif
