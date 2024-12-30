// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.Tests.Unit
{
    using Microsoft.Data.Sqlite;

    /// <summary>
    /// Sqlite test (in-memory) DB connector.
    /// </summary>
    internal sealed class TestSqlite : IDisposable
    {
        private const string MEMORY_CONNECTION_STRING_FORMAT = "Data Source={0};Mode=Memory";

        private SqliteConnection? _connection;

        /// <summary>
        /// Gets the active connection to DB or creates a new one.
        /// </summary>
        public SqliteConnection Connection
        {
            get
            {
                if (_connection == null ||
                    _connection.State == System.Data.ConnectionState.Closed ||
                    _connection.State == System.Data.ConnectionState.Broken)
                {
                    _connection = new SqliteConnection(string.Format(MEMORY_CONNECTION_STRING_FORMAT, "InMemoryTestDb"));
                    _connection.Open();
                }

                return _connection;
            }
        }

        /// <summary>
        /// Closes and disposes the connection to in-memory DB.
        /// </summary>
        /// <remarks>
        /// When connection to in-memory DB is closed, the DB is destroyed.
        /// </remarks>
        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
