---
id: 102
title: "Refactor BaseOptimization & RegistryHelper"
status: "Todo"
priority: "High"
order: 20
created: 2026-03-05
updated: 2026-03-05
links:
  - url: ../linear_ticket_parent.md
    title: Parent Ticket
---

# Description

## Problem to solve
Every optimization file is stuffed with redundant code for logging and status checks.

## Solution
Refactor `Tweaker/Optimizations/Base/BaseOptimization.cs` to provide helper methods that use the new `RegistryTransaction` system. Update `RegistryHelper.cs` to support transactional calls.

## Implementation Details
- Add `ApplyRegistryTweaks()` that takes a list of tweaks and uses a transaction.
- Standardize the `Execute()` pattern across the app.
- Ensure all logging is handled centrally.
