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

        public static void Test()
        {
            Console.WriteLine("[TEST] Testing data types.");
            TestCreate();
            TestGet();
        }

        private static void TestCreate()
        {
            DataType record = new DataType();
            record.Create();
            if (record.Id != 1)
                throw new SystemException(
                    "[TEST] DataType create test failed."
                );

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
        }

        private static void TestGet()
        {
            DataType record = new DataType(1);

            if (record.Id != 1)
                throw new SystemException(
                    "[TEST] DataType get test failed."
                );

            if (record.TestBoolNull)
                throw new SystemException(
                    "[TEST] Property TestBoolNull is true."  // null values should be considered false.
                );

            if (!record.TestBoolTrue)
                throw new SystemException(
                    "[TEST] Property TestBoolTrue is false."
                );

            if (record.TestBoolFalse)
                throw new SystemException(
                    "[TEST] Property TestBoolFalse is true."
                );
        }

    }
}
