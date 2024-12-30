// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.Tests.Unit.Commands
{
    /// <summary>
    /// Parameters class for <see cref="ResourceCommand"/>
    /// </summary>
    internal class ResourceCommandParameters
    {
        /// <summary>
        /// Integer id to insert into table.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Textual value to insert into table.
        /// </summary>
        public string? Value { get; set; }
    }
}
