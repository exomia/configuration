using System;

namespace Exomia.Configuration.Argv
{
    /// <inheritdoc />
    public sealed class ArgvConfigSource : ConfigSourceBase
    {
        /// <inheritdoc />
        protected override IConfig CreateConfig(string section, string comment = "")
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