# SportsStore .NET 10.0 Upgrade Tasks

## Overview

This document tracks the execution of the SportsStore solution upgrade from .NET 8.0 to .NET 10.0. Both projects will be upgraded simultaneously in a single atomic operation, followed by testing and validation.

**Progress**: 0/4 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites
**References**: Plan §Phase 0

- [▶] (1) Verify .NET 10.0 SDK installed on development machine
- [ ] (2) SDK version meets .NET 10.0 requirements (**Verify**)
- [ ] (3) Check for global.json file that might restrict SDK version
- [ ] (4) Global.json compatible with .NET 10.0 or absent (**Verify**)

---

### [ ] TASK-002: Atomic framework and dependency upgrade
**References**: Plan §Phase 1, Plan §Detailed Execution Steps, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [ ] (1) Update TargetFramework to net10.0 in both project files per Plan §Step 1 (SportsStore.csproj, SportsStore.Tests.csproj)
- [ ] (2) Both project files updated to net10.0 (**Verify**)
- [ ] (3) Update package references in SportsStore.csproj per Plan §Package Update Reference (Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.3, Microsoft.EntityFrameworkCore.Design 10.0.3, Microsoft.EntityFrameworkCore.SqlServer 10.0.3)
- [ ] (4) All package references updated to target versions (**Verify**)
- [ ] (5) Restore dependencies for entire solution
- [ ] (6) All dependencies restored successfully (**Verify**)
- [ ] (7) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog (focus: Identity configuration API, exception handler middleware)
- [ ] (8) Solution builds with 0 errors (**Verify**)

---

### [ ] TASK-003: Run full test suite and validate upgrade
**References**: Plan §Phase 2, Plan §Breaking Changes Catalog

- [ ] (1) Run tests in SportsStore.Tests project
- [ ] (2) Fix any test failures (reference Plan §Breaking Changes for behavioral changes)
- [ ] (3) Re-run tests after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)

---

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all changes with message: "feat: Upgrade solution to .NET 10.0 LTS - Update both projects to net10.0 - Upgrade Entity Framework Core packages to 10.0.3 - Fix API compatibility issues - All builds successful, all tests passing"

---
