using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace VersionUpdateAndPublish
{
	class Program
	{
		private class Arguments
		{
			public string SourceAssembly;
			public string WixProjectFile;
			public string WixProjectFileVersionElement;
			public string WixFile;
			public string WixDefineName;
			public string Version;
		}
		static int Main(string[] args)
		{
			var input = new Arguments
			{
#if DEBUG
				SourceAssembly = @"..\Org.Lwes\bin\Debug\Org.Lwes.dll",
#else
				SourceAssembly = @"..\Org.Lwes\bin\Release\Org.Lwes.dll",
#endif
				WixProjectFile = @"..\LwesBinaryInstaller\LwesBinaryInstaller.wixproj",
				WixProjectFileVersionElement = @"VersionStamp",
				WixFile = "Library.wxs",
				WixDefineName = "VersionStamp",
			};

			if (TryReadArguments(args, input))
			{
				Assembly asm = Assembly.LoadFile(Path.GetFullPath(input.SourceAssembly));

				var v = asm.GetName().Version;
				input.Version = String.Concat(v.Major, '.', v.Minor, '.', v.Build, '.', v.Revision);

				// Set the version variable in the WiX project file...
				var prj = XDocument.Load(input.WixProjectFile);
				XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
				namespaceManager.AddNamespace("empty", prj.Root.GetDefaultNamespace().NamespaceName);
				var e = prj.Root.XPathSelectElement(String.Concat("//empty:", input.WixProjectFileVersionElement), namespaceManager);
				if (e != null)
				{
					if (!String.Equals(e.Value, input.Version))
					{
						e.Value = input.Version;
						prj.Save(input.WixProjectFile);
					}
				}

				// Set the version variable in all of the wix files (if defined)
				foreach (var f in input.WixFile.Split(';'))
				{
					var filePath = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(input.WixProjectFile)), f);
					var file = XDocument.Load(filePath);

					foreach (var n in file.Nodes())
					{
						var pi = (XProcessingInstruction)n as XProcessingInstruction;

						if (pi != null
							&& pi.Target == "define"
							&& pi.Data.StartsWith(input.WixDefineName))
						{
							string newData = String.Concat(input.WixDefineName, "=\"", input.Version, "\"");
							if (!String.Equals(pi.Data, newData))
							{
								pi.Data = newData;
								file.Save(filePath);
							}
							break;
						}
					}
				}
				return 0;
			}
			return -1;
		}

		private static bool TryReadArguments(string[] args, Arguments input)
		{
			foreach (var a in args)
			{
				if (a.StartsWith("-a:"))
				{
					string v = a.Substring(3);
					if (!File.Exists(v))
					{
						Console.WriteLine(String.Concat("assembly file does not exist: ", v));
						return false;
					}
					input.SourceAssembly = v;
				}
				else if (a.StartsWith("-assembly:"))
				{
					string v = a.Substring(10);
					if (!File.Exists(v))
					{
						Console.WriteLine(String.Concat("assembly file does not exist: ", v));
						return false;
					}
					input.SourceAssembly = v;
				}
				else if (a.StartsWith("-p:"))
				{
					string v = a.Substring(3);
					if (!File.Exists(v))
					{
						Console.WriteLine(String.Concat("WiX project file does not exist: ", v));
						return false;
					}
					input.WixProjectFile = v;
				}
				else if (a.StartsWith("-wix-prj:"))
				{
					string v = a.Substring(9);
					if (!File.Exists(v))
					{
						Console.WriteLine(String.Concat("WiX project file does not exist: ", v));
						return false;
					}
					input.WixProjectFile = v;
				}
				else if (a.StartsWith("-e:"))
				{
					input.WixProjectFileVersionElement = a.Substring(3);
				}
				else if (a.StartsWith("-wix-elem:"))
				{
					input.WixProjectFileVersionElement = a.Substring(10);
				}
				else if (a.StartsWith("-f:"))
				{
					input.WixFile = a.Substring(3);
				}
				else if (a.StartsWith("-wix-file:"))
				{
					input.WixFile = a.Substring(10);
				}
				else if (a.StartsWith("-d:"))
				{
					input.WixDefineName = a.Substring(3);
				}
				else if (a.StartsWith("-define-name"))
				{
					input.WixDefineName = a.Substring(12);
				}
				else if (a.StartsWith("-v:"))
				{
					input.Version = a.Substring(3);
				}
				else if (a.StartsWith("-version:"))
				{
					input.Version = a.Substring(9);
				}
				else
				{
					Console.WriteLine(String.Concat("Unexpected command line argument: ", a));
					return false;
				}
			}
			return true;
		}
	}
}
