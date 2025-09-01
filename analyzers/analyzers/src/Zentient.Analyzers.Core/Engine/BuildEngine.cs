// <copyright file="src/Zentient.Analyzers/Engine/BuildEngine.cs" author="Ulf Bourelius">
// Copyright (c) 2025 Ulf Bourelius. All rights reserved. MIT License. See LICENSE in the project root for license information.
// </copyright>

using System.Collections.Immutable;

using Zentient.Analyzers.Abstractions;
using Zentient.Analyzers.Extensions;
using Zentient.Analyzers.Registry;

namespace Zentient.Analyzers.Engine
{
    using System.Collections.Immutable;

    using Zentient.Analyzers.Abstractions;
    using Zentient.Analyzers.Extensions;
    using Zentient.Analyzers.Registry;

    /// <summary>
    /// Generates <see cref="ISourceUnit"/> instances from registered <see cref="ICodeInstructions"/>
    /// using a robust topological sort (Kahn's algorithm) and safe event dispatch.
    /// </summary>
    public sealed class BuildEngine : IBuildEngine
    {
        private readonly IRegistry _registry;
        private readonly Dictionary<string, ISourceUnit> _cache = new(StringComparer.Ordinal);

        public event Action<ICodeInstructions>? BeforeEmit;
        public event Action<ISourceUnit>? AfterEmit;

        public BuildEngine(IRegistry registry) => _registry = registry;

        /// <inheritdoc/>
        public IReadOnlyList<ISourceUnit> Build(IEnumerable<string> instructionKeys, bool includeDependencies = true)
        {
            ArgumentNullException.ThrowIfNull(instructionKeys);

            var seeds = instructionKeys.ToHashSet(StringComparer.Ordinal);
            var allKeys = includeDependencies ? ComputeTransitiveClosure(seeds) : seeds;
            var (graph, indegree) = BuildGraph(allKeys);
            var order = new List<string>(allKeys.Count);
            var q = new Queue<string>(indegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));

            while (q.Count > 0)
            {
                var u = q.Dequeue();
                order.Add(u);

                if (!graph.TryGetValue(u, out var outs))
                    continue;

                foreach (var v in outs)
                {
                    indegree[v]--;

                    if (indegree[v] == 0)
                    {
                        q.Enqueue(v);
                    }
                }
            }

            if (order.Count != allKeys.Count)
            {
                var cyclic = string.Join(", ", indegree.Where(kv => kv.Value > 0).Select(kv => kv.Key));
                throw new InvalidOperationException($"Cyclic dependency detected among instructions: {cyclic}");
            }

            var emitted = new List<ISourceUnit>(order.Count);

            foreach (var key in order)
            {
                if (_cache.TryGetValue(key, out var unit))
                {
                    emitted.Add(unit);
                    continue;
                }

                var instr = _registry.GetInstructions(key);
                BeforeEmit?.Invoke(instr);

                unit = instr switch
                {
                    IStubInstructions stub => stub.Emit(),
                    ITemplateInstructions tpl => tpl.Emit(GetRequiredStubs(tpl)),
                    _ => throw new InvalidOperationException($"Unsupported instruction type: {instr.GetType().Name}")
                };

                _cache[key] = unit;
                emitted.Add(unit);
                AfterEmit?.Invoke(unit);
            }

            return emitted.ToImmutableArray();
        }

        private ImmutableList<ISourceUnit> GetRequiredStubs(ITemplateInstructions template)
        {
            var list = new List<ISourceUnit>(template.Requires.Count);

            foreach (var req in template.Requires)
            {
                var instr = _registry.GetInstructions(req);

                if (instr is not IStubInstructions)
                {
                    throw new InvalidOperationException($"Template '{template.Key}' requires '{req}', which is not a stub.");
                }

                if (!_cache.TryGetValue(req, out var stubUnit))
                {
                    throw new InvalidOperationException($"Required stub '{req}' was not emitted prior to template '{template.Key}'.");
                }

                list.Add(stubUnit);
            }

            return list.ToImmutableList();
        }

        private HashSet<string> ComputeTransitiveClosure(HashSet<string> seeds)
        {
            var all = new HashSet<string>(seeds, StringComparer.Ordinal);
            var stack = new Stack<string>(seeds);

            while (stack.Count > 0)
            {
                var key = stack.Pop();
                var instr = _registry.GetInstructions(key);

                foreach (var dep in instr.Requires)
                {
                    if (all.Add(dep))
                    {
                        stack.Push(dep);
                    }
                }
            }

            return all;
        }

        private (Dictionary<string, List<string>> graph, Dictionary<string, int> indegree) BuildGraph(HashSet<string> keys)
        {
            var graph = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            var indegree = keys.ToDictionary(k => k, _ => 0, StringComparer.Ordinal);

            foreach (var key in keys)
            {
                var instr = _registry.GetInstructions(key);

                foreach (var dep in instr.Requires)
                {
                    if (!keys.Contains(dep))
                        continue;

                    if (!graph.TryGetValue(dep, out var outs))
                    {
                        graph[dep] = outs = new List<string>();
                    }

                    outs.Add(key);
                    indegree[key] = indegree[key] + 1;
                }
            }

            return (graph, indegree);
        }
    }
}
