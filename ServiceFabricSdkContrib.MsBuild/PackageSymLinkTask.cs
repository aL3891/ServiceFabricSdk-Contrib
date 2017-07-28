﻿using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ServiceFabricSdkContrib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace ServiceFabricSdkContrib.MsBuild
{
    public class PackageSymLinkTask : Microsoft.Build.Utilities.Task
    {

        [DllImport("kernel32.dll")]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        public ITaskItem[] ProjectReferences { get; set; }
        public ITaskItem[] ServiceProjectReferences { get; set; }
        public string Configuration { get; set; }
        public string Platform { get; set; }
        public string ServicePackageRootFolder { get; set; }
        public string ApplicationManifestPath { get; set; }
        public string PackageBehavior { get; set; }
        public string PackageLocation { get; set; }
        public string ProjectPath { get; set; }

        public ITaskItem[] IncludeInPackagePaths { get; set; }

        public override bool Execute()
        {
            var basePath = Path.GetDirectoryName(ProjectPath);

            foreach (var spr in PatchMetadata(ProjectReferences, ServiceProjectReferences))
            {
                var serviceProjectPath = Path.GetFullPath(Path.Combine(basePath, spr.ItemSpec));
                var project = GetProject(serviceProjectPath);

                var codePath = project.GetPropertyValue("TargetDir");

                string servicePath = Path.Combine(basePath, PackageLocation, spr.GetMetadata("ServiceManifestName"));
                if (!Directory.Exists(servicePath))
                    Directory.CreateDirectory(servicePath);

                if (!Directory.Exists(Path.Combine(servicePath, spr.GetMetadata("CodePackageName"))))
                    CreateSymbolicLink(Path.Combine(servicePath, spr.GetMetadata("CodePackageName")), codePath, SymbolicLink.Directory);

                if (!Directory.Exists(Path.Combine(servicePath, "Config")))
                    CreateSymbolicLink(Path.Combine(servicePath, "Config"), Path.Combine(Path.GetDirectoryName(serviceProjectPath), "PackageRoot", "Config"), SymbolicLink.Directory);

                if (!File.Exists(Path.Combine(servicePath, "ServiceManifest.xml")))
                {
                    var manifestFile = Path.Combine(Path.GetDirectoryName(serviceProjectPath), project.GetPropertyValue("IntermediateOutputPath"), "ServiceManifest.xml");
                    if (File.Exists(manifestFile))
                    {
                        CreateSymbolicLink(Path.Combine(servicePath, "ServiceManifest.xml"), manifestFile, SymbolicLink.File);
                    }
                    else
                    {
                        manifestFile = Path.Combine(Path.GetDirectoryName(serviceProjectPath), "PackageRoot", "ServiceManifest.xml");
                        File.Copy(manifestFile, Path.Combine(servicePath, "ServiceManifest.xml"));
                    }
                }
            }

            File.Copy(Path.Combine(basePath, "ApplicationPackageRoot", "ApplicationManifest.xml"), Path.Combine(basePath, PackageLocation, "ApplicationManifest.xml"), true);
            var appManifest = Helper.FromFile(Path.Combine(basePath, "ApplicationPackageRoot", "ApplicationManifest.xml"));

            foreach (var serviceReference in appManifest.ServiceManifestImport)
            {
                var servicePath = Path.Combine(basePath, PackageLocation, serviceReference.ServiceManifestRef.ServiceManifestName, "ServiceManifest.xml");
                if (File.Exists(servicePath))
                {
                    var serviceManifest = Helper.serviceFromFile(servicePath);
                    serviceReference.ServiceManifestRef.ServiceManifestVersion = serviceManifest.Version;
                }
            }
            
            var aggregatedVersion = Uri.EscapeDataString(Convert.ToBase64String(MD5.Create().ComputeHash( Encoding.ASCII.GetBytes(string.Join("", appManifest.ServiceManifestImport.Select(ss => ss.ServiceManifestRef.ServiceManifestVersion))))));
            appManifest.ApplicationTypeVersion = appManifest.ApplicationTypeVersion + "." + aggregatedVersion;
            Helper.SaveApp(Path.Combine(basePath, PackageLocation, "ApplicationManifest.xml"), appManifest);

            return true;
        }

        private IEnumerable<ITaskItem> PatchMetadata(IEnumerable<ITaskItem> projectReferences, IEnumerable<ITaskItem> serviceProjectReferences)
        {
            if (serviceProjectReferences == null)
                serviceProjectReferences = Enumerable.Empty<ITaskItem>();

            if (projectReferences == null)
                projectReferences = Enumerable.Empty<ITaskItem>();

            var res = projectReferences.Where(p => !serviceProjectReferences.Any(spr => spr.ItemSpec == p.ItemSpec)).ToList();

            foreach (var r in res)
            {
                var manifestFile = Path.Combine(Path.GetDirectoryName(r.ItemSpec), "PackageRoot", "ServiceManifest.xml");

                r.SetMetadata("ServiceManifestName", Helper.serviceFromFile(manifestFile).Name);
                r.SetMetadata("CodePackageName", "Code");
            }

            return serviceProjectReferences.Concat(res).ToList();
        }

        public Project GetProject(string projectfile)
        {
            return ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(p => p.FullPath.ToLower() == projectfile.ToLower()) ?? new Project(projectfile);
        }
    }

    enum SymbolicLink
    {
        File = 0,
        Directory = 1
    }
}

