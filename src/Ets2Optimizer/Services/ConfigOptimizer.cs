using System.Text.RegularExpressions;
using Ets2Optimizer.Models;

namespace Ets2Optimizer.Services;

public static partial class ConfigOptimizer
{
    [GeneratedRegex(@"^uset\s+(\S+)\s+""([^""]*)""\s*$")]
    private static partial Regex UsetLineRegex();

    public static (List<string> NewLines, List<ConfigChange> Changes) Apply(
        IReadOnlyList<string> originalLines, PerformanceTier tier)
    {
        var rulesByKey = RuleSet.Rules.ToDictionary(r => r.Key, StringComparer.OrdinalIgnoreCase);
        var newLines = new List<string>(originalLines.Count);
        var changes = new List<ConfigChange>();

        foreach (var line in originalLines)
        {
            var match = UsetLineRegex().Match(line);
            if (!match.Success)
            {
                newLines.Add(line);
                continue;
            }

            var key = match.Groups[1].Value;
            var currentValue = match.Groups[2].Value;

            if (!rulesByKey.TryGetValue(key, out var rule))
            {
                newLines.Add(line);
                continue;
            }

            var desiredValue = rule.ValueFor(tier);
            if (string.Equals(currentValue, desiredValue, StringComparison.Ordinal))
            {
                newLines.Add(line);
                continue;
            }

            changes.Add(new ConfigChange(key, currentValue, desiredValue, rule.Reason));
            newLines.Add($"uset {key} \"{desiredValue}\"");
        }

        return (newLines, changes);
    }
}
