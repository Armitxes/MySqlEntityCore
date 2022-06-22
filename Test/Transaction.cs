using System;

namespace MySqlEntityCore.Test
{
    public class Transaction
    {
        public static void Test()
        {
            Console.WriteLine("[TEST] Testing transactions.");
            TestIsolation();
            TestCommit();
        }

        private static void TestIsolation()
        {
            // Create user isolated within transaction. Must have ID 2
            User isolatedUser = new User();
            isolatedUser.AttachedTransaction = new MySqlEntityCore.Transaction();
            isolatedUser.Username = "Transaction User";
            isolatedUser.Password = "111";
            isolatedUser.Create();

            if (isolatedUser.Id != 2)
                throw new SystemException(
                    $"[TEST] TestIsolation failed. User ID {isolatedUser.Id} != 2"
                );

            User user = new User(2); // Outside transaction, user should not be found.
            if (user.Username != null)
                throw new SystemException(
                    $"[TEST] TestIsolation failed. Transaction not isolated."
                );

            // Inside transaction, user should be found.
            user = User.Get<User>(2, isolatedUser.AttachedTransaction);
            if (user.Username != isolatedUser.Username)
                throw new SystemException(
                    $"[TEST] TestIsolation failed. Transaction data lost."
                );
            isolatedUser.AttachedTransaction.Rollback();
        }

        private static void TestCommit()
        {
            User user = User.Get<User>(1, new MySqlEntityCore.Transaction());
            user.Username = "Commited";
            user.Write();

            User Unisolated = new User(1);
            if (user.Username == Unisolated.Username)
                throw new SystemException(
                    $"[TEST] TestCommit failed. Transaction does not await commit on write."
                );

            Cache.Remove("User.1");
            user.AttachedTransaction.Commit();

            Unisolated = new User(1);
            if (user.Username != Unisolated.Username)
                throw new SystemException(
                    $"[TEST] TestCommit failed. Commit not applied."
                );
        }
    }
}
