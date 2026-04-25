using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using Tweaker.Services;

namespace Tweaker.Utilities;

public class ProfileManager : IProfileManager
{
    private readonly ITweakDispatcher _dispatcher;
    private readonly string _profilesDirectory;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ProfileManager(ITweakDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _profilesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GhostOptimizer", "Profiles");
        Directory.CreateDirectory(_profilesDirectory);
        EnsureBuiltInProfiles();
    }

    public List<TweakerProfile> GetAllProfiles()
    {
        EnsureBuiltInProfiles();

        var profiles = new List<TweakerProfile>();
        foreach (string file in Directory.EnumerateFiles(_profilesDirectory, "*.json"))
        {
            try
            {
                TweakerProfile? profile = JsonSerializer.Deserialize<TweakerProfile>(File.ReadAllText(file), JsonOptions);
                if (profile != null)
                {
                    profiles.Add(profile);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ProfileManager] Error reading {file}: {ex.Message}");
            }
        }

        return profiles.OrderBy(p => p.Name).ToList();
    }

    public TweakerProfile? LoadProfile(string profileName)
    {
        try
        {
            string filePath = GetProfilePath(profileName);
            if (!File.Exists(filePath))
            {
                return null;
            }

            return JsonSerializer.Deserialize<TweakerProfile>(File.ReadAllText(filePath), JsonOptions);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ProfileManager] Error loading profile {profileName}: {ex.Message}");
            return null;
        }
    }

    public void SaveProfile(TweakerProfile profile)
    {
        try
        {
            Directory.CreateDirectory(_profilesDirectory);
            File.WriteAllText(GetProfilePath(profile.Name), JsonSerializer.Serialize(profile, JsonOptions));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ProfileManager] Error saving profile {profile.Name}: {ex.Message}");
        }
    }

    public void ApplyProfile(TweakerProfile profile)
    {
        try
        {
            foreach (string tweakId in profile.Tweaks)
            {
                _dispatcher.ApplyTweak(tweakId);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ProfileManager] Error applying profile {profile.Name}: {ex.Message}");
        }
    }

    public bool DeleteProfile(string profileName)
    {
        try
        {
            string filePath = GetProfilePath(profileName);
            if (!File.Exists(filePath))
            {
                return false;
            }

            File.Delete(filePath);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ProfileManager] Error deleting profile {profileName}: {ex.Message}");
            return false;
        }
    }

    public IReadOnlyList<TweakerProfile> GetBuiltInProfiles() => new[]
    {
        new TweakerProfile
        {
            Name = "Casual",
            Description = "Perfil equilibrado para uso diario.",
            Created = DateTime.UtcNow,
            Tweaks = new List<string>
            {
                "mouse_acceleration",
                "keyboard_optimization",
                "visual_effects",
                "network_balanced",
                "gamedvr_disable"
            }
        },
        new TweakerProfile
        {
            Name = "Competitivo",
            Description = "Perfil para gaming competitivo.",
            Created = DateTime.UtcNow,
            Tweaks = new List<string>
            {
                "mouse_acceleration",
                "keyboard_optimization",
                "visual_effects",
                "network_optimization",
                "disable_lso",
                "gamedvr_disable",
                "high_performance_plan",
                "max_timer_resolution",
                "irq_network_priority",
                "nagle_algorithm_off",
                "cpu_anti_throttling"
            }
        },
        new TweakerProfile
        {
            Name = "Extremo",
            Description = "Perfil extremo para máximo rendimiento.",
            Created = DateTime.UtcNow,
            Tweaks = new List<string>
            {
                "mouse_acceleration",
                "keyboard_optimization",
                "visual_effects",
                "memory_optimization",
                "network_optimization",
                "disable_lso",
                "gamedvr_disable",
                "gpu_scheduling",
                "core_isolation",
                "hpet_optimization",
                "mpo_fix",
                "ultimate_power",
                "hyperv_disable",
                "spectre_meltdown_disable"
            }
        }
    };

    private void EnsureBuiltInProfiles()
    {
        foreach (TweakerProfile profile in GetBuiltInProfiles())
        {
            if (!File.Exists(GetProfilePath(profile.Name)))
            {
                SaveProfile(profile);
            }
        }
    }

    private string GetProfilePath(string profileName)
    {
        string invalid = new string(Path.GetInvalidFileNameChars());
        string fileName = new string(profileName.Where(c => !invalid.Contains(c)).ToArray()).Trim();
        return Path.Combine(_profilesDirectory, $"{fileName}.json");
    }
}

public class TweakerProfile
{
    public string Name { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public List<string> Tweaks { get; set; } = new();
    public string Description { get; set; } = string.Empty;

    public int TweakCount => Tweaks.Count;

    public List<string> TweakIds
    {
        get => Tweaks;
        set => Tweaks = value ?? new List<string>();
    }

    [JsonIgnore]
    public string Category { get; set; } = string.Empty;

    [JsonIgnore]
    public string Author { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTime CreatedDate
    {
        get => Created;
        set => Created = value;
    }
}
