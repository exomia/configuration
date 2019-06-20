#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Configuration.Argv
{
    /// <summary>
    ///     An argv configuration source. This class cannot be inherited.
    /// </summary>
    public sealed class ArgvConfigSource : ConfigSourceBase
    {
        /// <inheritdoc />
        protected override IConfig CreateConfig(string section, string comment)
        {
            return new ArgvConfig(this, section, comment);
        }

        /// <inheritdoc />
        protected override void OnReload()
        {
            throw new InvalidOperationException("can't reload an ArgvConfigSource");
        }

        /// <inheritdoc />
        protected override void OnSave()
        {
            throw new InvalidOperationException("can't save an ArgvConfigSource");
        }
    }
}