using backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace backend.Common.Catalog;

/// <summary>
/// Category hierarchy lookups shared by the catalog slices. A parent category
/// usually holds no products of its own, so browsing it has to reach the
/// products filed under its descendants.
/// </summary>
public static class CategoryTree
{
    /// <summary>The given category id plus every descendant beneath it.</summary>
    public static async Task<HashSet<long>> ResolveAsync(
        AppDbContext context,
        long rootId,
        CancellationToken cancellationToken)
    {
        // The table is small, so one read beats a recursive query per level.
        var edges = await context.Categories
            .AsNoTracking()
            .Select(c => new { c.Id, c.ParentId })
            .ToListAsync(cancellationToken);

        var childrenByParent = edges
            .Where(c => c.ParentId is not null)
            .GroupBy(c => c.ParentId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Id).ToList());

        var resolved = new HashSet<long> { rootId };
        var pending = new Queue<long>();
        pending.Enqueue(rootId);

        while (pending.Count > 0)
        {
            if (!childrenByParent.TryGetValue(pending.Dequeue(), out var children))
                continue;

            foreach (var child in children)
            {
                // Adding guards against a cycle in badly seeded data.
                if (resolved.Add(child))
                    pending.Enqueue(child);
            }
        }

        return resolved;
    }
}
