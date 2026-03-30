# WinFormsApp1 Windows Forms .NET 10 Upgrade Tasks

## Overview

This document tracks the all-at-once migration of the AHK GUI tool to a .NET 10 Windows Forms application. The upgrade will reconstruct the full GUI, integrate all core behaviors, and validate parity with the original AHK implementation in a single coordinated effort.

**Progress**: 1/4 tasks complete (25%) ![0%](https://progress-bar.xyz/25)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-30 07:50)*
**References**: Plan §Phase 0, Plan §5

- [✓] (1) Verify .NET 10.0 SDK and Windows Forms workload are installed per Plan §5
- [✓] (2) .NET 10.0 SDK and Windows Forms workload present (**Verify**)

---

### [✗] TASK-002: Atomic migration of AHK GUI tool to .NET 10 Windows Forms
**References**: Plan §Phase 1, Plan §4, Plan §6

- [✓] (1) Reconstruct main window and capture window per Plan §4 (Host Window, Capture Window)
- [✓] (2) Implement unified editing state model and synchronize all state transitions per Plan §4 (Unified Editing State Model)
- [✓] (3) Integrate capture and selection services, including GetRange, GetRange2, RangeTip, MouseTip, and screenshot display per Plan §4 (Capture and Selection Services)
- [✓] (4) Implement image transformation and binary conversion logic (Gray2Two, GrayDiff2Two, Color2Two, ColorPos2Two, Auto) per Plan §4 (Image Transformation and Binary Conversion)
- [✓] (5) Add template output and code generation features (ASCII export, OK, SplitAdd, AllAdd, Update) per Plan §4 (Template Output and Code Generation)
- [✓] (6) Integrate advanced desktop behaviors (hotkey, binding, background capture, directory management, image saving, offset calculation) per Plan §4 (Advanced Desktop Behaviors)
- [✓] (7) Update all project files and dependencies to target net10.0-windows per Plan §4, §5
- [✓] (8) Apply all required breaking change adaptations per Plan §6
- [✓] (9) Build the solution and fix all compilation errors
- [✓] (10) Solution builds with 0 errors (**Verify**)
- [✗] (11) Commit changes with message: "TASK-002: Atomic migration of AHK GUI tool to .NET 10 Windows Forms"

---

### [ ] TASK-003: Complete advanced behavior implementation and integration
**References**: Plan §Phase 2, Plan §4, Plan §6

- [ ] (1) Finalize advanced behaviors: hotkey handling, window binding, background capture, directory management, image saving, and offset calculations per Plan §4 (Advanced Desktop Behaviors)
- [ ] (2) Ensure all breaking changes and high-attention behavior parity items are addressed per Plan §6
- [ ] (3) Build and verify all advanced features are integrated and functional
- [ ] (4) All advanced behaviors function as specified (**Verify**)
- [ ] (5) Commit changes with message: "TASK-003: Complete advanced behavior implementation and integration"

---

### [ ] TASK-004: Run full validation and test suite
**References**: Plan §Phase 3, Plan §8

- [ ] (1) Run all validation steps per Plan §8 (foundation, interaction, binary conversion, export, advanced behavior)
- [ ] (2) Fix any test or validation failures
- [ ] (3) Re-run validation after fixes
- [ ] (4) All validation steps pass with 0 failures (**Verify**)
- [ ] (5) Commit test and validation fixes with message: "TASK-004: Complete validation and testing"

---




