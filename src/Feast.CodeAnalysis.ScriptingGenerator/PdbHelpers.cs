// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Reflection;
using Microsoft.CodeAnalysis.Emit;

namespace Feast.CodeAnalysis.Scripting
{
    [Literal("Feast.CodeAnalysis.Scripting.Generates.PdbHelpers")]
    internal static class PdbHelpers
    {
        private static readonly Lazy<Func<object>> Method = new(() =>
        {
            var m = Global.GetAssembly("Microsoft.CodeAnalysis.Scripting")?
                .GetType("Microsoft.CodeAnalysis.Scripting.PdbHelpers")?
                .GetMethod(nameof(GetPlatformSpecificDebugInformationFormat),
                    BindingFlags.Static | BindingFlags.Public);
            if (m is null)
            {
                throw new InvalidOperationException("Could not find GetPlatformSpecificDebugInformationFormat method in Microsoft.CodeAnalysis.Scripting.PdbHelpers.");
            }

            return () => m.Invoke(null, []);
        });
        
        public static DebugInformationFormat GetPlatformSpecificDebugInformationFormat()
        {
            return (DebugInformationFormat)Method.Value();
        }
    }
}