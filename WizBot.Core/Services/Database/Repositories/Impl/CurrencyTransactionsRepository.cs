using WizBot.Core.Services.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace WizBot.Core.Services.Database.Repositories.Impl
{
    public class CurrencyTransactionsRepository : Repository<CurrencyTransaction>, ICurrencyTransactionsRepository
    {
        public CurrencyTransactionsRepository(DbContext context) : base(context)
        {
        }

        public List<CurrencyTransaction> GetPageFor(ulong userId, int page)
        {
            return _set.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DateAdded)
                .Skip(10 * page)
                .Take(10)
                .ToList();
        }
    }
}
