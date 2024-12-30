// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo
{
    using System.Collections;
    using System.Data.Common;
    using Ormo.ScriptProviders;

    /// <summary>
    /// Base class for Commands and Queries containing common fields and methods.
    /// </summary>
    /// <typeparam name="TP">
    /// Type of the data class or primitive type used as parameters source. Use Nothing if no parameters required. Never use IEnumerable descendants.
    /// </typeparam>
    public class ScriptedActionBase<TP>
    {
        /// <summary>
        /// Script to execute.
        /// </summary>
        protected string? Script { get; private set; }

        /// <summary>
        /// Script's parameters.
        /// </summary>
        internal IDictionary<string, object>? Parameters { get; set; }

        /// <summary>
        /// Checks if value parameter is null and returns DbNull.Value in that case.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>DbNull.Value if value param is null, the value itself othrewise.</returns>
        internal static object NullToDbNull(object? value)
        {
            return value == null ? DBNull.Value : value;
        }

        /// <summary>
        /// Loads the script with scriptName using scriptProvider.
        /// </summary>
        /// <summary>
        /// The result is stored as Script property value.
        /// </summary>
        /// <param name="scriptName">Name of the script.</param>
        /// <param name="scriptProvider">Provider of scripts.</param>
        /// <exception cref="KeyNotFoundException">Is thrown if script with scriptName is not present in scriptProvider.</exception>
        internal void LoadScript(string scriptName, IScriptProvider scriptProvider)
        {
            Script = scriptProvider.Get(scriptName);
            if (Script == null)
                throw new KeyNotFoundException("Null script got from scripts provider using name \"{scriptName}\"");
        }

        /// <summary>
        /// Setup routine, that adds script parameters from data variable.
        /// </summary>
        /// <remarks>
        /// Script paremeters that were added before calling this routine will be removed.
        /// </remarks>
        /// <param name="data">Source of script's parameters.</param>
        /// <exception cref="ArgumentException">Thrown if data is IEnumerable/</exception>
        public void Setup(TP data)
        {
            Parameters = new Dictionary<string, object>();

            if (data is Nothing)
                return;

            var type = typeof(TP);

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                type = underlyingType;

            if (type.IsPrimitive)
            {
                Parameters.Add(type.Name.ToLowerInvariant(), NullToDbNull(data));
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                throw new ArgumentException(nameof(data));
            }
            else
            {
                var properties = type.GetProperties().Where(p => p.CanRead);

                foreach (var property in properties)
                {
                    string propertyName = property.Name.ToLowerInvariant();
                    object? propertyValue = property.GetValue(data);
                    Parameters.Add(propertyName, NullToDbNull(propertyValue));
                }
            }
        }

        /// <summary>
        /// Adds parameters from Parameters dictionary to DbCommand.
        /// </summary>
        /// <param name="command">DbCommand to add parameters to.</param>
        protected void AddParametersToDbCommand(DbCommand command)
        {
            if (Parameters != null)
            {
                foreach (var kvp in Parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@" + kvp.Key;
                    parameter.Value = kvp.Value;
                    command.Parameters.Add(parameter);
                }
            }
        }
    }
}
