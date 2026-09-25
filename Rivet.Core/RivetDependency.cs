namespace Rivet.Core
{
    public class RivetDependency
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public RivetDependency()
        {
            Name = "";
            Version = "";
        }
    }
}