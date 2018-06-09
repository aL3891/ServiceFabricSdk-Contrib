﻿// Decompiled with JetBrains decompiler
// Type: System.Fabric.Management.ServiceModel.CodePackageType
// Assembly: System.Fabric.Management.ServiceModel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C6D32D4D-966E-4EA3-BD3A-F4CF14D36DBC
// Assembly location: C:\Git\ServiceFabricSdkContrib\ServiceFabricSdkContrib.MsBuild\bin\Debug\netstandard2.0\publish\runtimes\win\lib\netstandard2.0\System.Fabric.Management.ServiceModel.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace System.Fabric.Management.ServiceModel
{
  [GeneratedCode("xsd", "4.0.30319.17929")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://schemas.microsoft.com/2011/01/fabric")]
  public class CodePackageType
  {
    private CodePackageTypeSetupEntryPoint setupEntryPointField;
    private EntryPointDescriptionType entryPointField;
    private EnvironmentVariableType[] environmentVariablesField;
    private string nameField;
    private string versionField;
    private bool isSharedField;

    public CodePackageType()
    {
      this.isSharedField = false;
    }

    public CodePackageTypeSetupEntryPoint SetupEntryPoint
    {
      get
      {
        return this.setupEntryPointField;
      }
      set
      {
        this.setupEntryPointField = value;
      }
    }

    public EntryPointDescriptionType EntryPoint
    {
      get
      {
        return this.entryPointField;
      }
      set
      {
        this.entryPointField = value;
      }
    }

    [XmlArrayItem("EnvironmentVariable", IsNullable = false)]
    public EnvironmentVariableType[] EnvironmentVariables
    {
      get
      {
        return this.environmentVariablesField;
      }
      set
      {
        this.environmentVariablesField = value;
      }
    }

    [XmlAttribute]
    public string Name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    [XmlAttribute]
    public string Version
    {
      get
      {
        return this.versionField;
      }
      set
      {
        this.versionField = value;
      }
    }

    [XmlAttribute]
    [DefaultValue(false)]
    public bool IsShared
    {
      get
      {
        return this.isSharedField;
      }
      set
      {
        this.isSharedField = value;
      }
    }
  }
}
