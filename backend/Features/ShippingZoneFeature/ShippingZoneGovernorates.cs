using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace backend.Features.ShippingZoneFeature;

internal static class ShippingZoneGovernorates
{
    public static List<string> Parse(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return new List<string>();

        var trimmed = raw.Trim();
        if (trimmed.StartsWith('['))
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(trimmed) ?? new List<string>();
            }
            catch (JsonException)
            {
                // Fall through to comma-separated handling for legacy seed data.
            }
        }

        return trimmed
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(g => g.Length > 0)
            .ToList();
    }

    public static string Serialize(IEnumerable<string>? governorates)
    {
        var list = governorates?
            .Select(g => g.Trim())
            .Where(g => g.Length > 0)
            .ToList() ?? new List<string>();

        return JsonSerializer.Serialize(list);
    }
}
