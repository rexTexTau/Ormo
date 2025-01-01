// (c) 202// (c) 2024 George Volsky. All rights reserved
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
    /// Test for <see cref="QueryMultiple"/> base class (using its childs).
    /// </summary>
    [Collection("Sequential")]
    public sealed class QueryMultipleTest : IDisposable
    {
        private readonly FolderScriptProvider _provider;
        private readonly TestSqlite _testSqlite;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryMultipleTest"/> class.
        /// </summary>
        /// <remarks>
        /// Creates script provider and test Sqlite in-memory database.
        /// </remarks>
        public QueryMultipleTest()
        {
            _provider = new FolderScriptProvider(AppContext.BaseDirectory);
            _testSqlite = new TestSqlite();
        }

        /// <summary>
        /// Test method to check query returns empty collection on empty database.
        /// </summary>
        [Fact]
        public void Execute_EmptyDatabase_ReturnsEmptySetOfTables()
        {
            var result = new FileQuery(_provider).Run(_testSqlite.Connection);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Test method to check query returns table names after tables are created.
        /// </summary>
        [Fact]
        public void Execute_AfterTablesCreation_ReturnsExpectedSetOfTables()
        {
            new FileCommand(_provider).Run(_testSqlite.Connection);
            var result = new FileQuery(_provider).Run(_testSqlite.Connection).ToArray();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(["another_table_from_file", "table_from_file"], result);
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
