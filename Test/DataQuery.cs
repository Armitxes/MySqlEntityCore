using System;
using System.Linq;

namespace MySqlEntityCore.Test
{
    [Model]
    public class DataQuery : MySqlEntityCore.Template.DefaultModel
    {
        [Field(Size = 45, Unique = true, Required = true)]
        public string Name { get; set; }

        public DataQuery() : base() { }
        public DataQuery(uint id) : base(id) { }

        public static void Test()
        {
            Console.WriteLine("[TEST] [DQL] Testing DataQuery.");
            DataQuery dq = new DataQuery() { Name = "Debug" };
            dq.Create();

            TestGet();
            TestConstructorGet();
            TestTableInfo();
        }

        private static void TestGet()
        {
            Console.WriteLine("[TEST] [DQL] Get: With where, orderby, offset and limit.");
            DataQuery dq = Get<DataQuery>(
                where: "`Id`=1",
                orderby: "`id` DESC",
                offset: 0,
                limit: 1
            ).FirstOrDefault();
            if (dq.Id != 1)
                throw new SystemException(
                    $"[TEST] [DQL] Get: Failed."
                );
        }

        private static void TestConstructorGet()
        {
            Console.WriteLine("[TEST] [DQL] Constructor Get.");
            DataQuery dq = new DataQuery(1);
            if (dq.Id != 1)
                throw new SystemException(
                    $"[TEST] [DQL] Constructor Get: Failed."
                );
        }

        private static void TestTableInfo()
        {
            Console.WriteLine("[TEST] [DQL] InformationSchema: All Tables");
            TableInfo.Get("mysqlentitycore");

            Console.WriteLine("[TEST] [DQL] InformationSchema: DataQuery Table");
            new DataQuery().GetTableInfo();
        }
    }
}
