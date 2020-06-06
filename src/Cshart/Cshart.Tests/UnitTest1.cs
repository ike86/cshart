using System;
using System.Diagnostics;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Cshart.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var dotGraph = Chart.Expand<UnitTest1>(x => x.Test1());

            // NOTE: it is really annoying to test this way
            dotGraph.Should().StartWith(
                @"digraph ""UnitTest1.Test1()"" { " +
                @"""Test1""[label=""Test1"",shape=ellipse]; " +
                @"""GetTypeFromHandle""[label=""GetTypeFromHandle"",shape=ellipse]; " +
                @"""Test1"" -> ""GetTypeFromHandle""[arrowhead=normal]; " +
                @"""Parameter""[label=""Parameter"",shape=ellipse]; " +
                @"""Test1"" -> ""Parameter""[arrowhead=normal]; " +
                @"""Test1""[label=""Test1"",shape=ellipse]; " +
                @"""Test1"" -> ""Test1""[arrowhead=normal]; " +
                @"""GetMethodFromHandle""[label=""GetMethodFromHandle"",shape=ellipse]; " +
                @"""Test1"" -> ""GetMethodFromHandle""[arrowhead=normal]; " +
                @"""Empty""[label=""Empty"",shape=ellipse]; " +
                @"""Test1"" -> ""Empty""[arrowhead=normal]; " +
                @"""Call""[label=""Call"",shape=ellipse]; " +
                @"""Test1"" -> ""Call""[arrowhead=normal]; " +
                @"""Lambda""[label=""Lambda"",shape=ellipse]; " +
                @"""Test1"" -> ""Lambda""[arrowhead=normal]; " +
                @"""Expand""[label=""Expand"",shape=ellipse]; " +
                @"""Test1"" -> ""Expand""[arrowhead=normal]; " +
                @"""Should""[label=""Should"",shape=ellipse]; " +
                @"""Test1"" -> ""Should""[arrowhead=normal]; " +
                @"""Empty""[label=""Empty"",shape=ellipse]; " +
                @"""Test1"" -> ""Empty""[arrowhead=normal]; " +
                @"""StartWith""[label=""StartWith"",shape=ellipse]; " +
                @"""Test1"" -> ""StartWith""[arrowhead=normal]; ");

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
                    Arguments = $@"-T png -o {fileName+ ".png"} ""{path}""",
                };
            Process.Start(pci).WaitForExit();
        }
    }
}
