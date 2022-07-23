using System;

namespace MySqlEntityCore.Test
{
    [Model]
    public class Transaction : MySqlEntityCore.Template.DefaultModel
    {
        [Field(Size = 45, Unique = true, Required = true)]
        public string Name { get; set; }

        public Transaction() : base() { }
        public Transaction(uint id) : base(id) { }

        public static void Test()
        {
            Console.WriteLine("[TEST] [TCL] Testing Transactions.");
            TestIsolation();
            TestCommit();
        }

        private static void TestIsolation()
        {
            Console.WriteLine("[TEST] [TCL] Transaction - Test isolation.");
            // Create user isolated within transaction. Must have ID 2
            Transaction isolated = new Transaction();
            isolated.AttachedTransaction = new MySqlEntityCore.Transaction();
            isolated.Name = "Transaction";
            isolated.Create();

            if (isolated.Id != 1)
                throw new SystemException(
                    $"[TEST] [TCL] Isolation failed. User ID {isolated.Id} != 1"
                );

            Transaction transaction = new Transaction(1); // Outside transaction, user should not be found.
            if (transaction.Name != null)
                throw new SystemException(
                    $"[TEST] [TCL] Isolation failed. Transaction not isolated."
                );

            // Inside transaction, user should be found.
            transaction = Transaction.Get<Transaction>(1, isolated.AttachedTransaction);
            if (transaction.Name != isolated.Name)
                throw new SystemException(
                    $"[TEST] [TCL] Isolation failed. Transaction data lost."
                );
            isolated.AttachedTransaction.Rollback();
        }

        private static void TestCommit()
        {
            Console.WriteLine("[TEST] [TCL] Transaction - Test commit.");
            Transaction unisolated = new Transaction();
            unisolated.Name = "Transaction";
            unisolated.Create();

            Transaction isolated = Transaction.Get<Transaction>(2, new MySqlEntityCore.Transaction());
            isolated.Name = "Commited";
            isolated.Write();

            if (isolated.Name == unisolated.Name)
                throw new SystemException(
                    $"[TEST] [TCL] TestCommit failed. Transaction does not await commit on write."
                );

            Cache.Remove("User.2");
            isolated.AttachedTransaction.Commit();

            unisolated = new Transaction(2);
            if (isolated.Name != unisolated.Name)
                throw new SystemException(
                    $"[TEST] [TCL] TestCommit failed. Commit not applied."
                );
        }
    }
}
