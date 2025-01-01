// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo
{
    using System.Data.Common;
    using System.Threading.Tasks;
    using Ormo.BaseClasses;
    using Ormo.CaseConverters;
    using Ormo.ScriptProviders;

    /// <summary>
    /// Abstract class to be inherited by an arbitraty query that returns a single value.
    /// </summary>
    /// <inheritdoc cref="ScriptedActionBase" path="/typeparam[@name='TP']" />
    /// <inheritdoc cref="ScriptedActionBase" path="/typeparam[@name='TR']" />
    /// Resulting data type.
    /// </typeparam>
    public abstract class QuerySingle<TP, TR> : QueryBase<TP, TR>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuerySingle"/> class.
        /// </summary>
        /// <param name="scriptProvider">Script provider to use to load query script.</param>
        /// <inheritdoc cref="ScriptedActionBase" path="/param[@name='fieldNameConverter']" />
        protected QuerySingle(IScriptProvider scriptProvider, IClassToDatabaseFieldNameConverter? fieldNameConverter = null) : base(fieldNameConverter)
        {
            LoadScript("Queries." + GetType().Name, scriptProvider);
        }

        /// <summary>
        /// Runs the query asynchronously.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <returns><see langword="true"/> if the query was run successfully, <see langword="false"/> otherwise.</returns>
        public async Task<TR> RunAsync(DbConnection connection)
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
#pragma warning disable CS8603 // Possible null reference return.
                    return default;
#pragma warning restore CS8603 // Possible null reference return.
                }
                if (reader != null)
                {
                    TR result = default;
                    if (reader.HasRows && await reader.ReadAsync())
                    {
                        result = RecordProcessor(reader);
                    }
                    await reader.DisposeAsync();
#pragma warning disable CS8603 // Possible null reference return.
                    return result;
#pragma warning restore CS8603 // Possible null reference return.
                }
#pragma warning disable CS8603 // Possible null reference return.
                return default;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        /// <summary>
        /// Runs the query synchronously.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <returns><see langword="true"/> if the query was run successfully, <see langword="false"/> otherwise.</returns>
        public TR Run(DbConnection connection)
        {
            return RunAsync(connection).GetAwaiter().GetResult();
        }
    }
}
