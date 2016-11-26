using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Tools.XBuild;
using Cake.Core.IO.Arguments;
using Cake.Plist;

namespace CakeBuilder
{
    //[AddPath("tools")]
    public class IOSBuild : PlatformBuild
    {
        public IOSBuild()
        {
            BuildHelper.InitCommon(this);

            BuildHelper.Clear(this);

            var sln = Cake.File("src/App2.sln");
            BuildHelper.RestoreNuget(this, sln);

            var plistFile = Cake.File("src/App2/Info.plist");
            UpdateApplePlist(plistFile);

            var csproj = Cake.File("src/App2/App2.csproj");
            Build(csproj);

            BuildHelper.GenerateArtifactsAndMove(this);

            BuildHelper.UploadToTeamCity(this);

            Task("Default");
        }

        private void Build(ConvertableFilePath csproj)
        {
            var configuration = Cake.Argument("Configuration", "Release");

            Cake.Information("Building '{0}' with '{1}' configuration", csproj, configuration);

            var outputPath = outputDir.Path.MakeAbsolute(Cake.Environment).FullPath + "/" + csproj.Path.GetFilenameWithoutExtension().FullPath + "_" + buildNumber + "/";

            if (Cake.IsRunningOnWindows())
            {
                Cake.MSBuild(csproj,
                    new MSBuildSettings()
                    .WithProperty("OutputPath", outputPath)
                    .SetConfiguration(configuration));
            }
            else
            {
                Cake.XBuild(csproj,
                    new XBuildSettings()
                    .WithProperty("OutputPath", outputPath)
                    .SetConfiguration(configuration));
            }
        }

        private void UpdateApplePlist(ConvertableFilePath plistFile)
        {
            dynamic plist = Cake.DeserializePlist(plistFile);

            plist["CFBundleVersion"] = this.buildName;
            plist["CFBundleShortVersionString"] = int.Parse(this.buildNumber);

            Cake.SerializePlist(plistFile, (object)plist);
        }
    }
}
