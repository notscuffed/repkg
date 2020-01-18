using System;
using System.Collections.Generic;
using RePKG.Native.Package;
using RePKG.Native.Texture;

namespace RePKG.Native
{
    public static class TinyReflection
    {
        private static readonly Dictionary<Type, string> TypeNames;

        static TinyReflection()
        {
            TypeNames = new Dictionary<Type, string>();

            AddType<byte[]>("byte[]");
            AddType<byte>("byte");

            AddType<CTex>(nameof(CTex));
            AddType<CTexFrameInfo>(nameof(CTexFrameInfo));
            AddType<CTexFrameInfoContainer>(nameof(CTexFrameInfoContainer));
            AddType<CTexHeader>(nameof(CTexHeader));
            AddType<CTexImage>(nameof(CTexImage));
            AddType<CTexImageContainer>(nameof(CTexImageContainer));
            AddType<CTexMipmap>(nameof(CTexMipmap));

            AddType<CPackage>(nameof(CPackage));
            AddType<CPackageEntry>(nameof(CPackageEntry));

            AddType<CBytesResult>(nameof(CBytesResult));
        }

        private static void AddType<T>(string name)
        {
            TypeNames.Add(typeof(T), name);
        }

        public static string GetTypeName(object o)
        {
            if (TypeNames.TryGetValue(o.GetType(), out var name))
                return name;

            return "Unknown type";
        }

        public static string GetTypeName<T>()
        {
            if (TypeNames.TryGetValue(typeof(T), out var name))
                return name;

            return "Unknown type";
        }
    }
}