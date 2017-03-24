using FSharpx;
using System;

using static Fake.RestorePackageHelper;
using static Fake.TargetHelper;
using static Fake.MSBuildHelper;
using static Fake.VSTest;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FakeBuilder.Helpers;

namespace FakeBuilder
{
    public class WindowsInstaller : Windows
    {
        private string installer;
        private string editCommnad;
        private string executeCommnad;
        private string sufix;

        #region Public Methods
        public WindowsInstaller()
        {
            Target("new", FSharpFunc.FromAction(() =>
            {
                GenerateInstaller(this.Args);
                GenerateArtifacts();
            }));

            Target("uploadInstaller", FSharpFunc.FromAction(() =>
            {
                UploadToTeamCity();
            }));

            //dependency("Build", "Clean");
        }

        public override void GenerateArtifacts()
        {
            if (!Directory.Exists(ConfigHelper.Instance.ArtifactsFolder))
            {
                Directory.CreateDirectory(ConfigHelper.Instance.ArtifactsFolder);
            }

            var exeName = $"{ConfigHelper.Instance.ProjectName}-{this.fileVersion}.exe";
            var renamedExeName = $"{ConfigHelper.Instance.ProjectName}{this.sufix}-{this.fileVersion}.exe";
            var pathToInstaller = Path.GetFullPath(Path.Combine(ConfigHelper.Instance.InstallerSetup, exeName));
            var pathToArtifact = Path.GetFullPath(Path.Combine(ConfigHelper.Instance.ArtifactsFolder, renamedExeName));
            Console.WriteLine($"Moving installer: {pathToInstaller} to {pathToArtifact}");
            File.Move(pathToInstaller, pathToArtifact);
        }


        public override void InitCommon()
        {
            base.InitCommon();
            this.installer = "advancedInstaller.com";
            this.editCommnad = $"/edit {ConfigHelper.Instance.AIPFilePath}";
            this.executeCommnad = $"/execute {ConfigHelper.Instance.AIPFilePath}";
        }
        #endregion

        #region Private Region        
        private void GenerateInstaller(IEnumerable<string> args)
        {
            if (args.Count() < 1)
            {
                throw new Exception($"No name given to installer:{args}");
            }

            this.sufix = args.ElementAt(0);
            this.SetVersion(fileVersion);
            this.AddDirectory(solutionOutput);
            string[] fileExtensions = { "*.exe", "*.dll", "*.zip", "*.config" };
            this.AddFile(solutionOutput, fileExtensions);
            this.SaveAndRebuild();
        }

        private bool UpdateExe(string solutionOutput)
        {
            var pathToExe = Path.GetFullPath(Path.Combine(solutionOutput, ConfigHelper.Instance.ExeFile));
            return this.execute($"{this.editCommnad} /UpdateFile APPDIR {ConfigHelper.Instance.ExeFile} {pathToExe} -overwrite always");
        }

        private bool SetVersion(string fileVersion)
        {
            return this.execute($"{this.editCommnad} /SetVersion {fileVersion}");
        }

        private bool AddFile(string path, string[] searchPattern)
        {
            var result = true;
            foreach (var pattern in searchPattern)
            {
                result = AddFile(path, pattern);
            }

            return result;

        }

        private bool AddFile(string path, string pattern = "*")
        {
            var result = true;
            foreach (var item in Directory.GetFiles(path, pattern))
            {
                if (!this.execute($"{this.editCommnad} /AddFile APPDIR {Path.GetFullPath(item)}"))
                {
                    result = false;
                }
            }

            return result;
        }

        private bool AddDirectory(string path, string[] searchPattern)
        {
            var result = true;
            foreach (var pattern in searchPattern)
            {
                result = AddDirectory(path, pattern);
            }

            return result;
        }

        private bool AddDirectory(string path, string pattern = "*")
        {
            var result = true;
            foreach (var item in Directory.GetDirectories(path, pattern))
            {
                if (!this.execute($"{this.editCommnad} /AddFolder APPDIR {Path.GetFullPath(item)}"))
                {
                    result = false;
                }
            }

            return result;
        }

        private bool SaveAndRebuild()
        {
            return this.execute($"{this.executeCommnad} {Path.GetFullPath(ConfigHelper.Instance.PathToCommands)}");
        }

        private bool execute(string cmd)
        {
            var result = true;

            var returnValue = PlatfomHelper.Execute(this.installer, cmd).GetAwaiter().GetResult();
            result = returnValue == 0;
            return result;
        }
        #endregion

    }
}
