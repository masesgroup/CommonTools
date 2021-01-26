/*
*  Copyright 2021 MASES s.r.l.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*  http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*  Refer to LICENSE for more information.
*/

using System;
using System.IO;
using System.Text;

namespace MASES.CommonTools.VersionGenerator
{
    class Program
    {
        static Version DeployVersion = new Version(1, 0, 0, 0);
        static string VersionFilename = "ComponentVersion.cs";
        static string VersionFilenameCpp = "ComponentVersion.h";
        const string LicensedProductVersionTemplate = "ProductCurrentVersion = ";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("VersionGenerator <path-to-Version.cs or product-version> <destination-path or file> <git-revision>");
                    Environment.Exit(1);
                }

                string searchPath = string.Empty;
                if (!Version.TryParse(args[0], out DeployVersion))
                {
                    searchPath = args[0];
                }

                var destinationPath = Environment.CurrentDirectory;
                var gitPath = Environment.CurrentDirectory;
                destinationPath = args[1];

                if (!string.IsNullOrEmpty(searchPath))
                {
                    var str = File.ReadAllText(searchPath);
                    var startIndex = str.IndexOf(LicensedProductVersionTemplate);
                    str = str.Substring(startIndex + LicensedProductVersionTemplate.Length + 1);
                    var endIndex = str.IndexOf("\";");
                    str = str.Substring(0, endIndex);
                    if (!string.IsNullOrEmpty(str))
                    {
                        DeployVersion = new Version(str);
                    }
                }

                string headShortRevision = args[2];
                var logString = headShortRevision;
                if (Directory.Exists(destinationPath) && !string.IsNullOrEmpty(headShortRevision))
                {
                    var template = string.Empty;
                    using (var streamReader = new StreamReader(typeof(Program).Assembly.GetManifestResourceStream("MASES.CommonTools.VersionGenerator." + VersionFilename)))
                    {
                        byte[] array = new byte[streamReader.BaseStream.Length];
                        streamReader.BaseStream.Read(array, 0, (int)streamReader.BaseStream.Length);
                        template = Encoding.ASCII.GetString(array);
                    }

                    var fileName = Path.Combine(destinationPath, VersionFilename);
                    var dt = DateTime.Now;
                    int revision = (dt.Year - 2000) * 10000 + dt.Month * 100 + dt.Day;
                    Version version = new Version(DeployVersion.Major, DeployVersion.Minor, DeployVersion.Build, revision);
                    string componentInformationalVersion = version + "-" + headShortRevision;
                    var commentInfo = string.Format("// {0}", version);
                    var classContent = string.Format(template, version, componentInformationalVersion, logString);
                    File.WriteAllText(fileName, classContent);
                }

                if (Directory.Exists(destinationPath) && !string.IsNullOrEmpty(headShortRevision))
                {
                    var template = string.Empty;
                    using (var streamReader = new StreamReader(typeof(Program).Assembly.GetManifestResourceStream("MASES.CommonTools.VersionGenerator." + VersionFilenameCpp)))
                    {
                        byte[] array = new byte[streamReader.BaseStream.Length];
                        streamReader.BaseStream.Read(array, 0, (int)streamReader.BaseStream.Length);
                        template = Encoding.ASCII.GetString(array);
                    }

                    var fileName = Path.Combine(destinationPath, VersionFilenameCpp);
                    var dt = DateTime.Now;
                    int revision = (dt.Year - 2000) * 10000 + dt.Month * 100 + dt.Day;
                    Version version = new Version(DeployVersion.Major, DeployVersion.Minor, DeployVersion.Build, revision);
                    string componentInformationalVersion = version + "-" + headShortRevision;
                    var commentInfo = string.Format("// {0}", version);
                    var classContent = string.Format(template, version, version.ToString().Replace('.', ','), componentInformationalVersion, logString);
                    File.WriteAllText(fileName, classContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
