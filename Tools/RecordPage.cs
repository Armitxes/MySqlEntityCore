using System.Collections.Generic;
using MySqlEntityCore.Template;

namespace MySqlEntityCore.Tools
{
    // <summary>Paged result of records.</summary>
    public class RecordPage<T> where T : Core
    {
        /// <summary>Current page number.</summary>
        public uint Number { get; private set; }

        /// <summary>Maximum record count of current page.</summary>
        public uint Limit { get; private set; }

        /// <summary>Total offset from first record.</summary>
        public uint Offset { get; private set; }

        /// <summary>Record list of the current page.</summary>
        public List<T> Records { get; private set; }

        /// <summary>Get a list of records by the given conditions. Leave null/0 for all lines.</summary>
        /// <param name="pageNumber">Requested page number.</param>
        /// <param name="recordLimit">Maximum record count per page.</param>
        /// <param name="where">SQL "WHERE" condition</param>
        /// <param name="orderBy">SQL "ORDER BY" statement</param>
        /// <returns>Result list of given class type.</returns>
        public RecordPage(
            uint pageNumber = 1,
            uint recordLimit = 30,
            string where = null,
            string orderBy = null
        )
        {
            Number = (pageNumber == 0) ? 1 : pageNumber;
            Limit = recordLimit;
            Offset = (Number - 1) * Limit;

            Records = Core.Get<T>(
                offset: Offset,
                limit: Limit,
                where: where,
                orderby: orderBy
            );
        }
    }
}
