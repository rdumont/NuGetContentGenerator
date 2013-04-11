using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace RDumont.NugetContentGenerator.Runtime
{
    using Microsoft.Win32;

    [Guid("EBD2692A-A869-4D3C-B8F4-37E66265D2BE")]
    public class NuGetContentGenerator : IVsSingleFileGenerator
    {
        private const string CustomToolName = "NuGetContentGenerator";
        private const string CustomToolDescription = "Inserts replacement tokens and converts the file to .pp";

        private const string VisualStudioVersion = "9.0";
        internal static Guid CSharpCategoryGuid = new Guid("FAE04EC1-301F-11D3-BF4B-00C04F79EFBC");

        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".pp";
            return pbstrDefaultExtension.Length;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            try
            {
                var generator = new Generator();
                var contents = generator.Generate(wszInputFilePath, bstrInputFileContents);

                var bytes = Encoding.UTF8.GetBytes(contents);
                var length = bytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);
                Marshal.Copy(bytes, 0, rgbOutputFileContents[0], length);
                pcbOutput = (uint) length;

                return VSConstants.S_OK;
            }
            catch (Exception ex)
            {
                pGenerateProgress.GeneratorError(0, 1, ex.ToString(), 0, 0);
                pcbOutput = 0;
                return VSConstants.S_FALSE;
            }
        }

        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {
            GuidAttribute guidAttribute = GetGuidAttribute(t);
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(
              GetKeyName(CSharpCategoryGuid, CustomToolName)))
            {
                key.SetValue("", CustomToolDescription);
                key.SetValue("CLSID", "{" + guidAttribute.Value + "}");
                key.SetValue("GeneratesDesignTimeSource", 1);
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
            Registry.LocalMachine.DeleteSubKey(GetKeyName(
              CSharpCategoryGuid, CustomToolName), false);
        }

        internal static GuidAttribute GetGuidAttribute(Type t)
        {
            return (GuidAttribute)GetAttribute(t, typeof(GuidAttribute));
        }

        internal static Attribute GetAttribute(Type t, Type attributeType)
        {
            object[] attributes = t.GetCustomAttributes(attributeType, /* inherit */ true);
            if (attributes.Length == 0)
                throw new Exception(
                  String.Format("Class '{0}' does not provide a '{1}' attribute.",
                  t.FullName, attributeType.FullName));
            return (Attribute)attributes[0];
        }

        internal static string GetKeyName(Guid categoryGuid, string toolName)
        {
            return
              String.Format("SOFTWARE\\Microsoft\\VisualStudio\\" + VisualStudioVersion +
                "\\Generators\\{{{0}}}\\{1}\\", categoryGuid, toolName);
        }
    }
}
