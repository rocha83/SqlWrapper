using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Rochas.Data.Specification.Annotations;
using Rochas.Data.Specification.Enums;
using Rochas.SqlWrapper.Exceptions;
using Rochas.SqlWrapper.Helpers;

namespace Rochas.SqlWrapper.Test
{
    public class EntitySqlParserCoverageTest
    {
        #region Pagination (ParseEntityPaged)

        [Fact]
        public void ParseEntityPaged_MySql_ShouldAppendLimitOffset()
        {
            var filter = new SampleEntity { Name = "roberto" };
            var result = EntitySqlParser.ParseEntityPaged(filter, DatabaseEngine.MySQL,
                PersistenceAction.Query, filter, offset: 10, pageSize: 5).Trim();

            Assert.StartsWith("SELECT", result);
            Assert.EndsWith("LIMIT 5 OFFSET 10;", result);
        }

        [Fact]
        public void ParseEntityPaged_Sqlite_ShouldAppendLimitOffset()
        {
            var filter = new SampleEntity { Name = "roberto" };
            var result = EntitySqlParser.ParseEntityPaged(filter, DatabaseEngine.SQLite,
                PersistenceAction.Query, filter, offset: 2, pageSize: 8).Trim();

            Assert.EndsWith("LIMIT 8 OFFSET 2;", result);
        }

        [Fact]
        public void ParseEntityPaged_PostgreSql_ShouldAppendLimitOffset()
        {
            var filter = new SampleEntity { Name = "roberto" };
            var result = EntitySqlParser.ParseEntityPaged(filter, DatabaseEngine.PostgreSQL,
                PersistenceAction.Query, filter, offset: 3, pageSize: 6).Trim();

            Assert.EndsWith("LIMIT 6 OFFSET 3;", result);
        }

        [Fact]
        public void ParseEntityPaged_SqlServer_ShouldAppendOffsetFetch()
        {
            var filter = new SampleEntity { Name = "roberto" };
            var result = EntitySqlParser.ParseEntityPaged(filter, DatabaseEngine.SQLServer,
                PersistenceAction.Query, filter, offset: 4, pageSize: 7).Trim();

            Assert.EndsWith("OFFSET 4 ROWS FETCH NEXT 7 ROWS ONLY;", result);
        }

        [Fact]
        public void ParseEntityPaged_UnknownEngine_ShouldFallbackToLimitOffset()
        {
            var filter = new SampleEntity { Name = "roberto" };
            var result = EntitySqlParser.ParseEntityPaged(filter, (DatabaseEngine)99,
                PersistenceAction.Query, filter, offset: 1, pageSize: 2).Trim();

            Assert.EndsWith("LIMIT 2 OFFSET 1;", result);
        }

        [Fact]
        public void ParseEntityPaged_WithGroupAndSort_ShouldRenderGroupByAndOrderBy()
        {
            var filter = new SampleEntity { Name = "roberto" };
            var result = EntitySqlParser.ParseEntityPaged(filter, DatabaseEngine.SQLite,
                PersistenceAction.Query, filter, offset: 0, pageSize: 10,
                groupAttributes: "Name", sortAttributes: "DocNumber", orderDescending: true).Trim();

            Assert.Contains("GROUP BY", result);
            Assert.Contains("ORDER BY doc_number DESC", result);
            Assert.EndsWith("LIMIT 10 OFFSET 0;", result);
        }

        #endregion

        #region Grouping

        [Fact]
        public void Query_WithGroupAttributes_ShouldRenderGroupBy()
        {
            var filter = new SampleEntity { Name = "roberto", Active = true };
            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite,
                PersistenceAction.Query, filter, groupAttributes: "Name").Trim();

            Assert.Contains("GROUP BY", result);
        }

        [Fact]
        public void Query_WithGroupAttributes_PostgreSql_ShouldQuoteColumns()
        {
            var filter = new SampleEntity { Name = "roberto", Age = 30 };
            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.PostgreSQL,
                PersistenceAction.Query, filter, groupAttributes: "Name, Age").Trim();

            Assert.Contains("GROUP BY", result);
            Assert.Contains("\"name\"", result);
        }

        [Fact]
        public void Query_WithSortDescending_ShouldUseDesc()
        {
            var filter = new SampleEntity();
            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite,
                PersistenceAction.Query, filter, sortAttributes: "Name", orderDescending: true).Trim();

            Assert.EndsWith("ORDER BY name DESC", result);
        }

        #endregion

        #region Validation

        [Fact]
        public void ParseEntity_WithoutKeyAnnotation_ShouldThrowKeyNotFound()
        {
            var entity = new SampleNoKeyEntity { Name = "x" };

            Assert.Throws<KeyNotFoundException>(() =>
                EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite, PersistenceAction.Get, entity));
        }

        [Fact]
        public void ParseEntityPaged_WithoutKeyAnnotation_ShouldThrowKeyNotFound()
        {
            var entity = new SampleNoKeyEntity { Name = "x" };

            Assert.Throws<KeyNotFoundException>(() =>
                EntitySqlParser.ParseEntityPaged(entity, DatabaseEngine.SQLite, PersistenceAction.Query));
        }

        #endregion

        #region Listable Attributes in ParseEntity

        [Fact]
        public void ParseEntity_OnlyListableAttributes_ShouldRestrictColumns()
        {
            var entity = new SampleListableEntity { Name = "a" };
            var result = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite,
                PersistenceAction.Query, entity, onlyListableAttributes: true, showAttributes: "Name").Trim();

            Assert.StartsWith("SELECT", result);
            Assert.Contains("sample_listable_entity.name", result);
            Assert.DoesNotContain("sample_listable_entity.description", result);
            Assert.DoesNotContain("sample_listable_entity.secret", result);
        }

        #endregion

        #region Aggregates Dictionary

        [Fact]
        public void Query_WithAggregatesDictionary_ShouldRenderAggregations()
        {
            var filter = new SampleEntity { DocNumber = 12345 };
            var aggregates = new Dictionary<string, DataAggregationType>
            {
                { "Height", DataAggregationType.Sum },
                { "Age", DataAggregationType.Average },
                { "Weight", DataAggregationType.Maximum },
                { "ChildId", DataAggregationType.Minimum }
            };

            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite,
                PersistenceAction.Query, filter, aggregates: aggregates).Trim();

            Assert.Contains("SUM(", result);
            Assert.Contains("AVG(", result);
            Assert.Contains("MAX(", result);
            Assert.Contains("MIN(", result);
        }

        #endregion

        #region Relational Columns and Aggregation Annotations

        [Fact]
        public void Query_WithRelationalAndAggregation_ShouldRenderJoinsAndAggregates()
        {
            var entity = new SampleQueryEntity { Id = 1, Name = "x", FilterRel = "abc" };
            var sqlParameters = new Dictionary<string, object>();
            var result = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite,
                PersistenceAction.Query, entity, sqlParameters: sqlParameters).Trim();

            Assert.StartsWith("SELECT", result);
            Assert.Contains("INNER JOIN related", result);
            Assert.Contains("LEFT JOIN", result);
            Assert.Contains("SUM(", result);
            Assert.Contains("COUNT(", result);
            Assert.Contains("AVG(", result);
            Assert.Contains("MIN(", result);
            Assert.Contains("MAX(", result);
            Assert.Contains("LIKE @p", result);
        }

        #endregion

        #region Relation Helpers

        [Fact]
        public void ParseOneToManyRelation_NewItem_ShouldKeepAddAction()
        {
            var action = PersistenceAction.Add;
            var childFilters = new List<object>();
            object childFilter = null;
            var props = typeof(SampleManyForeignEntity).GetProperties();

            EntitySqlParser.ParseOneToManyRelation(childFilter, new SampleManyForeignEntity { Id = 0 },
                typeof(SampleManyForeignEntity), props, ref action, childFilters);

            Assert.Equal(PersistenceAction.Add, action);
            Assert.Empty(childFilters);
        }

        [Fact]
        public void ParseOneToManyRelation_ExistingItem_ShouldSetUpdateActionAndFilter()
        {
            var action = PersistenceAction.Add;
            var childFilters = new List<object>();
            object childFilter = null;
            var props = typeof(SampleManyForeignEntity).GetProperties();

            EntitySqlParser.ParseOneToManyRelation(childFilter, new SampleManyForeignEntity { Id = 7 },
                typeof(SampleManyForeignEntity), props, ref action, childFilters);

            Assert.Equal(PersistenceAction.Update, action);
            Assert.Single(childFilters);
            Assert.Equal(7, ((SampleManyForeignEntity)childFilters[0]).Id);
        }

        [Fact]
        public void SetPersistenceAction_ShouldReturnAddForZeroKey()
        {
            var entity = new SampleManyForeignEntity { Id = 0 };
            var key = typeof(SampleManyForeignEntity).GetProperty("Id");

            Assert.Equal(PersistenceAction.Add, EntitySqlParser.SetPersistenceAction(entity, key));
        }

        [Fact]
        public void SetPersistenceAction_ShouldReturnUpdateForNonZeroKey()
        {
            var entity = new SampleManyForeignEntity { Id = 42 };
            var key = typeof(SampleManyForeignEntity).GetProperty("Id");

            Assert.Equal(PersistenceAction.Update, EntitySqlParser.SetPersistenceAction(entity, key));
        }

        [Fact]
        public void ParseManyToRelation_WithIntermediaryEntity_ShouldFillIntermediaryKey()
        {
            var relation = typeof(SampleEntity).GetProperty("ManyToManyForeignEntities")
                .GetCustomAttributes(typeof(RelatedEntityAttribute), false)
                .OfType<RelatedEntityAttribute>()
                .First();
            var child = new SampleManyForeignEntity { Id = 9 };

            var result = EntitySqlParser.ParseManyToRelation(child, relation) as SampleIntermedyForeignEntity;

            Assert.NotNull(result);
            Assert.Equal(9, result.LeftSideId);
        }

        [Fact]
        public void ParseManyToRelation_WithoutIntermediaryEntity_ShouldReturnNull()
        {
            var relation = typeof(SampleEntity).GetProperty("OneToOneForeignEntity")
                .GetCustomAttributes(typeof(RelatedEntityAttribute), false)
                .OfType<RelatedEntityAttribute>()
                .First();
            var child = new SampleOneForeignEntity();

            Assert.Null(EntitySqlParser.ParseManyToRelation(child, relation));
        }

        #endregion

        #region Value Formatting (sample_types_entity)

        [Fact]
        public void Add_WithVariousValueTypes_ShouldFormatAllValues()
        {
            var entity = new SampleTypesEntity
            {
                ShortCode = short.MinValue,
                LongCode = long.MinValue,
                FloatVal = 1.5f,
                DoubleVal = 2.5d,
                DecimalVal = 3.5m,
                GuidVal = Guid.Empty,
                Status = SampleStatus.Approved,
                Name = "john",
                Blob = new byte[] { 1, 2 },
                Codes = new uint[] { 1, 2, 3 }
            };

            var result = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite, PersistenceAction.Add).Trim();

            Assert.StartsWith("INSERT INTO sample_types_entity", result);
            Assert.Contains("short_code", result);
            Assert.Contains("long_code", result);
            Assert.Contains("NULL", result);
            Assert.Contains("guid_val", result);
            Assert.Contains("status", result);
            Assert.Contains("'1,2,3'", result);
            Assert.Contains("'AQI='", result);
        }

        [Fact]
        public void Query_WithByteArray_ShouldNotFail()
        {
            var entity = new SampleTypesEntity { Blob = new byte[] { 1, 2 } };
            var result = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite,
                PersistenceAction.Query, entity).Trim();

            Assert.StartsWith("SELECT", result);
        }

        [Fact]
        public void Query_WithArrayValue_ShouldNotCrash()
        {
            var entity = new SampleTypesEntity { Codes = new uint[] { 4, 5 } };
            var result = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite,
                PersistenceAction.Query, entity).Trim();

            Assert.StartsWith("SELECT", result);
        }

        [Fact]
        public void Query_WithEmptyGuid_ShouldNotAddGuidFilter()
        {
            var filter = new SampleTypesEntity { GuidVal = Guid.Empty };
            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite,
                PersistenceAction.Query, filter).Trim();

            Assert.StartsWith("SELECT", result);
        }

        [Fact]
        public void Query_WithPopulatedGuid_ShouldNotFail()
        {
            var filter = new SampleTypesEntity { GuidVal = Guid.NewGuid() };
            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite,
                PersistenceAction.Query, filter).Trim();

            Assert.StartsWith("SELECT", result);
        }

        [Fact]
        public void Update_WithDateTime_ShouldUseNormalDateFormat()
        {
            var entity = new SampleEntity { Name = "x", CreationDate = new DateTime(2026, 1, 15) };
            var filter = new SampleEntity { Id = 3 };

            var result = EntitySqlParser.ParseEntity(entity, DatabaseEngine.SQLite,
                PersistenceAction.Update, filter).Trim();

            Assert.StartsWith("UPDATE", result);
            Assert.Contains("'2026-01-15'", result);
        }

        #endregion

        #region Numeric Range Upper Bound

        [Fact]
        public void Query_NumericRange_OnlyUpperBound_ShouldUseLessOrEqual()
        {
            var filter = new SampleEntity()
            {
                AgeEnd = 65
            };

            var result = EntitySqlParser.ParseEntity(filter, DatabaseEngine.SQLite, PersistenceAction.Query, filter);
            result = result.Trim();

            Assert.Contains("<=", result);
            Assert.DoesNotContain("BETWEEN", result);
        }

        #endregion

        #region Array Filterable Search (LIKE array path)

        [Fact]
        public void Search_WithArrayFilterable_ShouldUseParameterizedLike()
        {
            var filterEntity = (SampleArrayEntity)EntityReflector.GetFilterByFilterableColumns(
                typeof(SampleArrayEntity), typeof(SampleArrayEntity).GetProperties(), "busca");
            var sqlParameters = new Dictionary<string, object>();

            var result = EntitySqlParser.ParseEntity(filterEntity, DatabaseEngine.SQLite,
                PersistenceAction.Query, filterEntity, sqlParameters: sqlParameters).Trim();

            Assert.StartsWith("SELECT", result);
            Assert.Contains("LIKE", result);
            Assert.Contains("@p", result);
        }

        #endregion
    }
}