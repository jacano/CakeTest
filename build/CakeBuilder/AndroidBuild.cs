using Cake.AndroidAppManifest;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Common.Tools.MSBuild;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;
using Cake.Xamarin;
using System;

namespace CakeBuilder
{
    //[AddPath("tools")]
    public class AndroidBuild : PlatformBuild
    { 
        public AndroidBuild()
        {
            BuildHelper.InitCommon(this);

            BuildHelper.Clear(this);

            var sln = Cake.File("src/App3.sln");
            BuildHelper.RestoreNuget(this, sln);

            var manifest = Cake.File("src/App3/Properties/AndroidManifest.xml");
            UpdateAndroidManifest(manifest);

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

            if (Cake.IsRunningOnWindows())
            {
                Cake.MSBuild(csproj,
                    new MSBuildSettings()
                    .WithProperty("OutputPath", outputPath)
                    .SetConfiguration(configuration));
            }
            else
            {
                Cake.AndroidPackage(csproj, true, (x) =>
                {
                    
                });
            }
        }

        private void UpdateAndroidManifest(ConvertableFilePath manifestFile)
        {
            var manifest = Cake.DeserializeAppManifest(manifestFile);

            manifest.VersionName = this.buildName;
            manifest.VersionCode = int.Parse(this.buildNumber);

            Cake.SerializeAppManifest(manifestFile, manifest);
        }
    }
}
