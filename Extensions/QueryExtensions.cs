using Neo4jClient.Cypher;

namespace Tokengram.Extensions
{
    public static class QueryExtensions
    {
        // Neo4j

        public static async Task<T> FirstAsync<T>(this ICypherFluentQuery<T> queryable)
        {
            var results = await queryable.Limit(1).ResultsAsync;

            return results.Single();
        }

        public static async Task<T?> FirstOrDefaultAsync<T>(this ICypherFluentQuery<T> queryable)
        {
            var results = await queryable.Limit(1).ResultsAsync;

            return results.SingleOrDefault();
        }

        public static ICypherFluentQuery<T> Paginate<T>(
            this ICypherFluentQuery<T> queryable,
            int pageNumber,
            int pageSize
        )
        {
            return queryable.Skip((pageNumber - 1) * pageSize).Limit(pageSize);
        }

        public static ICypherFluentQuery<T> OrderByNodeId<T>(
            this ICypherFluentQuery<T> queryable,
            params string[] properties
        )
        {
            return OrderByNodeIdInternal(queryable, properties, false);
        }

        public static ICypherFluentQuery<T> OrderByNodeIdDesceding<T>(
            this ICypherFluentQuery<T> queryable,
            params string[] properties
        )
        {
            return OrderByNodeIdInternal(queryable, properties, true);
        }

        public static ICypherFluentQuery OptionalMatchIf(
            this ICypherFluentQuery queryable,
            bool condition,
            string pattern
        )
        {
            return condition ? queryable.OptionalMatch(pattern) : queryable.Match(pattern);
        }

        public static ICypherFluentQuery WhereAlwaysTrue(this ICypherFluentQuery queryable)
        {
            return queryable.Where("true");
        }

        private static ICypherFluentQuery<T> OrderByNodeIdInternal<T>(
            ICypherFluentQuery<T> queryable,
            string[] properties,
            bool descending
        )
        {
            if (properties == null || properties.Length == 0)
            {
                return queryable;
            }

            string orderByClause = string.Join(", ", properties.Select(property => $"elementId({property})"));

            return descending ? queryable.OrderByDescending(orderByClause) : queryable.OrderBy(orderByClause);
        }

        // Postgres - EF

        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
        {
            return queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
