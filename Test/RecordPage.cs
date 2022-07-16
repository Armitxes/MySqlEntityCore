using System;
using MySqlEntityCore.Tools;

namespace MySqlEntityCore.Test
{
    public class RecordPages
    {
        public static uint TotalRecords = 61;
        public static uint RecordLimitPerPage = 5;

        public static void Test()
        {
            Console.WriteLine("[TEST] Testing RecordPage.");
            Prepare();
            TestFirstPage();
            TestLastPage();
        }

        private static void Prepare()
        {
            Console.WriteLine("[TEST] Preparing data for RecordPage tests.");
            for (uint i = 0; i < TotalRecords; i++)
            {
                RecordPageEntry recPage = new RecordPageEntry();
                recPage.Name = i.ToString();
                recPage.Create();

                if (recPage.Id == 0)
                    throw new SystemException(
                        $"[TEST] Failed to prepare RecordPage data at index {i}."
                    );
            }
        }

        private static void TestFirstPage()
        {
            Console.WriteLine("[TEST] RecordPage: Checking first page.");

            RecordPage<RecordPageEntry> page = new RecordPage<RecordPageEntry>(
                pageNumber: 0,  // Page 0 === Page 1
                recordLimit: RecordLimitPerPage
            );

            if (page.Records.Count != RecordLimitPerPage)
                throw new SystemException(
                    $"[TEST] RecordPage TestFirstPage: Wrong record amount ({page.Records.Count} != {RecordLimitPerPage})."
                );

            if (page.Records[0].Id != 1)
                throw new SystemException(
                    $"[TEST] RecordPage TestFirstPage: Wrong first ID ({page.Records[0].Id}) entry. Default sorting messed up?"
                );

            if (page.Number != 1)
                throw new SystemException(
                    $"[TEST] RecordPage TestFirstPage: FirstPage Number is not 1"
                );

            if (page.Offset != 0)
                throw new SystemException(
                    $"[TEST] RecordPage TestFirstPage: Offset property unequal 0"
                );
        }

        private static void TestLastPage()
        {
            Console.WriteLine("[TEST] RecordPage: Checking last page.");
            decimal difference = TotalRecords / RecordLimitPerPage;
            uint lastPage = (uint)Math.Ceiling(difference);

            // difference has decimal places -> records cannot fully fill last page
            bool lastPageFilled = (difference % 1) == 0;

            RecordPage<RecordPageEntry> page = new RecordPage<RecordPageEntry>(
                pageNumber: lastPage,
                recordLimit: RecordLimitPerPage
            );

            if (!lastPageFilled && page.Records.Count == RecordLimitPerPage)
                throw new SystemException(
                    $"[TEST] RecordPage TestLastPage: Last page contains too many records."
                );

            if (lastPageFilled && page.Records.Count != RecordLimitPerPage)
                throw new SystemException(
                    $"[TEST] RecordPage TestLastPage: Last page is missing records."
                );
        }
    }

    [Model]
    internal class RecordPageEntry : MySqlEntityCore.Template.DefaultModel
    {
        [Field(Size = 12, Required = true)]
        public string Name { get; set; }
        public RecordPageEntry() : base() { }
        public RecordPageEntry(uint id) : base(id) { }
    }
}
