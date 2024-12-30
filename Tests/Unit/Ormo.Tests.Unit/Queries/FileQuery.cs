// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.Tests.Unit.Queries
{
    using System.Data.Common;
    using Ormo.ScriptProviders;

    /// <summary>
    /// Test query which uses script from file.
    /// </summary>
    internal class FileQuery : QueryMultiple<Nothing, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileQuery"/> class.
        /// </summary>
        /// <param name="scriptsProvider">Script provider to use to get command script (should be <see cref="FolderScriptProvider"/>).</param>
        public FileQuery(IScriptProvider scriptsProvider) : base(scriptsProvider)
        {
        }

        /// <inheritdoc/>
        protected override string RecordProcessor(DbDataReader reader)
        {
            return reader.GetString(0);
        }
    }
}
