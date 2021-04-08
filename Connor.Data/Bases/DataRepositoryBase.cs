using System.Threading.Tasks;
using Connor.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Connor.Data.Bases
{
    /// <summary>
    /// Setup a Write + Read Replica Data Repository
    /// </summary>
    /// <typeparam name="W">Writer Type</typeparam>
    /// <typeparam name="R">Reader Type</typeparam>
    /// <typeparam name="B">Common Base Type</typeparam>
    public abstract class DataRepositoryBase<W, R, B> : GeneralRepositoryBase<B>
        where W : B 
        where R : B 
        where B : DbContext
    {
        protected readonly W writer;
        protected readonly R reader;

        public DataRepositoryBase(W writer, R reader)
        {
            this.writer = writer;
            this.reader = reader;
        }

        public virtual async Task<int> SaveAccountContext<T>(T detachObj) where T : class
        {
            var context = GetContext(DbMode.ReadWrite);
            var output = await context.SaveChangesAsync();
            context.Entry(detachObj).State = EntityState.Detached;
            return output;
        }

        protected virtual B GetContext(DbMode mode)
        {
            return mode == DbMode.ReadWrite ? writer : reader;
        }
    }
}
