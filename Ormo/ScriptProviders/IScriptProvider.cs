﻿// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.ScriptProviders
{
    /// <summary>
    /// Interface for a script provider to implement.
    /// </summary>
    public interface IScriptProvider
    {
        /// <summary>
        /// Gets script by name.
        /// </summary>
        /// <param name="name">Script name</param>
        /// <returns>Script text</returns>
        string? Get(string name);
    }
}
