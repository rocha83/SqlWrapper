using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Rochas.DapperRepository.Specification.Enums;
using Rochas.SqlWrapper.Helpers;

namespace Rochas.SqlWrapper.Test
{
    public class EntitySqlParserAdditionalTests
    {
        #region Validation errors

        [Fact]
        public void ParseEntity_WithoutTableAnnotation_FallsBackToClassName()
        {
            var entity = new NoTableValidationEntity { Id = 1 };

            var sql = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite, PersistenceAction.Get, entity);

            Assert.NotNull(sql);
            Assert.StartsWith("SELECT", sql);
            Assert.Contains("NoTableValidationEntity", sql);
        }

        [Fact]
        public void ParseEntity_WithoutKeyAnnotation_ThrowsKeyNotFound()
        {
            var entity = new NoKeyValidationEntity();

            Assert.Throws<KeyNotFoundException>(() =>
                EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite, PersistenceAction.Get));
        }

        #endregion

        #region ParseEntityPaged

        [Theory]
        [InlineData(DatabaseEngine.SQLite)]
        [InlineData(DatabaseEngine.MySQL)]
        [InlineData(DatabaseEngine.PostgreSQL)]
        [InlineData(DatabaseEngine.SQLServer)]
        public void ParseEntityPaged_GeneratesPaginationClause(DatabaseEngine engine)
        {
            var entity = new SampleEntity { Name = "roberto" };

            var sql = EntitySqlParser.ParseEntityPaged(entity, engine, PersistenceAction.Query, entity, 10, 20);

            Assert.NotNull(sql);
            Assert.StartsWith("SELECT", sql);

            if (engine == DatabaseEngine.SQLServer)
                Assert.Contains("OFFSET 10 ROWS FETCH NEXT 20 ROWS ONLY", sql);
            else
                Assert.Contains("LIMIT 20 OFFSET 10", sql);
        }

        [Fact]
        public void ParseEntityPaged_WithSort_AppliesOrdering()
        {
            var entity = new SampleEntity { };

            var sql = EntitySqlParser.ParseEntityPaged(entity, DatabaseEngine.SQLite, PersistenceAction.Query, entity,
                                                        0, 20, sortAttributes: "Name", orderDescending: true);

            Assert.NotNull(sql);
            Assert.Contains("ORDER BY", sql);
            Assert.Contains("DESC", sql);
        }

        #endregion

        #region Grouping

        [Fact]
        public void ParseEntity_WithGroupAttributes_GeneratesGroupBy()
        {
            var entity = new SampleEntity { Name = "roberto" };

            var sql = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite, PersistenceAction.Query, entity,
                                                   groupAttributes: "Name");

            Assert.NotNull(sql);
            Assert.Contains("GROUP BY", sql);
        }

        #endregion

        #region Aggregates dictionary

        [Fact]
        public void ParseEntity_WithAggregates_GeneratesAggregationColumns()
        {
            var entity = new SampleEntity { Name = "roberto" };
            var aggregates = new Dictionary<string, DataAggregationType>
            {
                { "Name", DataAggregationType.Count }
            };

            var sql = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite, PersistenceAction.Query, entity,
                                                   aggregates: aggregates);

            Assert.NotNull(sql);
            Assert.Contains("COUNT", sql);
        }

        #endregion

        #region Relational & Aggregation columns

        [Fact]
        public void ParseEntity_WithRelationalColumns_GeneratesJoins()
        {
            var entity = new SampleRelationalEntity { Name = "rel" };

            var sql = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite, PersistenceAction.Query);

            Assert.NotNull(sql);
            Assert.Contains("INNER JOIN", sql);
            Assert.Contains("LEFT JOIN", sql);
        }

        [Fact]
        public void ParseEntity_WithAggregationColumns_GeneratesAggregations()
        {
            var entity = new SampleRelationalEntity { Name = "rel" };

            var sql = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite, PersistenceAction.Query);

            Assert.NotNull(sql);
            Assert.Contains("SUM", sql);
            Assert.Contains("COUNT", sql);
            Assert.Contains("AVG", sql);
            Assert.Contains("MAX", sql);
            Assert.Contains("MIN", sql);
        }

        #endregion

        #region ParseOneToManyRelation / ParseManyToRelation / SetPersistenceAction

        [Fact]
        public void ParseOneToManyRelation_ExistingChild_AddsToFilterList()
        {
            var childEntityFilter = new SampleManyForeignEntity();
            var listItem = new SampleManyForeignEntity { Id = 9, Code = 3 };
            var listItemProps = Helpers.EntityReflector.GetEntityProperties(typeof(SampleManyForeignEntity));
            var action = PersistenceAction.Add;
            var childFilters = new List<object>();

            EntitySqlParser.ParseOneToManyRelation(childEntityFilter, listItem, typeof(SampleManyForeignEntity), listItemProps,
                                                    ref action, childFilters);

            Assert.Equal(PersistenceAction.Update, action);
            Assert.Single(childFilters);
        }

        [Fact]
        public void ParseOneToManyRelation_NewChild_KeepsAddAction()
        {
            var childEntityFilter = new SampleManyForeignEntity();
            var listItem = new SampleManyForeignEntity { Id = 0, Code = 3 };
            var listItemProps = Helpers.EntityReflector.GetEntityProperties(typeof(SampleManyForeignEntity));
            var action = PersistenceAction.Add;
            var childFilters = new List<object>();

            EntitySqlParser.ParseOneToManyRelation(childEntityFilter, listItem, typeof(SampleManyForeignEntity), listItemProps,
                                                    ref action, childFilters);

            Assert.Equal(PersistenceAction.Add, action);
            Assert.Empty(childFilters);
        }

        [Fact]
        public void ParseManyToRelation_WithIntermediaryEntity_ReturnsIntermediary()
        {
            var childEntity = new SampleManyForeignEntity { Id = 42 };
            var relation = Helpers.EntityReflector.GetRelatedEntityAttribute(
                typeof(SampleEntity).GetProperty("ManyToManyForeignEntities"));

            var result = EntitySqlParser.ParseManyToRelation(childEntity, relation);

            Assert.NotNull(result);
            Assert.IsType<SampleIntermedyForeignEntity>(result);
        }

        [Fact]
        public void SetPersistenceAction_ZeroKey_ReturnsAdd()
        {
            var entity = new SampleEntity { Id = 0 };
            var key = Helpers.EntityReflector.GetKeyColumn(typeof(SampleEntity), typeof(SampleEntity).GetProperties());

            var action = EntitySqlParser.SetPersistenceAction(entity, key);

            Assert.Equal(PersistenceAction.Add, action);
        }

        [Fact]
        public void SetPersistenceAction_NonZeroKey_ReturnsUpdate()
        {
            var entity = new SampleEntity { Id = 5 };
            var key = Helpers.EntityReflector.GetKeyColumn(typeof(SampleEntity), typeof(SampleEntity).GetProperties());

            var action = EntitySqlParser.SetPersistenceAction(entity, key);

            Assert.Equal(PersistenceAction.Update, action);
        }

        #endregion

        #region Range filter numeric

        [Fact]
        public void Query_NumericRangeOnlyFrom_ShouldUseGreaterOrEqual()
        {
            var filter = new SampleEntity { Age = 18 };

            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite, PersistenceAction.Query, filter);

            Assert.Contains(">=", result);
            Assert.DoesNotContain("BETWEEN", result);
        }

        [Fact]
        public void Query_NumericRangeOnlyTo_ShouldUseLessOrEqual()
        {
            var filter = new SampleEntity { AgeEnd = 65 };

            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite, PersistenceAction.Query, filter);

            Assert.Contains("<=", result);
            Assert.DoesNotContain("BETWEEN", result);
        }

        #endregion

        #region Postgres grouping/relational quoting

        [Fact]
        public void ParseEntity_GroupAttributes_Postgres_QuotesIdentifiers()
        {
            var entity = new SampleEntity { Name = "roberto" };

            var sql = EntitySqlParser.ParseEntity(entity, DatabaseEngine.PostgreSQL, PersistenceAction.Query, entity,
                                                   groupAttributes: "Name");

            Assert.NotNull(sql);
            Assert.Contains("\"sample_entity\"", sql);
            Assert.Contains("\"name\"", sql);
        }

        #endregion
    }

    public class NoTableValidationEntity
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("no_key_entity")]
    public class NoKeyValidationEntity
    {
        public string Name { get; set; }
    }
}
