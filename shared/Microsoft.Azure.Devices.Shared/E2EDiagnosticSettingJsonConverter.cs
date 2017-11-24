// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Converts <see cref="E2EDiagnosticSetting"/> to Json
    /// </summary>
    public sealed class E2EDiagnosticSettingJsonConverter : JsonConverter
    {
        const string SamplingPercentageJsonTag = "__e2e_diag_sample_rate";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            E2EDiagnosticSetting e2eDiagnosticSetting = value as E2EDiagnosticSetting;

            if (e2eDiagnosticSetting == null)
            {
                throw new InvalidOperationException("Object passed is not of type E2EDiagnostic.");
            }

            writer.WriteStartObject();

            writer.WritePropertyName(SamplingPercentageJsonTag);
            writer.WriteValue(e2eDiagnosticSetting.SamplingRate);
            
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            E2EDiagnosticSetting e2eDiagnosticSetting = null;

            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                string propertyName = reader.Value as string;
                reader.Read();

                switch (propertyName)
                {
                    case SamplingPercentageJsonTag:
                        int updatedIntegerValue;
                        if (reader.Value != null && Int32.TryParse(reader.Value.ToString(), out updatedIntegerValue))
                        {
                            e2eDiagnosticSetting = new E2EDiagnosticSetting(updatedIntegerValue);
                        }
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return e2eDiagnosticSetting;
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType) => typeof(E2EDiagnosticSetting).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
    }
}
