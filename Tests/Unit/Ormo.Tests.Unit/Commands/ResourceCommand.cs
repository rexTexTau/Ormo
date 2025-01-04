// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.Tests.Unit.Commands
{
    using Ormo.ScriptProviders;

    /// <summary>
    /// Test command which uses script from resource.
    /// </summary>
    internal class ResourceCommand : Command<ResourceCommandParameters>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCommand"/> class.
        /// </summary>
        /// <param name="scriptsProvider">Script provider to use to get command script (should be <see cref="EmbededResourcesScriptProvider"/>).</param>
        public ResourceCommand(IScriptProvider? scriptsProvider = null) : base(scriptsProvider)
        {
        }

        /// <inheritdoc/>
        protected override bool RowsAffectedProcessor(int rowsAffected)
        {
            return rowsAffected == 1;
        }
    }
}
