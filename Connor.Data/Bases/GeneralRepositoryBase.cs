using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Connor.Data.Bases
{
    public abstract class GeneralRepositoryBase<B> where B : DbContext
    {
        protected static async Task AddOrUpdateBase<T>(B context, DbSet<T> set, T obj, string pkField) where T : class
        {
            var pkValue = obj.GetType().GetProperty(pkField).GetValue(obj, null);
            if (Convert.ToInt64(pkValue).Equals(0L))
            {
                set.Add(obj);
            }
            else
            {
                set.Attach(obj);
                context.Entry(obj).State = EntityState.Modified;
            }
            await context.SaveChangesAsync();
            context.Entry(obj).State = EntityState.Detached;
        }

        protected static async Task AddOrUpdateViaLookup<T>(B context, DbSet<T> set, T obj, params string[] pkFields) where T : class
        {
            var objType = obj.GetType();
            var pkValues = pkFields.ToList().Select(x => objType.GetProperty(x).GetValue(obj)).ToArray();
            var dbRecord = await set.FindAsync(pkValues);
            if (dbRecord == null)
            {
                set.Add(obj);
            }
            else
            {
                // Detatch Old
                context.Entry(dbRecord).State = EntityState.Detached;

                // Attach New
                set.Attach(obj);
                context.Entry(obj).State = EntityState.Modified;
            }
            await context.SaveChangesAsync();
            context.Entry(obj).State = EntityState.Detached;
        }

        protected static async Task DeleteBase<T>(B context, DbSet<T> set, T obj) where T : class
        {
            try
            {
                set.Remove(obj);
                await context.SaveChangesAsync();
            }
            catch
            {
                // Likely trying to delete something that is already deleted
            }
        }
    }
}
