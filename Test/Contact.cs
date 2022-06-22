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

        public static bool TestCreate()
        {
            Contact contact = new Contact()
            {
                EMail = "test@domain.tld",
                Birthday = DateTime.Parse("1990-01-03 00:00:00"),
                Income = 1000,
            };
            Console.Write("[TEST] Creating simple default record with null values: ");
            contact.Create();

            bool success = contact.Id == 1;  // We are on a new DB with no records. Must be 1.
            Console.WriteLine(success);
            return success;
        }

        public static bool TestWriteCached()
        {
            Console.Write("[TEST] Write existing contact (cached): ");
            Contact contact = new Contact(1);


            contact.EMail = "test2@domain.tld";
            contact.Birthday = DateTime.Parse("1990-01-04 00:00:00");
            contact.Income = 2000;
            contact.Write();

            bool success = (
                contact.Id == 1
                && contact.EMail == "test2@domain.tld"
                && contact.Birthday == DateTime.Parse("1990-01-04 00:00:00")
                && contact.Income == 2000
            );
            Console.WriteLine(success);
            return success;
        }

        public static bool TestWriteUncached()
        {
            Console.Write("[TEST] Write existing contact (uncached): ");
            Cache.Remove("Contact.1");
            Contact contact = new Contact(1);

            bool success = (
                contact.Id == 1
                && contact.EMail == "test2@domain.tld"
                && contact.Birthday == DateTime.Parse("1990-01-04 00:00:00")
                && contact.Income == 2000
            );
            Console.WriteLine(success);

            return success;
        }

    }
}
