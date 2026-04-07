namespace CanDoItAll.CodeAnalytics.Analysis.Graphs;

public sealed class StronglyConnectedComponentFinder {
    public IReadOnlyList<IReadOnlyList<string>> FindCycles(
        IReadOnlyDictionary<string, IReadOnlyList<string>> adjacency) {
        var index = 0;
        var stack = new Stack<string>();
        var indexes = new Dictionary<string, int>(StringComparer.Ordinal);
        var lowLinks = new Dictionary<string, int>(StringComparer.Ordinal);
        var onStack = new HashSet<string>(StringComparer.Ordinal);
        var components = new List<IReadOnlyList<string>>();

        foreach (var node in adjacency.Keys.OrderBy(value => value, StringComparer.Ordinal)) {
            if (!indexes.ContainsKey(node)) {
                Visit(node);
            }
        }

        return components;

        void Visit(string node) {
            indexes[node] = index;
            lowLinks[node] = index;
            index++;

            stack.Push(node);
            onStack.Add(node);

            foreach (var neighbor in adjacency.GetValueOrDefault(node, []).OrderBy(value => value, StringComparer.Ordinal)) {
                if (!indexes.ContainsKey(neighbor)) {
                    Visit(neighbor);
                    lowLinks[node] = Math.Min(lowLinks[node], lowLinks[neighbor]);
                    continue;
                }

                if (onStack.Contains(neighbor)) {
                    lowLinks[node] = Math.Min(lowLinks[node], indexes[neighbor]);
                }
            }

            if (lowLinks[node] != indexes[node]) {
                return;
            }

            var component = new List<string>();
            string current;
            do {
                current = stack.Pop();
                onStack.Remove(current);
                component.Add(current);
            }
            while (!string.Equals(current, node, StringComparison.Ordinal));

            if (component.Count > 1) {
                components.Add(component.OrderBy(value => value, StringComparer.Ordinal).ToArray());
            }
        }
    }
}
