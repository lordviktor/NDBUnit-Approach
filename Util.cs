using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace NDbUnitQuery
{
    internal class Util
    {
        private const string rootNamespace = "NDbUnitXPath";
        
        internal static Stream ResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }

        internal static Stream AssemblyResourceStream(string relativeResourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( rootNamespace + "." + relativeResourceName);
            Assert.IsNotNull(stream);
            return stream;
        }
    }
}
