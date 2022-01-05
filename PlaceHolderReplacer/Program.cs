/*
*  Copyright 2022 MASES s.r.l.
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

namespace MASES.CommonTools.PlaceHolderReplacer
{
    class Program
    {
        static Version DeployVersion = new Version(1, 0, 0, 0);
        const string LicensedProductVersionTemplate = "ProductCurrentVersion = ";
        const string LicenseVersionPlaceHolder = "__VERSION_PLACEHOLDER__";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("PlaceHolderReplacer <path-to-Version.cs or product-version> <file-to-update> <version-trailer>");
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

                var deployVersion = string.Format("{0}.{1}.{2}", DeployVersion.Major, DeployVersion.Minor, DeployVersion.Build);

                string versionToPut = deployVersion;
                if (args.Length == 3)
                {
                    versionToPut += args[2];
                }

                var fileContent = File.ReadAllText(args[1]);
                fileContent = fileContent.Replace(LicenseVersionPlaceHolder, versionToPut);
                File.WriteAllText(args[1], fileContent);
                Console.WriteLine(deployVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
