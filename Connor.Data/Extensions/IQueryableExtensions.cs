using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Connor.Data.Extensions
{
    public static class IQueryableExtensions
    {
        public static List<T> ContainsToListBatched<T, Z>(this IQueryable<T> query, List<Z> checkList, string checkPropertyName, int batchSize)
        {
            var returnList = new List<T>();
            for (int i = 0; i < checkList.Count; i += batchSize)
            {
                var checkIds = checkList.Skip(i).Take(batchSize).ToList();
                var list = Expression.Constant(checkIds);

                var containMethod = typeof(List<Z>).GetMethod("Contains", new Type[] { typeof(Z) });

                var param = Expression.Parameter(typeof(T), "x");
                var checkValue = Expression.PropertyOrField(param, checkPropertyName);
                var body = Expression.Call(list, containMethod, checkValue);

                var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                returnList.AddRange(query.Where(lambda).ToList());
            }
            return returnList;
        }

        public static List<Y> ContainsToListBatched<T, Z, Y>(this IQueryable<T> query, List<Z> checkList, string checkPropertyName, int batchSize, Expression<Func<T, Y>> selector)
        {
            var returnList = new List<Y>();
            for (int i = 0; i < checkList.Count; i += batchSize)
            {
                var checkIds = checkList.Skip(i).Take(batchSize).ToList();
                var list = Expression.Constant(checkIds);

                var containMethod = typeof(List<Z>).GetMethod("Contains", new Type[] { typeof(Z) });

                var param = Expression.Parameter(typeof(T), "x");
                var checkValue = Expression.PropertyOrField(param, checkPropertyName);
                var body = Expression.Call(list, containMethod, checkValue);

                var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                returnList.AddRange(query.Where(lambda).Select(selector).ToList());
            }
            return returnList;
        }

        public static async Task<List<T>> ContainsToListBatchedAsync<T, Z>(this IQueryable<T> query, List<Z> checkList, string checkPropertyName, int batchSize)
        {
            var returnList = new List<T>();
            for (int i = 0; i < checkList.Count; i += batchSize)
            {
                var checkIds = checkList.Skip(i).Take(batchSize).ToList();
                var list = Expression.Constant(checkIds);

                var containMethod = typeof(List<Z>).GetMethod("Contains", new Type[] { typeof(Z) });

                var param = Expression.Parameter(typeof(T), "x");
                var checkValue = Expression.PropertyOrField(param, checkPropertyName);
                var body = Expression.Call(list, containMethod, checkValue);

                var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                returnList.AddRange(await query.Where(lambda).ToListAsync());
            }
            return returnList;
        }

        public static async Task<List<Y>> ContainsToListBatchedAsync<T, Z, Y>(this IQueryable<T> query, List<Z> checkList, string checkPropertyName, int batchSize, Expression<Func<T, Y>> selector)
        {
            var returnList = new List<Y>();
            for (int i = 0; i < checkList.Count; i += batchSize)
            {
                var checkIds = checkList.Skip(i).Take(batchSize).ToList();
                var list = Expression.Constant(checkIds);

                var containMethod = typeof(List<Z>).GetMethod("Contains", new Type[] { typeof(Z) });

                var param = Expression.Parameter(typeof(T), "x");
                var checkValue = Expression.PropertyOrField(param, checkPropertyName);
                var body = Expression.Call(list, containMethod, checkValue);

                var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                returnList.AddRange(await query.Where(lambda).Select(selector).ToListAsync());
            }
            return returnList;
        }
    }
}
