// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.ScriptProviders
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Script provider that loads scripts from a folder.
    /// </summary>
    internal class FolderScriptProvider : IScriptProvider
    {
        /// <summary>
        /// Scripts storage.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        internal readonly Dictionary<string, string> _storage = new Dictionary<string, string>();
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderScriptProvider"/> class.
        /// </summary>
        /// <param name="folder">Directory to load scripts from (recursive, using nested subdirs).</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if directory specified by folder parameter cannot be found.</exception>
        public FolderScriptProvider(string folder)
        {
            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException(folder);

            foreach (var sqlFilePath in Directory.GetFiles(
                folder,
                "*.sql",
                SearchOption.AllDirectories))
            {
                var value = File.ReadAllText(sqlFilePath);
                var name = Path.GetRelativePath(folder, sqlFilePath).Replace('\\', '.');
                if (_storage.ContainsKey(name))
                {
                    _storage[name] = value;
                }
                else
                {
                    _storage.Add(name, value);
                }
            }
        }

        /// <inheritdoc/>
        public string? Get(string name)
        {
            var resourceName = name + ".sql";
            return _storage.ContainsKey(resourceName) ?
                _storage[resourceName] :
                null;
        }
    }
}
