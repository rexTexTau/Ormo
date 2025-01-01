// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com

namespace Ormo.CaseConverters
{
    /// <summary>
    /// Interface to implement by classes that convert class field names to database field names
    /// </summary>
    public interface IClassToDatabaseFieldNameConverter
    {
        /// <summary>
        /// Converts class field name to database field name.
        /// </summary>
        /// <param name="classFieldName">Class field name to convert.</param>
        /// <returns>Database field name.</returns>
        string Convert(string classFieldName);
    }
}
