using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LVMS.FactuurSturen
{
    public class LowerCaseStringEnumConverter : StringEnumConverter
    {
        public LowerCaseStringEnumConverter() : base()
        {
        }
        
        //
        // Summary:
        //     Writes the JSON representation of the object.
        //
        // Parameters:
        //   writer:
        //     The Newtonsoft.Json.JsonWriter to write to.
        //
        //   value:
        //     The value.
        //
        //   serializer:
        //     The calling serializer.
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;

            string enumName = e.ToString("G");

            if (char.IsNumber(enumName[0]) || enumName[0] == '-')
            {
                // enum value has no name so write number
                writer.WriteValue(value);
            }
            else
            {
                Type enumType = e.GetType();

                string finalName = enumName.ToLowerInvariant();

                writer.WriteValue(finalName);
            }

        }
    }

    public class UpperCaseStringEnumConverter : StringEnumConverter
    {
        public UpperCaseStringEnumConverter() : base()
        {
        }

        //
        // Summary:
        //     Writes the JSON representation of the object.
        //
        // Parameters:
        //   writer:
        //     The Newtonsoft.Json.JsonWriter to write to.
        //
        //   value:
        //     The value.
        //
        //   serializer:
        //     The calling serializer.
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;

            string enumName = e.ToString("G");

            if (char.IsNumber(enumName[0]) || enumName[0] == '-')
            {
                // enum value has no name so write number
                writer.WriteValue(value);
            }
            else
            {
                Type enumType = e.GetType();

                string finalName = enumName.ToUpperInvariant();

                writer.WriteValue(finalName);
            }

        }
    }
}