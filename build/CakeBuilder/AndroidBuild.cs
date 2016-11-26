using Cake.AndroidAppManifest;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Common.Tools;
using Cake.Common.Tools.MSBuild;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Core.IO.Arguments;
using Cake.Common.Tools.XBuild;
using System;
using System.IO;
using System.Linq;

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
            var configuration = Cake.Argument("Configuration", "Release");

            Cake.Information("Building '{0}' with '{1}' configuration", csproj, configuration);

            var outputPath = outputDir.Path.MakeAbsolute(Cake.Environment).FullPath + "/" + csproj.Path.GetFilenameWithoutExtension().FullPath + "_" + buildNumber + "/";

            var target = "SignAndroidPackage";

            if (Cake.IsRunningOnWindows())
            {
                Cake.MSBuild(csproj,
                    new MSBuildSettings()
                    .WithProperty("OutputPath", outputPath)
                    .SetConfiguration(configuration)
                    .WithTarget(target));
            }
            else
            {
                Cake.XBuild(csproj, 
                    new XBuildSettings()
                    .WithProperty("OutputPath", outputPath)
                    .SetConfiguration(configuration)
                    .WithTarget(target));

                //var searchPattern = "/**/*-Signed.apk";

                //// Use the globber to find any .apk files within the tree
                //var t = Cake.Globber
                //    .GetFiles(searchPattern)
                //    .OrderBy(f => new FileInfo(f.FullPath).LastWriteTimeUtc)
                //    .FirstOrDefault();
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
