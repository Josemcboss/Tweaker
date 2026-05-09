---
id: parent
title: "[Epic] Hardware Scanner & Adaptive Tweaks"
status: "Todo"
priority: "High"
order: 0
created: 2026-04-25
updated: 2026-04-25
links:
  - url: ../prd.md
    title: PRD
---

# Description

## Problem to solve
The app currently uses a synchronous, blocking `HardwareDetector` that lacks detail and freezes the UI on startup. There is no logic to adapt tweak recommendations based on detected hardware.

## Solution
Implement a full hardware scanning infrastructure that runs asynchronously at startup, displays a splash overlay, and provides data for adaptive tweak logic.

## Implementation Details
- T-01: Core Hardware Infrastructure
- T-02: Startup Orchestration & Splash UI
- T-03: Dashboard Telemetry UI
- T-04: Adaptive Tweak Intelligence
- T-05: Hardware Integrity Tests
