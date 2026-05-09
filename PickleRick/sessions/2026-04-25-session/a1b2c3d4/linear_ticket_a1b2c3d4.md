---
id: a1b2c3d4
title: Audit and Restore Base Layout
status: Todo
priority: High
order: 10
created: 2026-04-25
updated: 2026-04-25
links:
  - url: ../linear_ticket_parent.md
    title: Parent Ticket
---

# Description
The current MainWindow.xaml is truncated at 377 lines. We need to restore the base grid, sidebar, and the first few pages (Dashboard, Input, Network) from the backup.

## Implementation Details
- Read MainWindow.xaml.backup.
- Reconstruct the base structure of MainWindow.xaml.
- Ensure the Sidebar navigation matches the backup.
- Restore Dashboard, Input, and Network pages.
