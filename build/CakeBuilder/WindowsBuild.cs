using Cake.Common.IO;
using Cake.Common.Tools.MSBuild;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Common.IO.Paths;

namespace CakeBuilder
{
    //[AddPath("tools")]
    public class WindowsBuild : PlatformBuild
    {
        public WindowsBuild()
        {
            BuildHelper.InitCommon(this);

            BuildHelper.Clear(this);

            var sln = Cake.File("src/App3.sln");
            BuildHelper.RestoreNuget(this, sln);

            var csproj = Cake.File("src/App3/App3.csproj");
            Build(csproj);

            BuildHelper.GenerateArtifactsAndMove(this);

            BuildHelper.UploadToTeamCity(this);

            Task("Default");
        }

        private void Build(ConvertableFilePath csproj)
        {
            Cake.Information("Building '{0}' with '{1}' configuration", csproj, configuration);

            var outputPath = outputDir.Path.MakeAbsolute(Cake.Environment).FullPath + "/" + csproj.Path.GetFilenameWithoutExtension().FullPath + "_" + buildNumber + "/";

            Cake.MSBuild(csproj,
                new MSBuildSettings()
                .WithProperty("OutputPath", outputPath)
                .SetConfiguration(configuration));
        }
    }
}
