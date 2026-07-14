namespace Ets2Optimizer.Models;

public sealed record ConfigChange(string Key, string OldValue, string NewValue, string Reason);
