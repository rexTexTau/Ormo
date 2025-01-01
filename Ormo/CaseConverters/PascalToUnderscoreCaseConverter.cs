// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com

namespace Ormo.CaseConverters
{
    using System.Linq;

    /// <summary>
    /// Service class to convert class field names in PascalCase to database field names in underscore_case
    /// </summary>
    internal class PascalToUnderscoreCaseConverter : IClassToDatabaseFieldNameConverter
    {
        /// <inheritdoc/>
        public string Convert(string classFieldName)
        {
            return string.Concat(classFieldName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }
}
