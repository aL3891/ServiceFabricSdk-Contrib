﻿// Decompiled with JetBrains decompiler
// Type: System.Fabric.Management.ServiceModel.ExtensionsTypeExtension
// Assembly: System.Fabric.Management.ServiceModel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C6D32D4D-966E-4EA3-BD3A-F4CF14D36DBC
// Assembly location: C:\Git\ServiceFabricSdkContrib\ServiceFabricSdkContrib.MsBuild\bin\Debug\netstandard2.0\publish\runtimes\win\lib\netstandard2.0\System.Fabric.Management.ServiceModel.dll

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace System.Fabric.Management.ServiceModel
{
  [GeneratedCode("xsd", "4.0.30319.17929")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/2011/01/fabric")]
  public class ExtensionsTypeExtension
  {
    private XmlElement anyField;
    private string nameField;
    private string generatedIdField;

    [XmlAnyElement]
    public XmlElement Any
    {
      get
      {
        return this.anyField;
      }
      set
      {
        this.anyField = value;
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
    public string GeneratedId
    {
      get
      {
        return this.generatedIdField;
      }
      set
      {
        this.generatedIdField = value;
      }
    }
  }
}