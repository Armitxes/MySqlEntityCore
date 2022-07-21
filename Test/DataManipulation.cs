using System;

namespace MySqlEntityCore.Test
{
    [Model]
    public class DataManipulation : MySqlEntityCore.Template.DefaultModel
    {
        [Field(Size = 45, Unique = true, Required = true)]
        public string Name { get; set; }

        public DataManipulation() : base() { }
        public DataManipulation(uint id) : base(id) { }

        public static void Test()
        {
            TestCreate();
            TestWrite();
            TestRevert();
        }

        public static void TestCreate()
        {
            Console.WriteLine("[TEST] [DML] Create - Check creation and set ID value.");
            DataManipulation dM = new DataManipulation();
            dM.Name = "Name 1";
            dM.Create();

            if (dM.Id != 1)
                throw new SystemException(
                    $"[TEST] [DML] Create test failed. Wrong ID returned ({dM.Id} != 1)"
                );
        }

        public static void TestWrite()
        {
            Console.WriteLine("[TEST] [DML] Write - Class instance.");
            DataManipulation dM = new DataManipulation(1);
            dM.Name = "Name";
            dM.Write();

            if (dM.Name != "Name")
                throw new SystemException(
                    $"[TEST] [DML] Write test failed. Class instance contains wrong value."
                );

            Console.WriteLine("[TEST] [DML] Write - Cache value.");
            dM = new DataManipulation(1);
            if (dM.Name != "Name")
                throw new SystemException(
                    $"[TEST] [DML] Write test failed. Cache contains wrong value."
                );

            Console.WriteLine("[TEST] [DML] Write - DB value.");
            Cache.Remove("DataManipulation.1");
            dM = new DataManipulation(1);
            if (dM.Name != "Name")
                throw new SystemException(
                    $"[TEST] [DML] Write test failed. DB field contains wrong value."
                );
        }

        public static void TestRevert()
        {
            Console.WriteLine("[TEST] [DML] Revert - Test revert of uncommited changes.");
            DataManipulation dM = new DataManipulation(1);
            dM.Name = "Dummy";
            dM.Revert();
            if (dM.Name != "Name")
                throw new SystemException(
                    $"[TEST] [DML] Revert test failed. Wrong value."
                );
        }
    }
}
