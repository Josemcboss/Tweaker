using System;

namespace Tweaker.Services
{
    public interface ITweakDispatcher
    {
        bool ApplyTweak(string tweakId);
        bool RevertTweak(string tweakId);
        bool IsTweakSupported(string tweakId);
        bool? GetRealState(string tweakId);
    }
}
