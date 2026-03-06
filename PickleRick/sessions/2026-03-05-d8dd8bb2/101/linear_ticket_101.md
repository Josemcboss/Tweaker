---
id: 101
title: "Implement RegistryTransaction System"
status: "Plan in Review"
priority: "High"
order: 10
created: 2026-03-05
updated: 2026-03-05
links:
  - url: ../linear_ticket_parent.md
    title: Parent Ticket
  - url: ./research_2026-03-05.md
    title: Research Document
  - url: ./plan_2026-03-05.md
    title: Implementation Plan
---

# Description

## Problem to solve
Direct registry manipulation is dangerous and not atomic.

## Solution
Create a `RegistryTransaction` class in `Tweaker/Utilities/RegistryHelper.cs` (or a new file) that tracks every change made during a session and can roll them all back if needed.

## Implementation Details
- Track (KeyPath, ValueName, OriginalValue, OriginalKind).
- Support `Commit()` and `Rollback()`.
- Use `IDisposable` pattern for easy block-level transactions.

# Comments
- **2026-03-05 (Pickle Rick)**: Research completed. Found existing `RegistryHelper` with primitive "transactions" and `RegistryBackupService` for persistent backups. Neither provides the requested `IDisposable` scoped transaction functionality. I will implement `RegistryTransaction` in a new file `Tweaker/Utilities/RegistryTransaction.cs`.
