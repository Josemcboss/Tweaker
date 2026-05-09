---
id: i9j0k1l2
title: Restore Network & Ping Page
status: Todo
priority: High
order: 30
created: 2026-04-25
updated: 2026-04-25
links:
  - url: ../linear_ticket_parent.md
    title: Parent Ticket
---

# Description
Restore the actual content of the NetworkPage ScrollViewer from MainWindow.xaml.backup.

## Implementation Details
- Extract NetworkPage section from backup.
- Insert into MainWindow.xaml.
- Fix event handlers and OneWay bindings.
