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

        public DataType() : base() { }

        public DataType(uint id) : base(id) { }

        public static bool Test()
        {
            Console.WriteLine("[TEST] Testing data types.");
            return (
                TestCreate()
                && TestGet()
            );
        }

        private static bool TestCreate()
        {
            DataType record = new DataType();
            record.Create();
            if (record.TestBoolNull)
                throw new SystemException(
                    "[TEST] Property TestBoolNull is true."
                );

            if (!record.TestBoolTrue)
                throw new SystemException(
                    "[TEST] Property TestBoolTrue is false."
                );

            if (record.TestBoolFalse)
                throw new SystemException(
                    "[TEST] Property TestBoolFalse is true."
                );
            return record.Id == 1;
        }

        private static bool TestGet()
        {
            DataType record = new DataType(1);
            if (record.TestBoolNull)
                throw new SystemException(
                    "[TEST] Property TestBoolNull is true."
                );

            if (!record.TestBoolTrue)
                throw new SystemException(
                    "[TEST] Property TestBoolTrue is false."
                );

            if (record.TestBoolFalse)
                throw new SystemException(
                    "[TEST] Property TestBoolFalse is true."
                );
            return record.Id == 1;
        }

    }
}
