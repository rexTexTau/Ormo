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
    using System.Reflection;

    /// <summary>
    /// Script provider that loads scripts embedded into an assembly.
    /// </summary>
    public class EmbededResourcesScriptProvider : IScriptProvider
    {
        /// <summary>
        /// Scripts storage.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        internal readonly Dictionary<string, string> _storage = new Dictionary<string, string>();
#pragma warning restore SA1401 // Fields should be private

        private readonly string _assemblyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbededResourcesScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">Assembly to load embedded scripts from.</param>
        public EmbededResourcesScriptProvider(Assembly assembly)
        {
            _assemblyName = Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name);
            var names = assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                using (var s = assembly.GetManifestResourceStream(name))
                {
                    if (s != null)
                    {
                        var value = new StreamReader(s).ReadToEnd();
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
            }
        }

        /// <inheritdoc/>
        public string? Get(string name)
        {
            var resourceName = _assemblyName + "." + name + ".sql";
            return _storage.ContainsKey(resourceName) ?
                _storage[resourceName] :
                null;
        }
    }
}
