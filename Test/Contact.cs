using System;

namespace MySqlEntityCore.Test
{
    [Model]
    public class Contact : MySqlEntityCore.Template.DefaultModel
    {

        [Field(Size = 64)]
        public string Firstname { get; set; }

        [Field(Size = 64)]
        public string Name { get; set; }

        [Field(Size = 128)]
        public string EMail { get; set; }

        [Field]
        public DateTime Birthday { get; set; }

        [Field]
        public uint Income { get; set; }

        public Contact() : base() { }

        public Contact(uint id) : base(id) { }

        public static void Test()
        {
            TestCreate();
            TestWriteCached();
            TestWriteUncached();
        }

        private static void TestCreate()
        {
            Contact contact = new Contact()
            {
                EMail = "test@domain.tld",
                Birthday = DateTime.Parse("1990-01-03 00:00:00"),
                Income = 1000,
            };
            Console.WriteLine("[TEST] Creating simple default record with null values.");
            contact.Create();

            if (contact.Id != 1)  // We are on a new DB with no records. Must be 1.
                throw new SystemException(
                    "[TEST] Contact create test failed."
                );
        }

        public static void TestWriteCached()
        {
            Console.WriteLine("[TEST] Write existing contact (cached).");
            Contact contact = new Contact(1);


            contact.EMail = "test2@domain.tld";
            contact.Birthday = DateTime.Parse("1990-01-04 00:00:00");
            contact.Income = 2000;
            contact.Write();

            if (
                contact.Id != 1
                || contact.EMail != "test2@domain.tld"
                || contact.Birthday != DateTime.Parse("1990-01-04 00:00:00")
                || contact.Income != 2000
            )
                throw new SystemException(
                    "[TEST] Writing contact retrieved from cache failed."
                );
        }

        public static void TestWriteUncached()
        {
            Console.Write("[TEST] Write existing contact (uncached): ");
            Cache.Remove("Contact.1");
            Contact contact = new Contact(1);

            if (
                contact.Id != 1
                || contact.EMail != "test2@domain.tld"
                || contact.Birthday != DateTime.Parse("1990-01-04 00:00:00")
                || contact.Income != 2000
            )
                throw new SystemException(
                    "[TEST] Inconsistent cache after writing contact to DB."
                );
        }

    }
}
