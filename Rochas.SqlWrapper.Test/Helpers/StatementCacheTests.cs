using System.Collections.Generic;
using System.Linq;
using Xunit;
using Rochas.Data.Specification.Enums;
using Rochas.SqlWrapper.Helpers;

namespace Rochas.SqlWrapper.Test
{
    /// <summary>
    /// Paridade: 2ª chamada (hit do statement cache) deve devolver SQL e
    /// parâmetros idênticos aos da 1ª chamada (miss).
    /// </summary>
    public class StatementCacheTests
    {
        private static (string sql, Dictionary<string, object> p) ParseTwice(
            SampleEntity filter, PersistenceAction action,
            string group = null, string sort = null, int limit = 0)
        {
            var p1 = new Dictionary<string, object>();
            var s1 = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite, action, filter,
                recordLimit: limit, groupAttributes: group, sortAttributes: sort, sqlParameters: p1).Trim();
            var p2 = new Dictionary<string, object>();
            var s2 = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite, action, filter,
                recordLimit: limit, groupAttributes: group, sortAttributes: sort, sqlParameters: p2).Trim();
            Assert.Equal(s1, s2);
            Assert.Equal(p1.Count, p2.Count);
            foreach (var kv in p1)
                Assert.Equal(kv.Value, p2[kv.Key]);
            return (s2, p2);
        }

        [Fact]
        public void QueryByName_Parity()
        {
            var (sql, p) = ParseTwice(new SampleEntity { Name = "roberto" }, PersistenceAction.Query);
            Assert.Contains("LIKE", sql);
            Assert.True(p.Count > 0);
        }

        [Fact]
        public void CountByDoc_Parity()
        {
            var (sql, p) = ParseTwice(new SampleEntity { DocNumber = 12345 }, PersistenceAction.Count);
            Assert.Contains("COUNT", sql);
            Assert.Contains("12345", sql);
        }

        [Fact]
        public void QueryGroupSort_Parity()
        {
            var (sql, _) = ParseTwice(new SampleEntity { Name = "roberto" }, PersistenceAction.Query,
                group: "Name", sort: "Name");
            Assert.Contains("GROUP BY", sql);
            Assert.Contains("ORDER BY", sql);
        }

        [Fact]
        public void Paged_Parity()
        {
            var f = new SampleEntity { Name = "roberto" };
            var p1 = new Dictionary<string, object>();
            var s1 = EntitySqlParser.ParseEntityPaged(f, DatabaseEngine.SQLite,
                PersistenceAction.Query, f, offset: 20, pageSize: 10, sqlParameters: p1).Trim();
            var p2 = new Dictionary<string, object>();
            var s2 = EntitySqlParser.ParseEntityPaged(f, DatabaseEngine.SQLite,
                PersistenceAction.Query, f, offset: 20, pageSize: 10, sqlParameters: p2).Trim();
            Assert.Equal(s1, s2);
            Assert.Equal(p1.Count, p2.Count);
        }

        [Fact]
        public void VaryingValues_NoStaleSql()
        {
            var p1 = new Dictionary<string, object>();
            var s1 = EntitySqlParser.ParseEntity(new SampleEntity { DocNumber = 111 },
                DatabaseEngine.SQLite, PersistenceAction.Query, new SampleEntity { DocNumber = 111 },
                sqlParameters: p1).Trim();
            var f2 = new SampleEntity { DocNumber = 222 };
            var p2 = new Dictionary<string, object>();
            var s2 = EntitySqlParser.ParseEntity(f2,
                DatabaseEngine.SQLite, PersistenceAction.Query, f2, sqlParameters: p2).Trim();
            Assert.Contains("111", s1);
            Assert.Contains("222", s2);
            Assert.DoesNotContain("111", s2);
        }
    }
}
