using System;

namespace Tweaker.Models;

public sealed class HistoryEntry
{
    public string Action { get; set; } = string.Empty;
    public string TweakName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
