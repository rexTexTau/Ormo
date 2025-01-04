// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com

namespace Ormo
{
    using System.Data.Common;
    using System.Threading;
    using Ormo.CaseConverters;
    using Ormo.ScriptProviders;

    /// <summary>
    /// Ormo global configuration singleton.
    /// </summary>
    public sealed class OrmoConfiguration
    {
        private static readonly object SyncRoot = new object();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private static OrmoConfiguration instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        private OrmoConfiguration()
        {
            DefaultClassToDatabaseFieldNameConverter = new PascalToUnderscoreCaseConverter();
        }

        /// <summary>
        /// Global configuration instance.
        /// </summary>
        public static OrmoConfiguration Global
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                Monitor.Enter(SyncRoot);
                var temp = new OrmoConfiguration();
#pragma warning disable CS8601 // Possible null reference assignment.
                Interlocked.Exchange(ref instance, temp);
#pragma warning restore CS8601 // Possible null reference assignment.
                Monitor.Exit(SyncRoot);
#pragma warning disable CS8603 // Possible null reference return.
                return instance;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        /// <summary>
        /// Default DB connection to use for queries.
        /// </summary>
        /// <remarks>
        /// Used if no connection is passed to Run query method.
        /// </remarks>
        public DbConnection? DefaultQueryConnection { get; set; }

        /// <summary>
        /// Default DB connection to use for commands.
        /// </summary>
        /// <remarks>
        /// Used if no connection is passed to Run command method.
        /// </remarks>
        public DbConnection? DefaultCommandConnection { get; set; }

        /// <summary>
        /// Default queries' script provider.
        /// </summary>
        /// <remarks>
        /// Used if no script provider is passed to query ctor.
        /// </remarks>
        public IScriptProvider? DefaultQueryScriptProvider { get; set; }

        /// <summary>
        /// Default commands' script provider.
        /// </summary>
        /// <remarks>
        /// Used if no script provider is passed to command ctor.
        /// </remarks>
        public IScriptProvider? DefaultCommandScriptProvider { get; set; }

        /// <summary>
        /// Default field name converter for both queries and commands.
        /// </summary>
        /// <remarks>
        /// Used if no field name converter is passed to query/command ctor.
        /// </remarks>
        public IClassToDatabaseFieldNameConverter DefaultClassToDatabaseFieldNameConverter { get; set; }
    }
}
