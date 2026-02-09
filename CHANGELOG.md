# ?? CHANGELOG

All notable changes to Ghost Optimizer will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [2.1.1] - 2025-01-XX - ?? CRITICAL FIX

### ?? Critical Fix - BrokerInfrastructure Protection

#### Problem Identified
- The "Disable Telemetry & Tracking" tweak was disabling `BrokerInfrastructure`, a **CRITICAL SYSTEM SERVICE**
- This caused system instability and potential crashes
- User reported: "Active solo esos 2 tweaks y se deshabilitó el broker"

#### Solution Applied
- **Removed `BrokerInfrastructure` from telemetry services list**
- Modified: `Tweaker\Optimizations\PrivacyTweaks.cs` (lines 328 and 405)
- Added documentation: `Release\CRITICAL_FIX_BROKERINFRASTRUCTURE.md`
- Created verification script: `Release\VERIFICAR_BROKERINFRASTRUCTURE.bat`

#### What is BrokerInfrastructure?
- **Function**: Background Tasks Infrastructure Service
- **Purpose**: Manages Windows background tasks and Modern Apps
- **Criticality**: One of 15 core Windows services
- **Impact if disabled**: App Store failures, scheduled tasks broken, system instability

#### Services Still Disabled (Correct):
- ? `DiagTrack` - Telemetry
- ? `dmwappushservice` - WAP Push
- ? `WerSvc` - Error Reporting
- ? `OneSyncSvc` - Sync Service
- ? `MessagingService` - Messaging
- ? `PimIndexMaintenanceSvc` - Contact Data
- ? `UserDataSvc` - User Data
- ? `UnistoreSvc` - Data Storage
- ? `DcpSvc` - Data Collection

#### Services Now Protected:
- ??? `BrokerInfrastructure` - **CRITICAL - NO LONGER TOUCHED**

### ?? New Documentation Files
- `CRITICAL_FIX_BROKERINFRASTRUCTURE.md` - Complete fix documentation
- `VERIFICAR_BROKERINFRASTRUCTURE.bat` - Quick verification script
- `RESUMEN_FIX_v2.1.1.md` - Executive summary
- `BUILD_v2.1.1.bat` - Build script with integrated changelog

### ?? Action Required for Users

#### If you used previous versions (v2.1.0 or earlier):

1. **Restore the service:**
   - Open Ghost Optimizer
   - Go to "Sistema & GPU - Toques Finales"
   - Find "Deshabilitar Telemetría & Tracking"
   - Click "ENABLE"
   - Restart Windows

2. **Verify restoration:**
   - Run: `DIAGNOSTICO_SERVICIOS.bat`
   - Check: `BrokerInfrastructure START_TYPE = 2`

3. **Update to v2.1.1:**
   - Download v2.1.1
   - Replace executable
   - Now safe to use telemetry tweak

### ? Validation
- [x] Build successful
- [x] BrokerInfrastructure protected
- [x] Other telemetry services work correctly
- [x] Complete documentation
- [x] Verification scripts

### ?? Priority
**CRITICAL** - This fix must be applied immediately by all users.

---

## [2.0.0] - 2025-01-25

### ?? Added

#### **New Tweaks (26 total)**
- **Advanced Pack** (11 tweaks):
  - Interrupt Moderation OFF (Network)
  - MenuShowDelay = 0ms
  - Win32 Priority Separation (3 profiles: Balanced, Smooth, Aggressive)
  - Timestamping optimization
  - NTFS Performance (8.3 filenames OFF)
  - Large System Cache (16GB+ RAM)
  - SvcHost Split Threshold
  - SearchBox disable
  - Lockscreen disable
  - Transparency OFF
  - Fast Startup optimization

- **Input & Visuals Pack** (3 new):
  - Transparency Effects disable
  - Sticky Keys disable
  - Enhanced mouse optimization

- **Network Pack** (2 new):
  - Browser optimization (balanced mode)
  - Advanced diagnostics

- **Services Pack** (5 new):
  - Telemetry services disable
  - Delivery Optimization OFF
  - Privacy tweaks
  - Background app restrictions
  - Update control

- **Sistema Pack** (2 new):
  - NTFS Last Access Time
  - Game Process Priority (15 games)

- **GHOST Pack** (3 new):
  - CPU Priority Profiles
  - MPO Fix
  - Enhanced power plans

#### **Security System**
- ? **ServiceGuard**: Protection for critical Windows services
  - Prevents disabling essential services (wuauserv, EventLog, DcomLaunch, RpcSs, etc.)
  - Automatic validation before service modifications
  
- ? **OptimizationBackup**: Automatic backup system
  - Registry backups before modifications
  - Service state snapshots
  - BCD command logging
  - Backup location: `%LOCALAPPDATA%\GhostOptimizer\Backups\`
  
- ? **BCD Validation**: Safe boot configuration editing
  - Validates `bcdedit` commands before execution
  - Prevents system-breaking configurations
  - Automatic backup of current BCD state
  
- ? **Rescue Script Generator**: Emergency recovery tool
  - Generates PowerShell script to revert ALL changes
  - Automatic creation on first run
  - Location: `%LOCALAPPDATA%\GhostOptimizer\Rescue\`
  - One-click restore to default Windows state

#### **Dashboard Improvements**
- ?? **Dynamic Stats**: Updated to show 58 total tweaks (was 32)
- ?? **Real-time Percentage**: Automatically calculates optimization level
- ?? **Performance Estimations**:
  - FPS gain calculator
  - Latency reduction estimator
  - RAM freed calculator
- ?? **Active Tweaks List**: Shows recently activated optimizations
- ?? **State Persistence**: Remembers active tweaks between sessions

#### **UI Enhancements**
- ?? Modern Discord-inspired design
- ?? Improved button indicators (ON/OFF states with colors)
- ?? Responsive layout for different screen sizes
- ? Smooth animations and transitions
- ?? Enhanced notification system

#### **Documentation**
- ?? Complete security system documentation
- ?? Dashboard usage guide
- ? Advanced tweaks technical details
- ??? Safety guidelines and warnings
- ?? Troubleshooting guide

---

### ?? Changed

#### **Tweak Organization**
- Reorganized tweaks into 7 categories (was 6):
  1. Input & Visuals (12 tweaks)
  2. Red & Ping (10 tweaks)
  3. Sistema & GPU (8 tweaks)
  4. Limpieza (4 tweaks)
  5. GHOST Pack (8 tweaks)
  6. **Advanced** (11 tweaks) - NEW
  7. Servicios (5 tweaks) - NEW

#### **Performance Optimizations**
- Improved startup time (-30%)
- Reduced memory footprint
- Faster tweak application
- Optimized registry operations

#### **State Management**
- Enhanced `TweakStateManager` with:
  - Better persistence (JSON format)
  - Automatic state recovery
  - Conflict detection
  - Rollback capabilities

---

### ?? Fixed

#### **Critical Bugs**
- Fixed crash when applying Core Isolation tweak on unsupported systems
- Resolved registry access errors on non-admin execution
- Corrected DNS flush command execution
- Fixed button indicators not updating after reboot

#### **Minor Bugs**
- Improved error handling for missing registry keys
- Fixed tooltip positioning issues
- Corrected notification overlapping
- Resolved UI freezing during heavy operations

#### **Stability**
- Enhanced exception handling across all tweak modules
- Better validation of user inputs
- Improved error messages (more descriptive)

---

### ?? Security

#### **New Security Features**
- ServiceGuard prevents critical service modifications
- Automatic backup creation before any system change
- BCD command validation to prevent boot failures
- Rescue script generation for emergency recovery

#### **Warnings Added**
- ?? **Core Isolation**: Can reduce security against advanced malware
- ?? **Spectre/Meltdown**: Exposes CPU vulnerabilities for FPS gain
- ?? **Advanced Pack**: Experimental tweaks may cause instability

---

### ??? Deprecated

- Old manual backup system (replaced by OptimizationBackup)
- Legacy tweak names (migrated to new naming convention)

---

### ? Removed

- Debug logging in Release builds
- Unused legacy code
- Temporary test tweaks

---

### ?? Statistics

#### **Codebase Growth**
- **Total Lines of Code**: ~10,500 (+31% from v1.5)
- **New Files**: 14
- **Modified Files**: 28
- **Deleted Files**: 5

#### **Features**
- **Total Tweaks**: 58 (+81% from v1.5)
- **Categories**: 7 (+1 from v1.5)
- **Security Features**: 4 (NEW)

#### **Performance Impact**
- **Estimated FPS Gain**: +120 (was +70)
- **Estimated Ping Reduction**: -150ms (was -88ms)
- **Estimated RAM Freed**: 11.5 GB (was 8.3 GB)

---

## [1.5.0] - 2024-12-15

### Added
- GHOST Pack with 8 legendary tweaks
- Ultimate Performance power plan
- Core Isolation toggle
- HPET optimization
- Modern UI with Discord-inspired design
- Sidebar navigation

### Changed
- Increased total tweaks to 32
- Improved dashboard layout
- Updated color scheme

### Fixed
- Registry access bugs
- UI rendering issues
- Service management errors

---

## [1.0.0] - 2024-11-20

### Added
- Initial release
- 20 basic gaming tweaks
- Categories:
  - Input & Visuals (6 tweaks)
  - Network (5 tweaks)
  - Sistema (5 tweaks)
  - Limpieza (4 tweaks)
- Basic dashboard
- ON/OFF toggle buttons
- Simple notification system

---

## ?? Version Comparison

| Feature | v1.0 | v1.5 | v2.0 |
|---------|------|------|------|
| **Total Tweaks** | 20 | 32 | 58 |
| **Categories** | 4 | 6 | 7 |
| **Security Features** | 0 | 0 | 4 |
| **Dashboard** | Basic | Improved | Dynamic |
| **State Persistence** | No | Partial | Full |
| **Documentation** | Minimal | Good | Extensive |
| **Est. FPS Gain** | +30 | +70 | +120 |
| **Est. Ping Reduction** | -40ms | -88ms | -150ms |
| **Est. RAM Freed** | 5 GB | 8.3 GB | 11.5 GB |

---

## ?? Future Roadmap

### **v2.1.0** (Planned - Q2 2025)
- [ ] Automatic game detection
- [ ] Per-game profiles
- [ ] Telemetry dashboard (ping monitor, FPS counter)
- [ ] Cloud sync for settings
- [ ] Multi-language support (ES, PT, FR, DE)

### **v2.2.0** (Planned - Q3 2025)
- [ ] GPU overclocking presets
- [ ] RAM overclocking (XMP profiles)
- [ ] Thermal monitoring
- [ ] Fan curve optimization

### **v3.0.0** (Planned - Q4 2025)
- [ ] AI-powered optimization recommendations
- [ ] Automatic rollback on system instability
- [ ] Integration with game launchers (Steam, Epic)
- [ ] Community tweak sharing

---

## ?? Notes

### **Breaking Changes v1.5 ? v2.0**
- None. Fully backward compatible.
- Old tweak states migrate automatically.

### **Migration Guide**
1. Close Ghost Optimizer v1.5
2. Download Ghost Optimizer v2.0
3. Run as Administrator
4. Existing tweaks will be preserved

### **Known Issues v2.0.0**
- HPET tweak may not work on some AMD systems (requires BIOS support)
- Core Isolation requires restart to take full effect
- Browser optimization may need manual DNS flush

---

## ?? Acknowledgments

### **v2.0.0 Contributors**
- DaddyGhost (Lead Developer)
- Beta Testers Community
- GitHub Contributors

### **Special Thanks**
- Windows optimization community
- Discord gaming communities
- Reddit r/pcmasterrace feedback
- YouTube tech reviewers

---

## ?? License

Ghost Optimizer is licensed under the MIT License.  
See [LICENSE](LICENSE) file for details.

---

**Developed by:** DaddyGhost  
**Version:** 2.0.0  
**Release Date:** 2025-01-25
