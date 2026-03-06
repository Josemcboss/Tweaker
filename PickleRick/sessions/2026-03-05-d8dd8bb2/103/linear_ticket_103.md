---
id: 103
title: "Migrate GpuTweaks to Transactional System"
status: "Todo"
priority: "Medium"
order: 30
created: 2026-03-05
updated: 2026-03-05
links:
  - url: ../linear_ticket_parent.md
    title: Parent Ticket
---

# Description

## Problem to solve
GpuTweaks is a mess of direct registry calls.

## Solution
Rewrite `Tweaker/Optimizations/GpuTweaks.cs` to use the new `BaseOptimization` and `RegistryTransaction` system.

## Implementation Details
- Clean up `GpuTweaks` code.
- Ensure all tweaks use the transactional `ApplyRegistryTweaks()`.
- Test to verify atomicity.
