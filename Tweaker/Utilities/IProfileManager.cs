using System.Collections.Generic;

namespace Tweaker.Utilities
{
    public interface IProfileManager
    {
        List<TweakerProfile> GetAllProfiles();
        TweakerProfile? LoadProfile(string profileName);
        void SaveProfile(TweakerProfile profile);
        void ApplyProfile(TweakerProfile profile);
        bool DeleteProfile(string profileName);
        IReadOnlyList<TweakerProfile> GetBuiltInProfiles();
    }
}
