using Cake.Common.IO.Paths;
using CodeCake;

namespace CakeBuilder
{
    public class PlatformBuild : CodeCakeHost
    {
        public ConvertableDirectoryPath outputDir;
        public ConvertableDirectoryPath toolsDir;
        public ConvertableDirectoryPath artifactDir;

        public string buildNumber;
        public string buildName;
    }
}