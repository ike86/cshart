#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetGraph;
using DotNetGraph.Attributes;
using DotNetGraph.Core;
using DotNetGraph.Edge;
using DotNetGraph.Node;
using DotNetGraph.SubGraph;

namespace Cshart.Sandbox
{
    public interface IEdgeAddingStrategy
    {
        void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode);
    }

    public class AddCtorParameterTypeEdges : IEdgeAddingStrategy
    {
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            foreach (var ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var param in ctor.TryGetParameters())
                {
                    if (assemblyGraph.TryGetTypeNode(() => param.ParameterType)
                        is { } paramTypeNode)
                    {
                        assemblyGraph.Elements.Add(
                            new DotEdge(typeNode, paramTypeNode) {Label = "ctor param"});
                    }
                }
            }
        }
    }
    
    public class AddInterfaceImplementationEdges : IEdgeAddingStrategy
    {
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            foreach (var i in type.GetInterfaces())
            {
                if (assemblyGraph.TryGetTypeNode(() => i) is { } interfaceTypeNode)
                {
                    assemblyGraph.Elements.Add(
                        new DotEdge(typeNode, interfaceTypeNode) {Label = "implements"});
                }
            }
        }
    }

    public class AddInheritanceEdges : IEdgeAddingStrategy
    {
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            if (assemblyGraph.TryGetTypeNode(() => type.BaseType!) is { } baseTypeNode)
            {
                assemblyGraph.Elements.Add(
                    new DotEdge(typeNode, baseTypeNode) {Label = "inherits"});
            }
        }
    }
    
    public class AddFieldReferenceEdges : IEdgeAddingStrategy
    {
        public void AddEdges(DotSubGraph assemblyGraph, Type type, IDotElement typeNode)
        {
            var fields = type.TryGetFields().ToArray();
            foreach (var field in fields)
            {
                if (assemblyGraph.TryGetTypeNode(() => field.FieldType) is { } fieldTypeNode)
                {
                    assemblyGraph.Elements.Add(
                        new DotEdge(typeNode, fieldTypeNode) {Label = "contains"});
                }
            }
        }
    }

    public class Builder
    {
        private readonly IEnumerable<Type> types;
        private readonly string assemblyName;

        public Builder(IEnumerable<Type> types, string assemblyName)
        {
            this.types = types.ToArray();
            this.assemblyName = assemblyName;

            StyleTypeNode = (_, _) => { };
            CreateTypeNodeAppender = g => new DefaultTypeNodeAppender(g);
            FilterTypes = t => true;
        }

        public Action<Type, DotNode> StyleTypeNode { init; private get; }

        public Func<DotSubGraph, ITypeNodeAppender> CreateTypeNodeAppender { init; private get; }

        public Func<Type, bool> FilterTypes { init; private get; }

        public ICollection<IEdgeAddingStrategy> EdgeAddingStrategies { get; } =
            new List<IEdgeAddingStrategy>
            {
                new AddFieldReferenceEdges(),
                new AddInheritanceEdges(),
                new AddInterfaceImplementationEdges(),
                new AddCtorParameterTypeEdges()
            };

        public DotGraph Build()
        {
            var assemblyGraph = new DotSubGraph(assemblyName)
            {
                Label = assemblyName,
                Style = new DotSubGraphStyleAttribute(DotSubGraphStyle.Rounded),
            };

            AddTypes(assemblyGraph);

            AddEdges(assemblyGraph);

            var dotGraph = new DotGraph("foo", directed: true);
            dotGraph.Elements.Add(assemblyGraph);
            return dotGraph;
        }

        private void AddTypes(DotSubGraph assemblyGraph)
        {
            var typeNodeAppender = CreateTypeNodeAppender(assemblyGraph);
            foreach (var type in FilteredTypes())
            {
                var typeNode = new DotNode(type.FullName) {Shape = new DotNodeShapeAttribute()};
                StyleTypeNode(type, typeNode);

                typeNodeAppender.Append(type, typeNode);
            }
        }

        private IEnumerable<Type> FilteredTypes()
        {
            return types
                .Where(t => !t.IsCompilerGenerated())
                .Where(FilterTypes);
        }

        private void AddEdges(DotSubGraph assemblyGraph)
        {
            foreach (var type in types)
            {
                var typeNode = assemblyGraph.TryGetTypeNode(() => type);
                if (typeNode is null)
                {
                    continue;
                }

                foreach (var strategy in EdgeAddingStrategies)
                {
                    strategy.AddEdges(assemblyGraph, type, typeNode);
                }
            }
        }
    }
}
