using System.Diagnostics;
using System.IO;
using Xunit;

namespace Cshart.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var dotGraph = Chart.Expand<UnitTest1>(x => x.Test1());

            // NOTE: it could be useful to introduce tests,
            // which compare the current emitted PNG
            // to the latest one
            var fileName = $"{nameof(UnitTest1)}.{nameof(Test1)}";
            var path = fileName + ".dotgraph";
            File.WriteAllText(path, dotGraph);
            var pci =
                new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "dot",
                    Arguments = $@"-T png -o {fileName + ".png"} ""{path}""",
                };
            Process.Start(pci).WaitForExit();
        }
    }
}