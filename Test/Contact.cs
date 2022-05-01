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

        public static bool Test()
        {
            Contact contact = new Contact()
            {
                EMail = "test@domain.tld",
                Birthday = DateTime.Parse("1990-01-03 00:00:00"),
            };
            Console.Write("[TEST] Creating simple default record with null values: ");
            contact.Create();

            bool success = contact.Id == 1;  // We are on a new DB with no records. Must be 1.
            Console.WriteLine(success);
            return success;
        }

    }
}
