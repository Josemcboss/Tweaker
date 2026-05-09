---
id: parent
title: "[Epic] UI Restoration & Tabs Organization"
status: Todo
priority: High
order: 0
created: 2026-04-25
updated: 2026-04-25
links:
  - url: ./prd.md
    title: PRD
---

# Description

## Problem to solve
The Ghost Optimizer UI is missing content across multiple tabs, and navigation is inconsistent.

## Solution
Restore all missing UI controls from backups, fix event handlers, and standardize the layout.

## Implementation Details
- Audit all ScrollViewers in MainWindow.xaml.
- Restore content from MainWindow.xaml.backup.
- Map correct event handlers in MainWindow.xaml.cs.
- Fix all TwoWay bindings to OneWay for read-only properties.
