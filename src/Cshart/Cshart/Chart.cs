﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetGraph;
using EnsureThat;
using Mono.Reflection;

namespace Cshart
{
    /// <summary>
    /// Represents a chart.
    /// </summary>
    public class Chart
    {
        public IEnumerable<IGraphElement> Add(Type type)
        {
            EnsureArg.IsNotNull(type, nameof(type));

            var node = Node.FromType(type);
            var memberInfos = GetMemberInfos(type);
            AddMembers(node, memberInfos, type);

            foreach (var member in memberInfos)
            {
                var methodInfo = type.GetMethod(member.Name);
                AddCallsFrom(methodInfo, node, type);
            }

            yield return node;
        }

        private static void AddMembers(
            Node node,
            IEnumerable<MemberInfo> memberInfos,
            Type type)
        {
            foreach (var member in memberInfos)
            {
                node.Add(Node.FromMember(type, member));
            }
        }

        private static void AddCallsFrom(
            MethodInfo callerMethodInfo,
            Node node,
            Type type)
        {
            if (callerMethodInfo is null)
            {
                return;
            }

            foreach (var instruction in callerMethodInfo.GetInstructions())
            {
                if (instruction.Operand is MethodInfo methodInfo)
                {
                    node.Add(
                        new Edge(
                            node,
                            Node.CreateId(type, callerMethodInfo),
                            Node.CreateId(type, methodInfo)));
                }
            }
        }

        internal DotGraph ConvertToDotGraph(IEnumerable<IGraphElement> nodes)
        {
            var root = new Graph("root");
            var typeNode = nodes.First() as Node;
            var typeCluster = new SubGraph(typeNode.Id);
            typeCluster.AddMethod(typeNode.Children.First() as Node);
            root.Elements.Add(typeCluster);
            return root;
        }

        private static IEnumerable<MemberInfo> GetMemberInfos(Type type)
        {
            return
                type.GetMembers(
                    BindingFlags.DeclaredOnly
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance);
        }
    }
}
