using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Dapplo.Hosting.Sample.Common
{
    public static class ProgramUtility
    {
        public static string GetExecutableFilePath()
        {
            string result = null;

#if NET6_0_OR_GREATER
            result = Environment.ProcessPath;
#else
            result = Process.GetCurrentProcess().MainModule.FileName;
#endif

            return result;
        }

        public static string GetExecutableDirectoryName()
        {
            /*
            return AppContext.BaseDirectory;
            return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            */

            return AppContext.BaseDirectory;
        }
    }
}
