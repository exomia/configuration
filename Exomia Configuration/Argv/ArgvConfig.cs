namespace Exomia.Configuration.Argv
{
    /// <inheritdoc />
    public sealed class ArgvConfig : ConfigBase
    {
        internal ArgvConfig(IConfigSource configSource, string name, string comment = "")
            : base(configSource, name, comment) { }
    }
}