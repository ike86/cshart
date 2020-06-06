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

        private static string ConvertToDotGraph(IEnumerable<MethodCall> methodCalls)
        {
            methodCalls = methodCalls.ToArray();
            var graph =
                new DotGraph(
                    $"{methodCalls.First().Caller.DeclaringType.Name}.{methodCalls.First().Caller.Name}()",
                    directed: true);

            graph.Elements.Add(
                new DotNode
                {
                    Identifier = methodCalls.First().Caller.Name,
                    Label = methodCalls.First().Caller.Name,
                    Shape = DotNodeShape.Ellipse,
                });

            foreach (var call in methodCalls)
            {
                graph.Elements.Add(
                    new DotNode
                    {
                        Identifier = call.Callee.Name,
                        Label = call.Callee.Name,
                        Shape = DotNodeShape.Ellipse,
                    });

                graph.Elements.Add(
                    new DotEdge(
                        left: methodCalls.First().Caller.Name,
                        right: call.Callee.Name)
                    {
                        ArrowHead = DotEdgeArrowType.Normal
                    });
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
}