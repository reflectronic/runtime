// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    /// This delegate is used to resolve object names when performing
    /// serialization and deserialization.
    /// </summary>
    public delegate void ResolveNameEventHandler(object? sender, ResolveNameEventArgs e);
}
