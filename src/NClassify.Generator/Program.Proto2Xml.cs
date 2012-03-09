#region Copyright (c) 2012 Roger O Knapp
//  Permission is hereby granted, free of charge, to any person obtaining a copy 
//  of this software and associated documentation files (the "Software"), to deal 
//  in the Software without restriction, including without limitation the rights 
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
//  copies of the Software, and to permit persons to whom the Software is 
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in 
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//  IN THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Google.ProtocolBuffers;
using Google.ProtocolBuffers.DescriptorProtos;
using CSharpTest.Net.IO;
using CSharpTest.Net.Processes;
using NClassify.Generator.CodeWriters;

namespace NClassify.Generator
{
    partial class Program
    {
        public void Proto2Xml(string[] proto, string output, string protoc)
        {
            string[] ignore;
            Proto2Xml(proto, output, protoc, out ignore);
        }

        public void ProtoGen(string[] proto, string output, string protoc, [DefaultValue(null)] string nameSpace, [DefaultValue(".cs")] string ext)
        {
            string[] files;
            Proto2Xml(proto, output, protoc, out files);
            GenerateSource(files, nameSpace, ext, out files);
        }

        void Proto2Xml(string[] proto, string output, string protoc, out string[] sourceFiles)
        {
            TempFile tempFile = null;
            if (proto.Length == 0)
                throw new ApplicationException("Please specify one or more proto files.");

            if (!Directory.Exists(output))
                throw new DirectoryNotFoundException("Unable to locate output directory: " + output);
            output = Path.GetFullPath(output);

            string[] descriptors = proto.Where(f => !f.EndsWith(".proto")).ToArray();
            proto = proto.Where(f => f.EndsWith(".proto")).ToArray();

            if (proto.Length > 0)
            {
                using (ProcessRunner program = new ProcessRunner(protoc))
                {
                    proto = proto.SelectMany(
                            p => Directory.GetFiles(Path.GetDirectoryName(p) ?? Environment.CurrentDirectory, Path.GetFileName(p))
                                 .Select(fp => Path.GetFullPath(fp))
                        ).ToArray();
                    string dir = Path.GetDirectoryName(proto[0]);
                    foreach (string path in proto)
                        while (!String.IsNullOrEmpty(dir) && !path.StartsWith(dir, StringComparison.OrdinalIgnoreCase))
                            dir = Path.GetDirectoryName(dir);

                    List<string> args = new List<string>();
                    tempFile = new TempFile();
                    args.Add("--include_imports");
                    args.Add("--error_format=msvs");
                    args.Add("--descriptor_set_out=" + tempFile.TempPath);
                    args.Add("--proto_path=" + dir);
                    args.AddRange(proto);

                    program.OutputReceived += (o, e) => { Console.Error.WriteLine(e.Data); };
                    int result = program.Run(args.ToArray());
                    if (result != 0)
                        throw new ApplicationException("Protoc.exe returned error " + result);

                    descriptors = descriptors.Concat(new[] {tempFile.TempPath}).ToArray();
                }
            }

            List<FileDescriptorProto> files = new List<FileDescriptorProto>(
                descriptors.SelectMany(
                    f => FileDescriptorSet.CreateBuilder()
                        .MergeFrom(File.ReadAllBytes(f))
                        .FileList
                    )
                );

            if (tempFile != null)
                tempFile.Dispose();

            XmlSerializer xserialize = new XmlSerializer(typeof (NClassifyConfig));
            Dictionary<string, IMessageLite> types = new Dictionary<string, IMessageLite>(StringComparer.Ordinal);
            foreach (FileDescriptorProto fd in files.Distinct(new FileDescriptorByName()))
            {
                AddScope(types, fd.Package, fd.EnumTypeList, fd.MessageTypeList);
            }

            List<string> sources = new List<string>();
            foreach (FileDescriptorProto fd in files.Distinct(new FileDescriptorByName()))
            {
                NClassifyConfig cfg = new NClassifyConfig();
                cfg.Settings.Namespace = String.Join(".", fd.Package.Split('.')
                                                         .Select(s => CodeWriter.ToPascalCase(s))
                                                         .ToArray());
                List<RootItem> items = new List<RootItem>();
                items.AddRange(
                    fd.DependencyList.Select(
                        i => new ImportFile {File = Path.ChangeExtension(Path.GetFileName(i), ".xml")}
                        ).ToArray()
                    );

                items.AddRange(Convert(fd.EnumTypeList));
                items.AddRange(Convert(fd.MessageTypeList, fd.Package.TrimStart('.'), types));
                items.AddRange(Convert(fd.ServiceList, fd.Package.TrimStart('.')));
                //items.AddRange(Convert(fd.SourceCodeInfo));

                cfg.Items = items.ToArray();
                string path = Path.Combine(output, Path.ChangeExtension(fd.Name, ".xml"));
                using(XmlWriter wtr = XmlWriter.Create(path, new XmlWriterSettings{ Indent = true, Encoding = new UTF8Encoding(false) }))
                    xserialize.Serialize(wtr, cfg);
                sources.Add(path);
            }

            sourceFiles = sources.ToArray();
        }

        private void AddScope(Dictionary<string, IMessageLite> types, string scope, IList<EnumDescriptorProto> enums, IList<DescriptorProto> messages)
        {
            foreach (var e in enums)
                types.Add(String.Format("{0}.{1}", scope, e.Name).TrimStart('.'), e);
            foreach (var m in messages)
            {
                string fqn = String.Format("{0}.{1}", scope, m.Name).TrimStart('.');
                types.Add(fqn, m);
                AddScope(types, fqn, m.EnumTypeList, m.NestedTypeList);
            }
        }

        private ComplexType[] Convert(IList<DescriptorProto> messages, string scope, Dictionary<string, IMessageLite> types)
        {
            return messages.Select(
                m =>
                new ComplexType
                    {
                        Name = m.Name,
                        Fields = m.FieldList.Select(f => Convert(f, scope, types)).ToArray(),
                        ChildTypes = 
                            Convert(m.EnumTypeList).OfType<BaseType>()
                            .Concat(Convert(m.NestedTypeList, scope, types))
                            .ToArray()
                    }
                )
                .ToArray();
        }

        private FieldInfo Convert(FieldDescriptorProto field, string scope, Dictionary<string, IMessageLite> types)
        {
            FieldInfo fld;
            if (field.HasTypeName)
            {
                string fqn = ScopeName(field.TypeName, scope);

                IMessageLite mtype;
                if (types.TryGetValue(field.TypeName.TrimStart('.'), out mtype) && mtype is EnumDescriptorProto)
                    fld = new EnumTypeRef { TypeName = fqn };
                else
                    fld = new ComplexTypeRef { TypeName = fqn };
            }
            else if (field.HasType)
            {
                fld = new Primitive
                          {
                              Type = MapType(field.Type)
                          };
            }
            else
                throw new ArgumentOutOfRangeException();

            fld.Name = field.Name;
            fld.FieldId = (uint)field.Number;
            fld.Access = FieldAccess.Public;
            fld.FieldUse = (field.Label == FieldDescriptorProto.Types.Label.LABEL_REQUIRED) ? FieldUse.Required : FieldUse.Optional;
            fld.IsArray = field.Label == FieldDescriptorProto.Types.Label.LABEL_REPEATED;
            fld.FieldDirection = FieldDirection.Bidirectional;
            if (field.Options.Deprecated)
                fld.FieldUse |= FieldUse.Obsolete;
            if (field.HasDefaultValue)
                fld.DefaultValue = field.DefaultValue;

            return fld;
        }

        private FieldType MapType(FieldDescriptorProto.Types.Type type)
        {
            switch(type)
            {
                case FieldDescriptorProto.Types.Type.TYPE_DOUBLE:
                    return FieldType.Double;
                case FieldDescriptorProto.Types.Type.TYPE_FLOAT:
                    return FieldType.Float;
                case FieldDescriptorProto.Types.Type.TYPE_INT64:
                    return FieldType.Int64;
                case FieldDescriptorProto.Types.Type.TYPE_UINT64:
                    return FieldType.UInt64;
                case FieldDescriptorProto.Types.Type.TYPE_INT32:
                    return FieldType.Int32;
                case FieldDescriptorProto.Types.Type.TYPE_FIXED64:
                    return FieldType.UInt64;
                case FieldDescriptorProto.Types.Type.TYPE_FIXED32:
                    return FieldType.UInt32;
                case FieldDescriptorProto.Types.Type.TYPE_BOOL:
                    return FieldType.Boolean;
                case FieldDescriptorProto.Types.Type.TYPE_STRING:
                    return FieldType.String;
                case FieldDescriptorProto.Types.Type.TYPE_BYTES:
                    return FieldType.Bytes;
                case FieldDescriptorProto.Types.Type.TYPE_UINT32:
                    return FieldType.UInt32;
                case FieldDescriptorProto.Types.Type.TYPE_SFIXED32:
                    return FieldType.Int32;
                case FieldDescriptorProto.Types.Type.TYPE_SFIXED64:
                    return FieldType.Int64;
                case FieldDescriptorProto.Types.Type.TYPE_SINT32:
                    return FieldType.Int32;
                case FieldDescriptorProto.Types.Type.TYPE_SINT64:
                    return FieldType.Int64;
                case FieldDescriptorProto.Types.Type.TYPE_ENUM:
                case FieldDescriptorProto.Types.Type.TYPE_GROUP:
                case FieldDescriptorProto.Types.Type.TYPE_MESSAGE:
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private string ScopeName(string fqn, string scope)
        {
            fqn = fqn.TrimStart('.');
            if (fqn.StartsWith(scope + '.', StringComparison.Ordinal))
                fqn = fqn.Substring(scope.Length + 1);
            return fqn;
        }

        private ServiceInfo[] Convert(IList<ServiceDescriptorProto> services, string scope)
        {
            return services.Select(
                s =>
                new ServiceInfo
                    {
                        Name = s.Name,
                        Methods = s.MethodList.Select(
                            m => new ServiceMethod
                                     {
                                         Name = m.Name,
                                         Response = ScopeName(m.OutputType, scope),
                                         Request = ScopeName(m.InputType, scope)
                                     }
                            ).ToArray()
                    }
                )
                .ToArray();
        }

        private static EnumType[] Convert(IEnumerable<EnumDescriptorProto> enums)
        {
            return enums.Select(
                e => new EnumType
                         {
                             Name = e.Name,
                             Values = e.ValueList
                                 .Select(ev => new EnumType.Item {Name = ev.Name, Value = (uint) ev.Number})
                                 .ToArray()
                         }
                ).ToArray();
        }

    }

    internal class FileDescriptorByName : IEqualityComparer<FileDescriptorProto>
    {
        public bool Equals(FileDescriptorProto x, FileDescriptorProto y)
        {
            return x.Package == y.Package && x.Name == y.Name;
        }

        public int GetHashCode(FileDescriptorProto obj)
        {
            return (obj.Package + '|' + obj.Name).GetHashCode();
        }
    }
}
