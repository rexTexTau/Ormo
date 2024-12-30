﻿// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.Tests.Unit
{
    using Ormo.ScriptProviders;
    using Ormo.Tests.Unit.Commands;
    using Ormo.Tests.Unit.Queries;

    /// <summary>
    /// Test for <see cref="QuerySingle"/> base class (using its childs).
    /// </summary>
    [Collection("Sequential")]
    public sealed class QuerySingleTest : IDisposable
    {
        private readonly EmbededResourcesScriptProvider _provider;
        private readonly TestSqlite _testSqlite;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySingleTest"/> class.
        /// </summary>
        /// <remarks>
        /// Creates script provider and test Sqlite in-memory database.
        /// </remarks>
        public QuerySingleTest()
        {
            _provider = new EmbededResourcesScriptProvider(typeof(QuerySingleTest).Assembly);
            _testSqlite = new TestSqlite();
        }

        /// <summary>
        /// Test method to check query returns null on empty database.
        /// </summary>
        [Fact]
        public void Run_EmptyDatabase_ReturnsNull()
        {
            var query = new ResourceQuery(_provider);
            query.Setup(1);

            var result = query.Run(_testSqlite.Connection);

            Assert.Null(result);
        }

        /// <summary>
        /// Test method to check query returns data from table after table is created and data stored.
        /// </summary>
        [Fact]
        public void Run_AfterTableCreationAndDataInsertion_ReturnsExpectedValue()
        {
            var command = new ResourceCommand(_provider);
            command.Setup(new ResourceCommandParameters { Id = 1, Value = "Test" });
            command.Run(_testSqlite.Connection);
            var query = new ResourceQuery(_provider);
            query.Setup(1);

            var result = query.Run(_testSqlite.Connection);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("Test", result);
        }

        /// <summary>
        /// Flushes the in-memory DB.
        /// </summary>
        public void Dispose()
        {
            _testSqlite.Dispose();
        }
    }
}