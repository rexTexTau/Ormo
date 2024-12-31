// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Ormo.ScriptProviders;

    /// <summary>
    /// Abstract class to be inherited by an arbitraty query that can return multiple rows of data.
    /// </summary>
    /// <inheritdoc cref="ScriptedActionBase" path="/typeparam[@name='TP']" />
    /// <typeparam name="TR">
    /// Resulting row data type.
    /// </typeparam>
    public abstract class QueryMultiple<TP, TR> : ScriptedActionBase<TP>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryMultiple"/> class.
        /// </summary>
        /// <param name="scriptProvider">Script provider to use to load query script.</param>
        protected QueryMultiple(IScriptProvider scriptProvider)
        {
            LoadScript("Queries." + GetType().Name, scriptProvider);
        }

        /// <summary>
        /// Runs the query asynchronously.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <returns><see langword="true"/> if the query was run successfully, <see langword="false"/> otherwise.</returns>
        public async IAsyncEnumerable<TR> RunAsync(DbConnection connection)
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
                    yield break;
                }
                if (reader != null)
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            yield return RecordProcessor(reader);
                        }
                    }
                    await reader.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// Runs the query asynchronously.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <returns><see langword="true"/> if the query was run successfully, <see langword="false"/> otherwise.</returns>
        public IEnumerable<TR> Run(DbConnection connection)
        {
            foreach (var element in RunAsync(connection).ToEnumerable())
            {
                yield return element;
            }
        }

        /// <summary>
        /// The record processor routine to override, that transfroms each output row to typed and most convenient result for consumer to use.
        /// </summary>
        /// <param name="reader">Reader that provides raw query output for each row.</param>
        /// <returns>Typed data row.</returns>
        protected abstract TR RecordProcessor(DbDataReader reader);
    }
}
