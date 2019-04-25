using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RunProcessAsTask
{
    // these overloads match the ones in Process.Start to make it a simpler transition for callers
    // see http://msdn.microsoft.com/en-us/library/system.diagnostics.process.start.aspx
    public partial class ProcessEx
    {
        public static Task<ProcessResults> RunAsync(string fileName)
        {
            return RunAsync(new ProcessStartInfo(fileName));
        }

        public static Task<ProcessResults> RunAsync(string fileName, string arguments)
        {
            Console.WriteLine("In Process Ex");
            return RunAsync(new ProcessStartInfo(fileName, arguments));
        }

        public static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo)
        {
            return RunAsync(processStartInfo, CancellationToken.None);
        }

        public static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken)
        {
            Console.WriteLine("In Process Ex 2");
            return RunAsync(processStartInfo, new List<string>(), new List<string>(), cancellationToken);
        }
    }
}
