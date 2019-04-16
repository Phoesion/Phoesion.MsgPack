// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;

#pragma warning disable SA1649 // File name should match first type name

namespace Phoesion.MsgPack.Formatters
{
    public sealed class TypeFormatter : IMessagePackFormatter<Type>
    {
        public static readonly IMessagePackFormatter<Type> Instance = new TypeFormatter();

        static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

        // mscorlib or System.Private.CoreLib
        static readonly bool isMscorlib = typeof(int).AssemblyQualifiedName.Contains("mscorlib");


        public void Serialize(ref MessagePackWriter writer, Type value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else if (value == typeof(string) || value == typeof(string[]) || value.IsPrimitive)
            {
                writer.Write(value.FullName);
            }
            else
            {
                writer.Write(value.FullName + ", " + value.Assembly.GetName().Name);
            }
        }

        public Type Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                var name = reader.ReadString();
                if (typeCache.TryGetValue(name, out Type type))
                {
                    return type;
                }
                else
                {
                    lock (typeCache)
                    {
                        // mscorlib or System.Private.CoreLib fixes
                        string fixedName;
                        if (isMscorlib && name.Contains("System.Private.CoreLib"))
                        {
                            fixedName = name.Replace("System.Private.CoreLib", "mscorlib");
                        }
                        else if (!isMscorlib && name.Contains("mscorlib"))
                        {
                            fixedName = name.Replace("mscorlib", "System.Private.CoreLib");
                        }
                        else
                        {
                            fixedName = name;
                        }
                        //get type
                        try { type = Type.GetType(name); } catch { return null; }
                        typeCache[name] = type;
                        return type;
                    }
                }
            }
        }
    }
}
