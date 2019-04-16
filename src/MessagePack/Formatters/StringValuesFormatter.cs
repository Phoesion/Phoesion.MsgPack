// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

#pragma warning disable SA1649 // File name should match first type name

namespace Phoesion.MsgPack.Formatters
{
    public sealed class StringValuesFormatter : IMessagePackFormatter<StringValues>
    {
        public static readonly IMessagePackFormatter<StringValues> Instance = new StringValuesFormatter();

        public void Serialize(ref MessagePackWriter writer, StringValues value, MessagePackSerializerOptions options)
        {
            if (value.Count == 0)
            {
                writer.WriteNil();
            }
            else
            {
                writer.Write(value.Count);
                for (int n = 0; n < value.Count; n++)
                {
                    writer.Write(value[n]);
                }
            }
        }

        public StringValues Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return StringValues.Empty;
            }
            else
            {
                var count = reader.ReadInt32();
                if (count == 1)
                {
                    return new StringValues(reader.ReadString());
                }
                else
                {
                    var arr = new string[count];
                    for (int n = 0; n < count; n++)
                    {
                        arr[n] = reader.ReadString();
                    }

                    return new StringValues(arr);
                }
            }
        }
    }
}

