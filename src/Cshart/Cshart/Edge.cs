using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace Cshart
{
    public class Edge : IGraphElement
    {
        public Edge(IGraphElement context, string fromId, string toId)
        {
            EnsureArg.IsNotNull(fromId, nameof(fromId));
            EnsureArg.IsNotNull(toId, nameof(toId));

            var from = context.Children.FirstOrDefault(child => child.Id == fromId)
                ?? throw new ArgumentException(
                    $"There is no graph element in context {context.Id} with id {fromId}.",
                    nameof(fromId));

            var to = context.Children.FirstOrDefault(child => child.Id == toId)
                ?? throw new ArgumentException(
                    $"There is no graph element in context {context.Id} with id {toId}.",
                    nameof(toId));

            From = from;
            To = to;
        }

        public string Id => default;

        public IEnumerable<IGraphElement> Children => Enumerable.Empty<IGraphElement>();

        public IGraphElement From { get; }

        public IGraphElement To { get; }

        public void Add(IGraphElement child)
        {
            throw new NotImplementedException();
        }
    }
}
