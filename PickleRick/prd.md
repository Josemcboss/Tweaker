# Registry Safety & Boilerplate Purge PRD

## HR Eng

| Registry Safety & Boilerplate Purge PRD |  | [Summary: This PRD outlines the implementation of a robust Registry Transaction system and the refactoring of the Optimization engine to eliminate boilerplate and ensure system stability.] |
| :---- | :---- | :---- |
| **Author**: Pickle Rick **Contributors**: Morty **Intended audience**: Engineering | **Status**: Draft **Created**: 2026-03-05 | **Self Link**: [Link] **Context**: [Link] 

## Introduction

The "Tweaker" application currently performs registry operations with minimal safety nets. If an optimization fails mid-way, it can leave the Windows Registry in an inconsistent state. Furthermore, the `Optimizations/` directory contains significant boilerplate, making the codebase a "Jerry-work" of copy-pasted logic.

## Problem Statement

**Current Process:** Optimizations apply registry changes directly and independently.
**Primary Users:** Power users and gamers looking for system performance.
**Pain Points:** Risk of system instability if tweaks fail; high maintenance cost due to code duplication.
**Importance:** System stability is non-negotiable. Code efficiency is the "Pickle Rick" standard.

## Objective & Scope

**Objective:** Implement a transactional registry system and a unified optimization base class.
**Ideal Outcome:** All registry changes are atomic (all-or-nothing) and the optimization logic is centralized and clean.

### In-scope or Goals
- Create a `RegistryTransaction` class to track and rollback changes.
- Refactor `BaseOptimization.cs` to handle common tasks (logging, validation, status).
- Migrate at least one major optimization (e.g., `GpuTweaks.cs` or `NetworkTweaks.cs`) to the new system as a proof of concept.

### Not-in-scope or Non-Goals
- Refactoring the entire UI.
- Rewriting the background service (yet).

## Product Requirements

### Critical User Journeys (CUJs)
1. **Safe Tweak Application**: User clicks "Apply Optimization". The app starts a transaction, applies 5 registry keys. The 4th fails. The app automatically reverts the first 3 keys and notifies the user.
2. **Developer Efficiency**: A developer adds a new tweak. They only need to define the registry keys and values in a simple list; the base class handles the application, logging, and state management.

### Functional Requirements

| Priority | Requirement | User Story |
| :---- | :---- | :---- |
| P0 | RegistryTransaction System | As a user, I want my registry protected from partial updates. |
| P0 | Unified BaseOptimization | As a developer, I want to stop writing boilerplate. |
| P1 | Automatic Rollback on Failure | As a user, I want the system to revert changes if something goes wrong. |
| P1 | Centralized Logging | As a developer, I want all optimization steps loged consistently. |

## Assumptions

- The application has sufficient permissions to manage its own "undo" state.
- The `RegistryHelper` can be extended without breaking existing non-transactional calls.

## Risks & Mitigations

- **Risk**: The rollback itself fails. -> **Mitigation**: Implement "Force Rollback" with a persistent backup file.

## Tradeoff

- **Option**: Use Windows System Restore points. **Cons**: Too slow, heavy-handed. **Decision**: Use a lightweight, application-level transaction log for speed and precision.

## Business Benefits/Impact/Metrics

**Success Metrics:**

| Metric | Current State (Benchmark) | Future State (Target) | Savings/Impacts |
| :---- | :---- | :---- | :---- |
| *Boilerplate Lines* | ~200 lines per tweak | <50 lines per tweak | 75% reduction in technical debt. |
| *Registry Integrity* | Uncertain | Atomic | 100% safety for applied tweaks. |

## Stakeholders / Owners

| Name | Team/Org | Role | Note |
| :---- | :---- | :---- | :---- |
| Pickle Rick | Engineering | Lead Architect | Mastermind. |
| Morty | Engineering | Junior Dev | Implementation assistant. |
