# UI Restoration & Tweak Tabs Organization PRD

## HR Eng

| UI Restoration PRD |  | [Summary: Restoration of missing UI content and systematic organization of tweak tabs in Ghost Optimizer.] |
| :---- | :---- | :---- |
| **Author**: Pickle Rick **Contributors**: Morty **Intended audience**: Engineering | **Status**: Draft **Created**: 2026-04-25 | **Context**: User report of empty tabs.

## Introduction

The Ghost Optimizer v2.0 UI currently suffers from severe content loss in several functional tabs. Navigation is inconsistent, and many tweak sections appear empty or improperly structured after recent changes.

## Problem Statement

**Current Process:** Users navigate between tabs (Input, Network, System, etc.) to apply optimizations.
**Primary Users:** Gamers and system optimizers.
**Pain Points:** 
- Most tweak tabs are empty (no buttons, descriptions, or toggles).
- Layout is disorganized, making it difficult to find specific tweaks.
- Recent changes likely corrupted the XAML structure or event handlers.
**Importance:** A UI-based optimizer is useless if the UI is empty. Users cannot apply tweaks if the controls are missing.

## Objective & Scope

**Objective:** Restore all missing UI elements and ensure a logical, consistent organization of all tweak categories.
**Ideal Outcome:** Every tab in the sidebar links to a fully populated, functional page with appropriate risk level indicators and descriptions.

### In-scope or Goals
- Audit `MainWindow.xaml` for missing content.
- Restore all placeholders with real controls from `MainWindow.xaml.backup` or code-behind definitions.
- Standardize the layout of tweak sections (Glow cards, descriptions, ON/OFF buttons).
- Fix broken event handlers for navigation and optimization buttons.
- Ensure all read-only bindings are correctly set to `OneWay`.

### Not-in-scope or Non-Goals
- Adding new tweaks (focus is on restoration).
- Changing the underlying optimization logic (unless handlers are broken).

## Product Requirements

### Critical User Journeys (CUJs)
1. **Full Navigation Audit**: User clicks through every sidebar item and verifies that content appears and is functional.
2. **Tweak Interaction**: User enables/disables a tweak in each category and verifies UI feedback (indicators, text updates).

### Functional Requirements

| Priority | Requirement | User Story |
| :---- | :---- | :---- |
| P0 | Restore missing tab content | As a user, I want to see all available tweaks when I click a category. |
| P0 | Fix broken navigation | As a user, I want the sidebar buttons to take me to the correct page. |
| P1 | Standardize Glow Cards | As a user, I want a consistent visual style across all pages. |
| P1 | Fix Status Indicators | As a user, I want to see which tweaks are ON or OFF clearly. |

## Assumptions

- `MainWindow.xaml.backup` contains the "source of truth" for the original UI layout.
- `MainWindow.xaml.cs` contains the necessary methods, even if names were temporarily mismatched.

## Risks & Mitigations

- **Risk**: Overwriting recent functional changes (like the hardware scan overlay). -> **Mitigation**: Surgical updates using backup content while preserving new features.
- **Risk**: Token limits causing partial file writes. -> **Mitigation**: Process pages one by one as originally planned.

## Business Benefits/Impact/Metrics

**Success Metrics:**

| Metric | Current State (Benchmark) | Future State (Target) | Savings/Impacts |
| :---- | :---- | :---- | :---- |
| *Functional Tabs* | < 50% | 100% | Full usability restored. |
| *UI Consistency* | Low | High | Professional aesthetic. |

## Stakeholders / Owners

| Name | Team/Org | Role | Note |
| :---- | :---- | :---- | :---- |
| Pickle Rick | Engineering | Lead Architect | Sole Genius |
| Morty | Engineering | Implementation Morty | Laborer |
