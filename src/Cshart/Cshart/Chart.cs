using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DotNetGraph;
using DotNetGraph.Edge;
using DotNetGraph.Extensions;
using DotNetGraph.Node;
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

        // TODO introduce declaring types as clusters
        private static string ConvertToDotGraph(IEnumerable<MethodCall> methodCalls)
        {
            methodCalls = methodCalls.ToArray();
            var caller = methodCalls.First().Caller;

            var graph =
                new DotGraph(
                    $"{caller.DeclaringType.Name}.{caller.Name}()",
                    directed: true);
            graph.AddMethod(caller);

            foreach (var call in methodCalls)
            {
                graph.AddMethod(call.Callee);

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
        public static void AddMethod(this DotGraph graph, MethodInfo caller)
        {
            graph.Elements.Add(
                new DotNode
                {
                    Identifier = caller.Name,
                    Label = caller.Name,
                    Shape = DotNodeShape.Ellipse,
                });
        }

        public static void AddMethodCall(this DotGraph graph, MethodInfo caller,  MethodInfo callee)
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
}