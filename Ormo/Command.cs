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
    /// Abstract class to be inherited by an arbitraty command (script that alters the data, not queries it).
    /// </summary>
    /// <inheritdoc cref="ScriptedActionBase" path="/typeparam[@name='TP']" />
    public abstract class Command<TP> : ScriptedActionBase<TP>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="scriptProvider">Script provider to use to load command script.</param>
        /// <inheritdoc cref="ScriptedActionBase" path="/param[@name='fieldNameConverter']" />
        protected Command(IScriptProvider scriptProvider, IClassToDatabaseFieldNameConverter? fieldNameConverter = null) : base(fieldNameConverter)
        {
            LoadScript("Commands." + GetType().Name, scriptProvider);
        }

        /// <summary>
        /// Runs the command asynchronously.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <returns><see langword="true"/> if the command was run successfully, <see langword="false"/> otherwise.</returns>
        public async Task<bool> RunAsync(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                AddParametersToDbCommand(command);

#pragma warning disable CA2100
                command.CommandText = Script;
#pragma warning restore CA2100

                int affected = 0;
                try
                {
                    affected = await command.ExecuteNonQueryAsync();
                }
                catch
                {
                    return false;
                }
                return RowsAffectedProcessor(affected);
            }
        }

        /// <summary>
        /// Runs the command synchronously.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <returns><see langword="true"/> if the command was run successfully, <see langword="false"/> otherwise.</returns>
        public bool Run(DbConnection connection)
        {
            return RunAsync(connection).GetAwaiter().GetResult();
        }

        /// <summary>
        /// The rows affected processor routine to override, that analyzes the number and returns if the command was run successfully based on this analysis.
        /// </summary>
        /// <param name="rowsAffected">Reader that provides raw query output for each row.</param>
        /// <returns><see langword="true"/> if the command was run successfully, <see langword="false"/> otherwise.</returns>
        protected virtual bool RowsAffectedProcessor(int rowsAffected)
        {
            return true;
        }
    }
}
