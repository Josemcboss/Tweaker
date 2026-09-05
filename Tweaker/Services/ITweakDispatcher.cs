using System;
using System.Collections.Generic;

namespace Tweaker.Services
{
    public interface ITweakDispatcher
    {
        bool ApplyTweak(string tweakId);
        bool RevertTweak(string tweakId);
        bool IsTweakSupported(string tweakId);
        bool? GetRealState(string tweakId);

        /// <summary>
        /// Returns the canonical (non-alias) tweak IDs managed by this dispatcher.
        /// Used to derive TotalTweaksCount dynamically.
        /// </summary>
        IReadOnlyCollection<string> GetCanonicalTweakIds();

        /// <summary>
        /// Resolves any alias or canonical ID to its canonical ID.
        /// </summary>
        string? GetCanonicalId(string tweakId);

        /// <summary>
        /// Returns all known aliases for a given tweak ID.
        /// </summary>
        IReadOnlyList<string> GetAliases(string tweakId);
    }
}
