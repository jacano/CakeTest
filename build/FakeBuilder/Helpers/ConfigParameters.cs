using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeBuilder.Helpers
{
    public class ConfigParameters
    {
        // General Settings
        public string SlnPath { get; set; }

        public string CsprojPath { get; set; }

        public string OutputFolder { get; set; }

        public string ArtifactsFolder { get; set; }

        public string MajorVersion { get; set; }

        public string MinorVersion { get; set; }

        public string ProjectName { get; set; }

        public string TestProjectName { get; set; }

        // Windows Installer
        public string AIPFilePath { get; set; }

        public string ExeFile { get; set; }

        public string ExeFilePath { get; set; }

        /// <summary>
        /// Where AdvanvedInstaller will generate installer file
        /// </summary>
        public string InstallerSetup { get; set; }

        /// <summary>
        /// File with commands for AdvanvedInstaller
        /// </summary>
        public string PathToCommands { get; set; }

        /// <summary>
        /// Build name for AdvanvedInstaller to compile
        /// </summary>
        public string BuildName { get; set; }
    }
}
