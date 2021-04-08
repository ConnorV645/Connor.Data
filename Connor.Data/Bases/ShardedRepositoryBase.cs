using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Connor.Data.Enums;
using Connor.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Connor.Data.Bases
{
    /// <summary>
    /// Setup a Sharded Data Repository
    /// </summary>
    /// <typeparam name="T">Common Base Type</typeparam>
    /// <typeparam name="K">Shard Key Type</typeparam>
    /// <typeparam name="S"></typeparam>
    public abstract class ShardedRepositoryBase<T, K, S> : GeneralRepositoryBase<T>
        where T : DbContext, IContextWithShardTracking<S>
        where S : ShardTrackingBase
    {
        protected readonly List<(T writer, T reader)> shards;

        public int ShardCount { get => shards.Count; }

        public ShardedRepositoryBase(List<(T writer, T reader)> shards)
        {
            if (shards == null || shards.Count == 0)
            {
                throw new Exception("DB Shards Required");
            }
            this.shards = shards;
        }

        protected abstract T GetContext(DbMode mode, K key, bool isShardId = false);
        protected abstract int FindShard(K key);

        public virtual async Task<int> SaveAccountContext<E>(E detachObj, K key) where E : class
        {
            var context = GetContext(DbMode.ReadWrite, key);
            var output = await context.SaveChangesAsync();
            context.Entry(detachObj).State = EntityState.Detached;
            return output;
        }

        private async Task<long> NextIdBase(string field, int attempts = 0)
        {
            try
            {
                var defaultContext = shards[0].writer;
                var set = defaultContext.GetShardTrackingSet();
                var record = await set.FirstOrDefaultAsync(x => x.Field == field);
                if (record == null)
                {
                    record = defaultContext.GetNewShardTrackingRecord(field);
                    set.Add(record);
                }
                var nextId = record.NextId;
                record.NextId++;
                await defaultContext.SaveChangesAsync();
                return nextId;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (attempts == 99)
                {
                    throw new Exception("Error Getting Next Id!");
                }
                return await NextIdBase(field, ++attempts);
            }
        }
    }
}
