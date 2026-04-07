using System.Text.Json;
using System.Text.Json.Serialization;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;

namespace CanDoItAll.CodeAnalytics.Storage.Snapshots;

public sealed class SnapshotJsonSerializer {
    private static readonly JsonSerializerOptions Options = CreateOptions();

    public ArchitectureSnapshot DeserializeSnapshot(string json) {
        return JsonSerializer.Deserialize<ArchitectureSnapshot>(json, Options)
            ?? throw new InvalidOperationException("Snapshot JSON could not be deserialized.");
    }

    public T Deserialize<T>(string json) {
        return JsonSerializer.Deserialize<T>(json, Options)
            ?? throw new InvalidOperationException($"JSON could not be deserialized to {typeof(T).Name}.");
    }

    public string Serialize<T>(T value) {
        return JsonSerializer.Serialize(value, Options);
    }

    private static JsonSerializerOptions CreateOptions() {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }
}
