using System;

namespace MySqlEntityCore.Test
{
    [Model]
    public class DataType : MySqlEntityCore.Template.DefaultModel
    {

        [Field]
        public int TestIntNull { get; set; }

        [Field]
        public int TestInt { get; set; } = 1;

        [Field]
        public bool TestBoolNull { get; set; }  // Should convert to false

        [Field]
        public bool TestBoolTrue { get; set; } = true;

        [Field]
        public bool TestBoolFalse { get; set; } = false;

        [Field(Size = 45)]
        public string TestString { get; set; } = "NotNull";

        [Field]
        public DateTime TestDateTime { get; set; }

        public static bool Test()
        {
            DataType record = new DataType();
            Console.Write("[TEST] Testing data types: ");
            record.Create();

            bool success = record.Id == 1;  // We are on a new DB with no records. Must be 1.
            Console.WriteLine(success);
            return success;
        }

    }
}
