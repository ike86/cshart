using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DotNetGraph;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.Extensions;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;
using Mono.Reflection;

namespace Cshart
{
    public class Chart
    {
        public static string Expand<T>(Expression<Action<T>> selector)
        {
            var result = ExtractCalls(selector);

            return ConvertToDotGraph(result);
        }

        private static IEnumerable<MethodCall> ExtractCalls<T>(Expression<Action<T>> selector)
        {
            var expression = (MethodCallExpression)selector.Body;
            string name = expression.Method.Name;

            var methodBase = typeof(T).GetMethod(name);
            foreach (var instruction in methodBase.GetInstructions())
            {
                var methodInfo = instruction.Operand as MethodInfo;

                if (methodInfo != null)
                {
                    yield return new MethodCall(methodBase, methodInfo);
                }
            }
        }

        private static string ConvertToDotGraph(IEnumerable<MethodCall> methodCalls)
        {
            methodCalls = methodCalls.ToArray();
            var caller = methodCalls.First().Caller;

            var graph =
                new Graph(
                    $"{caller.DeclaringType.Name}.{caller.Name}()",
                    directed: true);

            var callerTypeCluster =
                new SubGraph($"cluster {caller.DeclaringType.Name}")
                {
                    Style = DotSubGraphStyle.Rounded,
                    Label = caller.DeclaringType.Name,
                };
            graph.Elements.Add(callerTypeCluster);
            callerTypeCluster.AddMethod(caller);

            foreach (var call in methodCalls)
            {
                var calleeTypeCluster =
                    new SubGraph($"cluster {call.Callee.DeclaringType.Name}")
                    {
                        Style = DotSubGraphStyle.Rounded,
                        Label = call.Callee.DeclaringType.Name,
                    };
                calleeTypeCluster.AddMethod(call.Callee);
                graph.Elements.Add(calleeTypeCluster);

                graph.AddMethodCall(caller, call.Callee);
            }

            return graph.Compile();
        }


        private class MethodCall
        {
            public MethodCall(MethodInfo caller, MethodInfo callee)
            {
                Caller = caller;
                Callee = callee;
            }

            public MethodInfo Caller { get; }
            public MethodInfo Callee { get; }
        }
    }

    public static class GraphExtensions
    {
        public static void AddMethod(this IGraph graph, MethodInfo caller)
        {
            graph.Elements.Add(
                new DotNode
                {
                    Identifier = caller.Name,
                    Label = caller.Name,
                    Shape = DotNodeShape.Ellipse,
                });
        }

        public static void AddMethodCall(this IGraph graph, MethodInfo caller,  MethodInfo callee)
        {
            graph.Elements.Add(
                new DotEdge(
                    left: caller.Name,
                    right: callee.Name)
                {
                    ArrowHead = DotEdgeArrowType.Normal
                });
        }
    }

    public class Graph : DotGraph, IGraph
    {
        public Graph()
        {
        }

        public Graph(string identifier, bool directed = false) : base(identifier, directed)
        {
        }
    }

    public class SubGraph : DotSubGraph, IGraph
    {
        public SubGraph(string identifier = null) : base(identifier)
        {
        }
    }

    public interface IGraph
    {
        List<IDotElement> Elements { get; }
    }
}