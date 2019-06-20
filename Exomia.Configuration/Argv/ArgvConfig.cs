#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Configuration.Argv
{
    /// <summary>
    ///     An argv configuration. This class cannot be inherited.
    /// </summary>
    public sealed class ArgvConfig : ConfigBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ArgvConfig" /> class.
        /// </summary>
        /// <param name="configSource"> The configuration source. </param>
        /// <param name="name">         The name. </param>
        /// <param name="comment">      (Optional) The comment. </param>
        internal ArgvConfig(IConfigSource configSource, string name, string comment = "")
            : base(configSource, name, comment) { }
    }
}