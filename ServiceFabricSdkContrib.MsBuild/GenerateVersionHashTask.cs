﻿using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ServiceFabricSdkContrib.Common;
using System;
using System.Collections.Generic;
using System.Fabric.Management.ServiceModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ServiceFabricSdkContrib.MsBuild
{
    public class GenerateVersionHashTask : Task
    {
        public string TargetDir { get; set; }
        public string IntermediateOutputPath { get; set; }
        public string BasePath { get; set; }

        public override bool Execute()
        {
            SHA512Managed sha;
            int offset = 0;

            List<byte> configHash = new List<byte>();
            var srv = Helper.serviceFromFile(Path.Combine(Path.GetDirectoryName(BasePath), "PackageRoot", "ServiceManifest.xml"));

            if (srv.CodePackage != null)
                foreach (var cv in srv.CodePackage)
                {
                    sha = new SHA512Managed();
                    offset = 0;

                    var path = cv.Name == "Code" ? TargetDir : Path.Combine(Path.GetDirectoryName(BasePath), "PackageRoot", cv.Name);

                    foreach (var f in Directory.GetFiles(path, "*.dll").Concat(Directory.GetFiles(path, "*.exe")))
                    {
                        var b = File.ReadAllBytes(f);
                        sha.TransformBlock(b, offset, b.Length, b, offset);
                    }

                    sha.TransformFinalBlock(new byte[0], offset, 0);
                    cv.Version += "." + Uri.EscapeDataString(Convert.ToBase64String(sha.Hash));
                    configHash.AddRange(sha.Hash);
                }

            if (srv.ConfigPackage != null)
                foreach (var cv in srv.ConfigPackage)
                {
                    sha = new SHA512Managed();
                    offset = 0;

                    foreach (var f in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(BasePath), "PackageRoot", cv.Name)))
                    {
                        var b = File.ReadAllBytes(f);
                        sha.TransformBlock(b, offset, b.Length, b, offset);
                    }

                    sha.TransformFinalBlock(new byte[0], offset, 0);
                    cv.Version += "." + Uri.EscapeDataString(Convert.ToBase64String(sha.Hash));
                    configHash.AddRange(sha.Hash);
                }

            if (srv.DataPackage != null)
                foreach (var cv in srv.DataPackage)
                {
                    sha = new SHA512Managed();
                    offset = 0;

                    foreach (var f in Directory.GetFiles(Path.Combine(Path.GetDirectoryName(BasePath), "PackageRoot", cv.Name)))
                    {
                        var b = File.ReadAllBytes(f);
                        sha.TransformBlock(b, offset, b.Length, b, offset);
                    }

                    sha.TransformFinalBlock(new byte[0], offset, 0);
                    cv.Version += "." + Uri.EscapeDataString(Convert.ToBase64String(sha.Hash));
                    configHash.AddRange(sha.Hash);
                }

            srv.Version += "." + Uri.EscapeDataString(Convert.ToBase64String(new SHA512Managed().ComputeHash(configHash.ToArray())));
            Helper.SaveService(Path.Combine(Path.GetDirectoryName(BasePath), IntermediateOutputPath, "ServiceManifest.xml"), srv);

            return true;
        }
    }
}
