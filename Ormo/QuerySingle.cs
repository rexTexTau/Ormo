// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo
{
    using System.Data.Common;
    using Ormo.ScriptProviders;

    /// <summary>
    /// Abstract class to be inherited by an arbitraty query that returns a single value.
    /// </summary>
    /// <inheritdoc cref="ScriptedActionBase" path="/typeparam[@name='TP']" />
    /// <typeparam name="TR">
    /// Resulting data type.
    /// </typeparam>
    public abstract class QuerySingle<TP, TR> : ScriptedActionBase<TP>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySingle"/> class.
        /// </summary>
        /// <param name="scriptProvider">Script provider to use to load query script.</param>
        protected QuerySingle(IScriptProvider scriptProvider)
        {
            LoadScript("Queries." + GetType().Name, scriptProvider);
        }

        /// <summary>
        /// Runs the query asynchronously.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <returns><see langword="true"/> if the query was run successfully, <see langword="false"/> otherwise.</returns>
        public async Task<TR?> RunAsync(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                AddParametersToDbCommand(command);

#pragma warning disable CA2100
                command.CommandText = Script;
#pragma warning restore CA2100

                DbDataReader? reader = null;
                try
                {
                    reader = await command.ExecuteReaderAsync();
                }
                catch
                {
                    return default;
                }
                if (reader != null)
                {
                    TR? result = default;
                    if (reader.HasRows && await reader.ReadAsync())
                    {
                        result = RecordProcessor(reader);
                    }
                    await reader.DisposeAsync();
                    return result;
                }
                return default;
            }
        }

        /// <summary>
        /// Runs the query synchronously.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <returns><see langword="true"/> if the query was run successfully, <see langword="false"/> otherwise.</returns>
        public TR? Run(DbConnection connection)
        {
            return RunAsync(connection).GetAwaiter().GetResult();
        }

        /// <summary>
        /// The record processor routine to override, that transfroms output to typed and most convenient result for consumer to use.
        /// </summary>
        /// <param name="reader">Reader that provides raw query output.</param>
        /// <returns>Typed data.</returns>
        protected abstract TR RecordProcessor(DbDataReader reader);
    }
}
