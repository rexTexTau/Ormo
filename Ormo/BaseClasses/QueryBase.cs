// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com

namespace Ormo.BaseClasses
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Ormo.CaseConverters;
    using Ormo.ScriptProviders;

    /// <summary>
    /// Base class for queries.
    /// </summary>
    /// <inheritdoc cref="ScriptedActionBase" path="/typeparam[@name='TP']" />
    /// <typeparam name="TR">
    /// Resulting data type.
    /// </typeparam>
    public class QueryBase<TP, TR> : ScriptedActionBase<TP>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBase"/> class.
        /// </summary>
        /// <inheritdoc cref="ScriptedActionBase" path="/param[@name='fieldNameConverter']" />
        public QueryBase(IScriptProvider? scriptProvider = null, IClassToDatabaseFieldNameConverter? fieldNameConverter = null) : base(fieldNameConverter)
        {
            var provider = scriptProvider ?? OrmoConfiguration.Global.DefaultQueryScriptProvider;
            if (provider == null)
                throw new ArgumentNullException(nameof(scriptProvider));
            LoadScript("Queries." + GetType().Name, provider);
        }

        /// <summary>
        /// The record processor routine, that transfroms output to typed and most convenient result for consumer to use.
        /// </summary>
        /// <remarks>
        /// Override it if you need more fast DbDataReader fields mapping (without using RTTI).
        /// </remarks>
        /// <param name="reader">Reader that provides raw query output.</param>
        /// <returns>Typed data.</returns>
        protected virtual TR RecordProcessor(DbDataReader reader)
        {
            var type = typeof(TR);

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                type = underlyingType;

            if (type.IsClass && type.Name != "String")
            {
                var ctor = type.GetConstructor(new Type[] { });
                var result = (TR)ctor.Invoke(new object[] { });

                var properties = type.GetProperties().Where(p => p.CanRead);
                foreach (var property in properties)
                {
                    var propertyName = FieldNameConverter.Convert(property.Name);
                    var index = -1;
                    try
                    {
                        index = reader.GetOrdinal(propertyName);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        index = -1;
                    }
                    if (index >= 0)
                    {
                        var propertyValue = reader.GetValue(index);
                        property.SetValue(result, Convert.ChangeType(propertyValue, property.PropertyType));
                    }
                }
                return result;
            }
            else
            {
                return reader.GetFieldValue<TR>(0);
            }
        }
    }
}
