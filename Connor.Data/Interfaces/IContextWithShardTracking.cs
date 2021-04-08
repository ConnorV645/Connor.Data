using Connor.Data.Bases;
using Microsoft.EntityFrameworkCore;

namespace Connor.Data.Interfaces
{
    public interface IContextWithShardTracking<T> where T : ShardTrackingBase
    {
        DbSet<T> GetShardTrackingSet();
        T GetNewShardTrackingRecord(string field);
    }
}
