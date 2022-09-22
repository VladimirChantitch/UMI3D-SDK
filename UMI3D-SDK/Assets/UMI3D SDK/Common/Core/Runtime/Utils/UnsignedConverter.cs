/*
Copyright 2019 - 2021 Inetum

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Newtonsoft.Json;
using System;

public class UnsignedConverter : Newtonsoft.Json.JsonConverter
{
    public override bool CanRead => true;

    public override bool CanWrite => true;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ulong) || objectType == typeof(uint) || objectType == typeof(ushort);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var value = reader?.Value;
        if (value == null)
            return 0;
        object ret = convert(value, objectType);
        return ret;
    }


    object convert(object value, Type objectType)
    {
        switch (true)
        {
            case true when objectType == typeof(ulong):
                var l = (long)value;
                if (l < 0)
                    return (UInt64)(ulong)(l + long.MaxValue);
                return (UInt64)((ulong)l + (ulong)long.MaxValue);

            case true when objectType == typeof(uint):
                var i = (int)(long)value;
                if (i < 0)
                    return (UInt32)(uint)(i + int.MaxValue);
                return (UInt32)((uint)i + (uint)int.MaxValue);

            case true when objectType == typeof(ushort):
                var s = (short)(long)value;
                if (s < 0)
                    return (UInt16)(ushort)(s + short.MaxValue);
                return (UInt16)((ushort)s + (ushort)short.MaxValue);

        }
        throw new Exception($"No case found for type {objectType.FullName} {value} [{value.GetType()}]");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        switch (value)
        {
            case ulong ul:
                if (ul < long.MaxValue)
                    writer.WriteValue(((long)ul - long.MaxValue));
                else
                    writer.WriteValue((long)(ul - long.MaxValue));
                break;
            case uint ul:
                if (ul < int.MaxValue)
                    writer.WriteValue(((int)ul - int.MaxValue));
                else
                    writer.WriteValue((int)(ul - int.MaxValue));
                break;
            case ushort ul:
                if (ul < short.MaxValue)
                    writer.WriteValue(((short)ul - short.MaxValue));
                else
                    writer.WriteValue((short)(ul - short.MaxValue));
                break;
        }
    }
}
