namespace Rivet.Core
{
    public class RivetConfig
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<RivetDependency> Packages { get; set; }

        public RivetConfig()
        {
            Version = "0.0.1";
            Name = "Name";
            Packages = new();
        }
    }
}