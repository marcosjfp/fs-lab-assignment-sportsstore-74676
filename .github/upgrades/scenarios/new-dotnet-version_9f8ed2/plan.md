# .NET 10.0 Upgrade Plan - SportsStore Solution

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Implementation Timeline](#implementation-timeline)
- [Detailed Execution Steps](#detailed-execution-steps)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
Upgrade the SportsStore Blazor solution from **.NET 8.0** to **.NET 10.0 (Long Term Support)**.

### Scope
**Projects to Upgrade:**
- **SportsStore** (main Blazor web application)
- **SportsStore.Tests** (xUnit test project)

**Current State:**
- All projects targeting .NET 8.0
- Entity Framework Core 8.0.0 packages
- Blazor Server application with Identity authentication
- 3,049 total lines of code across 62 files

**Target State:**
- All projects targeting .NET 10.0
- Entity Framework Core 10.0.3 packages
- Updated ASP.NET Core Identity packages
- Full compatibility with .NET 10.0 APIs

### Selected Strategy
**All-At-Once Strategy** - All projects upgraded simultaneously in a single atomic operation.

**Rationale:**
- **Small solution**: Only 2 projects (well below 5-project threshold)
- **Simple dependency structure**: Linear dependency (Tests → SportsStore)
- **Low risk profile**: Both projects marked "Low" difficulty, no security vulnerabilities
- **Small codebase**: 3,049 LOC total
- **Clear package path**: All 3 packages requiring updates have known target versions (10.0.3)
- **Good compatibility**: 76.9% of packages already compatible, minimal API issues (2 source incompatible, 1 behavioral)

This approach minimizes coordination overhead and provides fastest time to completion.

### Discovered Metrics
- **Total Projects**: 2
- **Dependency Depth**: 1 (maximum levels)
- **Total LOC**: 3,049
- **Code Files**: 62 (3 with compatibility incidents)
- **Package Updates Required**: 3 of 13 packages
- **API Issues**: 3 total (2 source incompatible, 1 behavioral change)
- **Security Vulnerabilities**: 0 ✅
- **High-Risk Projects**: 0

### Complexity Classification
**Classification: SIMPLE**

This solution meets all criteria for simple classification:
- ≤5 projects ✅
- Dependency depth ≤2 ✅
- No high-risk projects ✅
- No security vulnerabilities ✅
- Homogeneous technology stack (all .NET 8.0, all SDK-style) ✅

**Iteration Strategy**: Fast batch (2-3 detail iterations covering all projects together)

### Critical Issues
**None** - No security vulnerabilities or blocking issues identified.

### Recommended Approach
Execute as single coordinated upgrade operation with comprehensive testing phase following the atomic update.

---

## Migration Strategy

### Approach Selection: All-At-Once

**Selected Approach**: All projects upgraded simultaneously in a single coordinated operation.

**Justification:**

This solution is an ideal candidate for All-At-Once migration:

✅ **Small Scale**
- Only 2 projects (well below 5-project threshold)
- 3,049 total LOC (small codebase)
- 13 total packages (3 requiring updates)

✅ **Low Complexity**
- Simple linear dependency structure (Tests → Main)
- No circular dependencies
- All projects currently on .NET 8.0 (homogeneous)
- All SDK-style projects (no legacy project format conversions)

✅ **Low Risk Profile**
- Both projects assessed as "Low" difficulty
- No security vulnerabilities
- 76.9% of packages already compatible
- Only 3 API compatibility issues (2 source incompatible, 1 behavioral)
- 3 files with incidents out of 62 total (4.8%)

✅ **Clear Upgrade Path**
- All packages have known target versions (Entity Framework 10.0.3)
- Serilog packages already compatible with .NET 10.0
- No blocking dependencies or compatibility conflicts

### All-At-Once Strategy Characteristics

**Atomic Operation:**
- Update both project files to net10.0 simultaneously
- Update all Entity Framework packages to 10.0.3 together
- Restore dependencies once for entire solution
- Build solution and address all compilation errors in single pass
- Execute all tests after build succeeds

**Advantages for This Solution:**
- Fastest completion time (single upgrade operation)
- No intermediate multi-targeting complexity
- Clean dependency resolution
- Simple coordination (both projects benefit simultaneously)
- Both projects stay in sync throughout

**Execution Order:**
Within the atomic operation, respect dependency flow for validation:
1. Update both project files
2. Update all package references
3. Restore dependencies
4. Build solution (SportsStore compiles first due to dependency order)
5. Fix any compilation errors
6. Rebuild to verify
7. Run tests (validates both projects)

### Parallel vs Sequential Execution

**Project Updates**: Parallel (both project files updated simultaneously)
**Package Updates**: Parallel (all packages updated simultaneously)
**Build Validation**: Sequential (respects dependency order: SportsStore builds before Tests can build)
**Testing**: Sequential (tests run after builds succeed)

### Phase Definition

**Single Migration Phase: Atomic Upgrade**
- All projects included
- All updates applied together
- Single validation checkpoint
- No intermediate states

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The SportsStore solution has a simple, linear dependency structure:

```
SportsStore.Tests.csproj (Test Project)
    └── SportsStore.csproj (Main Blazor Application)
```

**Key Observations:**
- **Leaf Node**: SportsStore.csproj has zero project dependencies
- **Root Node**: SportsStore.Tests.csproj depends only on SportsStore
- **No circular dependencies**
- **Dependency depth**: 1 level
- **Clear migration path**: SportsStore first, then SportsStore.Tests

### Project Groupings for Migration

Since this is an All-At-Once strategy for a small solution, both projects will be upgraded simultaneously in a single atomic operation. However, understanding the dependency structure ensures:

1. Validation occurs in correct order (build SportsStore first, then Tests)
2. Any build errors are addressed respecting dependency flow
3. Testing validates from bottom up (unit tests after main app builds)

**Single Upgrade Phase:**
- **SportsStore** (Blazor app - no dependencies)
- **SportsStore.Tests** (test project - depends on SportsStore)

Both projects upgraded together, validated in dependency order.

### Critical Path

**Critical Path**: SportsStore.csproj → SportsStore.Tests.csproj

Since SportsStore.Tests depends on SportsStore, any compilation issues in SportsStore must be resolved before tests can be successfully built and executed.

### Circular Dependencies

**None detected** ✅

The dependency graph is acyclic, allowing straightforward migration execution.

---

## Implementation Timeline

### Phase 0: Prerequisites Validation

**Operations:**
- Verify .NET 10.0 SDK installed on development machine
- Check for global.json files that might restrict SDK version
- Confirm SQL Server compatibility with Entity Framework Core 10.0

**Deliverables:** 
- Environment ready for .NET 10.0 development

---

### Phase 1: Atomic Upgrade

**Operations** (performed as single coordinated batch):
- Update both project files to net10.0 target framework
- Update all Entity Framework Core packages to 10.0.3
- Update ASP.NET Core Identity package to 10.0.3
- Restore dependencies for entire solution
- Build solution to identify compilation errors
- Fix all compilation errors addressing API compatibility issues
- Rebuild solution to verify all fixes applied

**Deliverables:** 
- Solution builds successfully with 0 errors
- No package dependency conflicts
- All API compatibility issues resolved

---

### Phase 2: Test Validation

**Operations:**
- Execute all tests in SportsStore.Tests project
- Address any test failures related to behavioral changes
- Validate application functionality
- Confirm no regressions introduced

**Deliverables:** 
- All tests pass
- Application functions as expected
- No behavioral regressions detected

---

### Phase 3: Final Validation & Documentation

**Operations:**
- Perform final build verification
- Run comprehensive test suite
- Document any workarounds or patterns discovered
- Update README/documentation if needed

**Deliverables:** 
- Complete, validated upgrade to .NET 10.0
- All success criteria met
- Documentation updated

---

## Detailed Execution Steps

### Step 1: Update Project Files

Update `<TargetFramework>` element in both project files:

**File: SportsStore\SportsStore.csproj**
```xml
<TargetFramework>net10.0</TargetFramework>
```

**File: SportsStore.Tests\SportsStore.Tests.csproj**
```xml
<TargetFramework>net10.0</TargetFramework>
```

---

### Step 2: Update Package References

See [§Package Update Reference](#package-update-reference) for complete details.

**Key Updates (all in SportsStore.csproj):**
- Microsoft.AspNetCore.Identity.EntityFrameworkCore: 8.0.0 → 10.0.3
- Microsoft.EntityFrameworkCore.Design: 8.0.0 → 10.0.3
- Microsoft.EntityFrameworkCore.SqlServer: 8.0.0 → 10.0.3

**No Changes Required:**
- All Serilog packages (already compatible with .NET 10.0)
- All test framework packages in SportsStore.Tests (already compatible)

**Update Method:**
Edit `<PackageReference>` elements in SportsStore\SportsStore.csproj:
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.3">
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.3" />
```

---

### Step 3: Restore Dependencies

Execute dependency restoration:
```bash
dotnet restore SportsSln.sln
```

**Expected Outcome:** All packages restored successfully with no conflicts.

---

### Step 4: Build Solution and Address Breaking Changes

**Build Command:**
```bash
dotnet build SportsSln.sln --no-restore
```

**Expected Compilation Issues:**

See [§Breaking Changes Catalog](#breaking-changes-catalog) for comprehensive details.

**Key Areas to Address:**
1. **Identity Configuration** (Source Incompatible)
   - File: `Program.cs` or `Startup.cs`
   - Issue: `IdentityEntityFrameworkBuilderExtensions.AddEntityFrameworkStores<TContext>`
   - Resolution: API signature may have changed, verify generic parameter usage

2. **Exception Handler Middleware** (Behavioral Change)
   - File: `Program.cs` or middleware configuration
   - Issue: `UseExceptionHandler(IApplicationBuilder, string)` behavior changed
   - Resolution: Review exception handling configuration, test error pages

**Fix Approach:**
- Address errors in dependency order (SportsStore first, then Tests)
- Use assessment data to identify affected files
- Apply fixes from Breaking Changes Catalog
- Verify each fix before moving to next

---

### Step 5: Rebuild and Verify

After applying all fixes:
```bash
dotnet build SportsSln.sln --no-restore
```

**Expected Outcome:** Solution builds with 0 errors and 0 warnings.

---

### Step 6: Execute Tests

Run all tests in SportsStore.Tests project:
```bash
dotnet test SportsSln.sln --no-build
```

**Expected Outcome:** All tests pass.

**If Tests Fail:**
- Review failure messages for behavioral change indicators
- Check for test framework compatibility issues
- Validate mock configurations (Moq 4.16.1) still compatible
- Address test-specific .NET 10 compatibility issues

---

### Step 7: Final Validation

**Build Verification:**
```bash
dotnet build SportsSln.sln --configuration Release
```

**Test Verification:**
```bash
dotnet test SportsSln.sln --configuration Release
```

**Manual Validation:**
- Run the Blazor application locally
- Verify authentication/authorization works (Identity integration)
- Test key user workflows
- Check logging functionality (Serilog)
- Validate database connectivity (Entity Framework)

**Expected Outcome:** Application runs correctly with full functionality.

---

## Project-by-Project Migration Plans

### Project: SportsStore (Main Blazor Application)

**Path:** `SportsStore\SportsStore.csproj`

#### Current State
- **Target Framework:** net8.0
- **Project Type:** AspNetCore Blazor Server
- **SDK-Style:** Yes
- **Dependencies:** 0 project dependencies
- **Dependants:** 1 (SportsStore.Tests)
- **Package Count:** 8 packages
- **Lines of Code:** 2,504
- **Files:** 68 total (2 with compatibility incidents)
- **Risk Level:** Low

**Current Packages:**
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.0
- Microsoft.EntityFrameworkCore.Design 8.0.0
- Microsoft.EntityFrameworkCore.SqlServer 8.0.0
- Serilog 4.3.1
- Serilog.AspNetCore 10.0.0
- Serilog.Sinks.Console 6.1.1
- Serilog.Sinks.File 7.0.0
- Serilog.Sinks.Seq 9.0.0

#### Target State
- **Target Framework:** net10.0
- **Updated Package Count:** 3 packages requiring version updates

**Target Packages:**
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.3
- Microsoft.EntityFrameworkCore.Design 10.0.3
- Microsoft.EntityFrameworkCore.SqlServer 10.0.3
- All Serilog packages remain at current versions (already compatible)

#### Migration Steps

**1. Prerequisites**
- .NET 10.0 SDK installed
- SQL Server compatible with EF Core 10.0
- No blocking dependencies (project is leaf node)

**2. Framework Update**
Update `SportsStore\SportsStore.csproj`:
- Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`

**3. Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.0 | 10.0.3 | Framework compatibility, includes Identity improvements |
| Microsoft.EntityFrameworkCore.Design | 8.0.0 | 10.0.3 | Framework compatibility, migration tooling support |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.0 | 10.0.3 | Framework compatibility, SQL Server provider updates |

**4. Expected Breaking Changes**

**API Compatibility Issues:**

**(a) Source Incompatible - Identity Entity Framework Extensions**
- **Type:** `Microsoft.Extensions.DependencyInjection.IdentityEntityFrameworkBuilderExtensions`
- **Member:** `AddEntityFrameworkStores<TContext>(IdentityBuilder)`
- **Impact:** Source-level incompatibility requiring recompilation
- **Affected File(s):** Likely `Program.cs` where Identity is configured
- **Resolution:** Review generic parameter usage, ensure context type properly specified

**(b) Source Incompatible - AddEntityFrameworkStores Method**
- **API:** `AddEntityFrameworkStores<TContext>`
- **Impact:** Method signature or constraints may have changed
- **Resolution:** Verify `TContext` derives from `IdentityDbContext` correctly

**(c) Behavioral Change - Exception Handler Middleware**
- **API:** `UseExceptionHandler(IApplicationBuilder, string)`
- **Impact:** Exception handling behavior may differ in .NET 10
- **Affected File(s):** `Program.cs` or middleware configuration
- **Resolution:** Review exception handling configuration, test error page routing

**5. Code Modifications**

**Program.cs / Startup.cs:**
- Review Identity configuration:
  ```csharp
  builder.Services.AddDefaultIdentity<IdentityUser>(options => ...)
      .AddEntityFrameworkStores<ApplicationDbContext>();
  ```
- Verify generic type constraints satisfied
- Check exception handler configuration:
  ```csharp
  app.UseExceptionHandler("/Error");
  ```
- Test error handling behavior

**Entity Framework Context:**
- Verify DbContext derived from IdentityDbContext correctly
- Check for EF Core 10.0 breaking changes in model configuration
- Review any custom Identity entity configurations

**Configuration Files:**
- Verify `appsettings.json` connection strings compatible
- Check for any .NET 10 configuration schema changes

**Blazor Components:**
- Review for .NET 10 Blazor improvements/changes
- Check component lifecycle methods
- Verify JavaScript interop compatibility
- Test enhanced navigation features

**6. Testing Strategy**

**Unit Tests:**
- Run SportsStore.Tests project (covers this project)
- All existing tests should pass

**Integration Tests:**
- Test Identity authentication flows (login, logout, registration)
- Verify database connectivity and EF Core operations
- Test Blazor component rendering
- Validate navigation and routing

**Manual Testing:**
- Launch application and verify startup
- Test user authentication workflows
- Verify database operations (CRUD)
- Check logging output (Serilog)
- Test error handling (exception pages)

**Performance Testing:**
- Compare application startup time
- Verify Blazor rendering performance
- Check database query performance

**7. Validation Checklist**

- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] All Entity Framework packages updated to 10.0.3
- [ ] Identity configuration compiles and runs
- [ ] Exception handling works correctly
- [ ] Blazor components render properly
- [ ] Database migrations compatible
- [ ] All dependent tests pass (SportsStore.Tests)
- [ ] Authentication/authorization functional
- [ ] Logging works (Serilog)
- [ ] No package version conflicts
- [ ] No security vulnerabilities introduced

---

### Project: SportsStore.Tests (Test Project)

**Path:** `SportsStore.Tests\SportsStore.Tests.csproj`

#### Current State
- **Target Framework:** net8.0
- **Project Type:** DotNetCoreApp (xUnit test project)
- **SDK-Style:** Yes
- **Dependencies:** 1 project dependency (SportsStore)
- **Dependants:** 0
- **Package Count:** 8 packages
- **Lines of Code:** 545
- **Files:** 9 total (1 with compatibility incident)
- **Risk Level:** Low

**Current Packages:**
- coverlet.collector 3.1.0
- Microsoft.NET.Test.Sdk 16.11.0
- Moq 4.16.1
- xunit 2.4.1
- xunit.runner.visualstudio 2.4.3
- Serilog 4.3.1
- Serilog.AspNetCore 10.0.0
- Serilog.Sinks.Console 6.1.1
- Serilog.Sinks.File 7.0.0
- Serilog.Sinks.Seq 9.0.0

(Note: Some Serilog packages shared with SportsStore project)

#### Target State
- **Target Framework:** net10.0
- **Updated Package Count:** 0 packages requiring version updates (all already compatible)

**Target Packages:**
- All packages remain at current versions (all compatible with .NET 10.0)

#### Migration Steps

**1. Prerequisites**
- SportsStore project must be upgraded first (dependency)
- SportsStore project must build successfully

**2. Framework Update**
Update `SportsStore.Tests\SportsStore.Tests.csproj`:
- Change `<TargetFramework>net8.0</TargetFramework>` to `<TargetFramework>net10.0</TargetFramework>`

**3. Package Updates**

| Package | Current Version | Target Version | Reason |
|---------|----------------|----------------|---------|
| (none) | - | - | All packages already compatible with .NET 10.0 |

**No package updates required** - All test framework packages and Serilog packages are compatible with .NET 10.0.

**4. Expected Breaking Changes**

**API Compatibility Issues:**
- **None identified** for this project
- All 672 APIs analyzed are compatible

**Potential Test-Related Changes:**
- xUnit behavior should remain consistent
- Moq 4.16.1 compatible with .NET 10.0
- Test SDK 16.11.0 supports .NET 10.0

**5. Code Modifications**

**Expected:** Minimal to no code changes required.

**Areas to Review:**
- Test fixtures and setup code
- Mock configurations (Moq)
- Assertions and test patterns
- Any tests depending on .NET 8-specific behavior

**If Compilation Errors Occur:**
- Review test method signatures
- Check for obsolete test attributes
- Verify assertion library compatibility

**6. Testing Strategy**

**Build Validation:**
- Project must compile after framework update
- No new warnings introduced

**Test Execution:**
- Run all tests in project:
  ```bash
  dotnet test SportsStore.Tests\SportsStore.Tests.csproj
  ```
- All tests should pass

**Test Coverage:**
- Verify existing test coverage maintained
- No tests skipped or disabled
- All test scenarios execute successfully

**7. Validation Checklist**

- [ ] Project builds without errors
- [ ] Project builds without warnings
- [ ] Project references SportsStore successfully
- [ ] All tests compile
- [ ] All tests execute
- [ ] All tests pass
- [ ] Test coverage unchanged
- [ ] No test framework compatibility issues
- [ ] Mock configurations work correctly
- [ ] Test output logging functional

---

## Package Update Reference

### Packages Requiring Updates

**Entity Framework Core & Identity Packages** (SportsStore project only):

| Package | Current | Target | Projects Affected | Update Reason |
|---------|---------|--------|-------------------|---------------|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 8.0.0 | 10.0.3 | 1 (SportsStore) | Framework compatibility, Identity API updates for .NET 10 |
| Microsoft.EntityFrameworkCore.Design | 8.0.0 | 10.0.3 | 1 (SportsStore) | Framework compatibility, migration tooling updates |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.0 | 10.0.3 | 1 (SportsStore) | Framework compatibility, SQL Server provider improvements |

**Update Notes:**
- All three packages are from Microsoft Entity Framework Core family
- Coordinated version update (all to 10.0.3)
- Design package maintains special attributes (`IncludeAssets`, `PrivateAssets`)

---

### Compatible Packages (No Updates Required)

**Logging Packages** (both projects):

| Package | Current Version | Projects Using | Status |
|---------|----------------|----------------|---------|
| Serilog | 4.3.1 | SportsStore, SportsStore.Tests | ✅ Compatible with .NET 10.0 |
| Serilog.AspNetCore | 10.0.0 | SportsStore, SportsStore.Tests | ✅ Compatible with .NET 10.0 |
| Serilog.Sinks.Console | 6.1.1 | SportsStore, SportsStore.Tests | ✅ Compatible with .NET 10.0 |
| Serilog.Sinks.File | 7.0.0 | SportsStore, SportsStore.Tests | ✅ Compatible with .NET 10.0 |
| Serilog.Sinks.Seq | 9.0.0 | SportsStore, SportsStore.Tests | ✅ Compatible with .NET 10.0 |

**Test Framework Packages** (SportsStore.Tests only):

| Package | Current Version | Status |
|---------|----------------|---------|
| Microsoft.NET.Test.Sdk | 16.11.0 | ✅ Compatible with .NET 10.0 |
| xunit | 2.4.1 | ✅ Compatible with .NET 10.0 |
| xunit.runner.visualstudio | 2.4.3 | ✅ Compatible with .NET 10.0 |
| Moq | 4.16.1 | ✅ Compatible with .NET 10.0 |
| coverlet.collector | 3.1.0 | ✅ Compatible with .NET 10.0 |

---

### Package Update Summary

**Total Packages:** 13 (accounting for shared packages)
- **Require Updates:** 3 (23.1%)
- **Already Compatible:** 10 (76.9%)

**Update Approach:** All 3 package updates applied simultaneously in atomic operation.

---

## Breaking Changes Catalog

### Source Incompatible Changes

#### 1. IdentityEntityFrameworkBuilderExtensions Type

**Category:** Source Incompatible (🟡)  
**Affected API:** `Microsoft.Extensions.DependencyInjection.IdentityEntityFrameworkBuilderExtensions`  
**Package:** Microsoft.AspNetCore.Identity.EntityFrameworkCore  
**Project:** SportsStore

**Description:**
The `IdentityEntityFrameworkBuilderExtensions` type has source-level incompatibility in .NET 10.0. This type provides extension methods for configuring Entity Framework stores for Identity.

**Impact:**
- Code will need recompilation
- May require syntax adjustments in Identity configuration
- Potential for new compiler errors or warnings

**Affected Code Location:**
- **File:** `Program.cs` (or `Startup.cs` if using older pattern)
- **Pattern:** Identity service registration code
  ```csharp
  builder.Services.AddDefaultIdentity<IdentityUser>(options => ...)
      .AddEntityFrameworkStores<ApplicationDbContext>();
  ```

**Resolution Steps:**
1. Build solution after package update to identify specific error
2. Review compiler error message for exact issue
3. Common fixes:
   - Verify generic type parameter constraints
   - Check DbContext type derives from IdentityDbContext
   - Update using statements if namespace changed
   - Verify method overload selection

**Verification:**
- Code compiles without errors
- Identity services register successfully
- Application starts without Identity configuration errors

---

#### 2. AddEntityFrameworkStores Method

**Category:** Source Incompatible (🟡)  
**Affected API:** `IdentityEntityFrameworkBuilderExtensions.AddEntityFrameworkStores<TContext>(IdentityBuilder)`  
**Package:** Microsoft.AspNetCore.Identity.EntityFrameworkCore  
**Project:** SportsStore

**Description:**
The `AddEntityFrameworkStores` extension method has source-level changes in .NET 10.0.

**Impact:**
- Method signature may have changed
- Generic parameter constraints may be stricter
- Overload resolution may differ

**Affected Code Location:**
- **File:** `Program.cs`
- **Usage:** Chained after `AddDefaultIdentity` or `AddIdentity`

**Resolution Steps:**
1. Review compiler error for specific incompatibility
2. Check that `TContext` parameter properly typed
3. Verify `TContext : IdentityDbContext` constraint satisfied
4. Update method call syntax if required

**Verification:**
- Method call compiles successfully
- Identity stores configured correctly
- User authentication/authorization works

---

### Behavioral Changes

#### 3. UseExceptionHandler Middleware

**Category:** Behavioral Change (🔵)  
**Affected API:** `Microsoft.AspNetCore.Builder.ExceptionHandlerExtensions.UseExceptionHandler(IApplicationBuilder, string)`  
**Package:** Microsoft.AspNetCore (implicit in framework)  
**Project:** SportsStore

**Description:**
The `UseExceptionHandler` middleware behavior has changed in .NET 10.0. This affects how unhandled exceptions are processed and error pages are displayed.

**Impact:**
- Exception handling behavior may differ
- Error page routing may work differently
- Exception logging patterns may change
- Status code handling may vary

**Affected Code Location:**
- **File:** `Program.cs`
- **Pattern:** Middleware configuration
  ```csharp
  if (!app.Environment.IsDevelopment())
  {
      app.UseExceptionHandler("/Error");
      app.UseHsts();
  }
  ```

**Behavioral Differences to Test:**
- Error page display in production mode
- Exception information exposure
- Status code preservation
- Exception propagation to logging middleware

**Resolution Steps:**
1. Code will compile (behavioral change, not API change)
2. **Must test exception handling at runtime**
3. Trigger test exceptions in different scenarios
4. Verify error pages display correctly
5. Confirm exceptions logged properly (Serilog integration)

**Testing Requirements:**
- Test unhandled exceptions in production mode
- Verify error page renders at `/Error` route
- Check exception details logged correctly
- Validate status codes (500, 404, etc.)
- Test HSTS behavior with error handling

**Verification:**
- Error pages display as expected
- No sensitive exception information leaked in production
- Logging captures exception details
- User experience acceptable for error scenarios

---

### Framework-Level Breaking Changes

**Potential .NET 10.0 Framework Changes:**

While specific breaking changes depend on .NET 10.0 release notes, common areas include:

**ASP.NET Core / Blazor:**
- Middleware ordering requirements
- Authentication/authorization defaults
- Blazor component lifecycle changes
- Enhanced navigation behavior
- Form handling improvements

**Entity Framework Core:**
- Query translation improvements (may expose previously hidden issues)
- Migration generation changes
- Database provider behavior updates
- Lazy loading behavior
- Connection resilience defaults

**General .NET 10:**
- Obsolete API removals (deprecated in .NET 8/9)
- Performance optimizations affecting timing
- Library consolidation
- Implicit using changes

**Mitigation:**
- Monitor build warnings for obsolete APIs
- Review .NET 10.0 release notes for breaking changes
- Test comprehensively after upgrade
- Check for runtime behavior differences

---

### Breaking Changes Summary

| Category | Count | Severity | Action Required |
|----------|-------|----------|-----------------|
| Binary Incompatible | 0 | High | None |
| Source Incompatible | 2 | Medium | Fix during compilation |
| Behavioral Change | 1 | Low | Test at runtime |

**Overall Impact:** Low to Medium - Small number of issues, well-defined resolution paths, no binary incompatibilities.

---

## Risk Management

### High-Risk Changes

| Project | Risk Level | Description | Mitigation |
|---------|-----------|-------------|------------|
| N/A | N/A | No high-risk changes identified | N/A |

**Risk Assessment:**
- Both projects assessed as **Low Difficulty**
- No security vulnerabilities present
- Minimal API surface changes (3 issues total)
- Small codebase reduces scope of unexpected issues
- Comprehensive test project provides validation safety net

### All-At-Once Strategy Risk Factors

**Consolidated Update Risk**: All projects change simultaneously, creating single large change surface.

**Mitigation:**
- Solution is small (2 projects, 3K LOC) - manageable change surface
- Good test coverage via SportsStore.Tests project
- Clear rollback path (revert all changes atomically)
- Build validation before testing reduces cascade failures

### Security Vulnerabilities

**None identified** ✅

All current package versions are secure. Updated packages (Entity Framework 10.0.3) maintain security posture.

### Contingency Plans

**If Entity Framework package updates cause issues:**
- Verify SQL Server compatibility with EF Core 10.0
- Check for breaking changes in EF Core 10.0 release notes
- Consider EF Core migrations regeneration if needed
- Fallback: Stay on EF Core 9.0 if 10.0 blocks (still .NET 10 compatible)

**If ASP.NET Core Identity issues arise:**
- Review Identity schema changes between versions
- Verify authentication/authorization middleware configuration
- Check for breaking changes in Identity API
- Validate user management and role-based access still functions

**If Blazor-specific issues occur:**
- Review Blazor component lifecycle changes in .NET 10
- Check for rendering behavior changes
- Validate JavaScript interop compatibility
- Test enhanced navigation and form handling features

**If compilation errors persist:**
- Review .NET 10.0 breaking changes documentation
- Check for obsolete API usage
- Verify implicit usings compatibility
- Consider explicit namespace imports if needed

### Rollback Strategy

**Clean Rollback Path:**
- All changes in single atomic operation
- Git branch isolation (upgrade-to-NET10)
- Simple revert if needed: discard all changes on upgrade branch
- Source branch (first-changes) remains unchanged

---

## Testing & Validation Strategy

### Multi-Level Testing Approach

Given the All-At-Once strategy, testing occurs after the atomic upgrade completes successfully.

---

### Level 1: Build Validation

**Objective:** Ensure solution compiles with no errors or warnings.

**Scope:** Entire solution (both projects)

**Steps:**
1. Clean build output:
   ```bash
   dotnet clean SportsSln.sln
   ```

2. Build solution in dependency order:
   ```bash
   dotnet build SportsSln.sln --no-incremental
   ```

3. Verify output:
   - 0 build errors
   - 0 warnings
   - All projects successfully compiled

**Success Criteria:**
- [ ] SportsStore project builds successfully
- [ ] SportsStore.Tests project builds successfully
- [ ] No compilation errors
- [ ] No build warnings
- [ ] No package restore failures

---

### Level 2: Automated Test Execution

**Objective:** Verify existing functionality through automated tests.

**Scope:** SportsStore.Tests project (covers SportsStore application)

**Steps:**
1. Execute all tests:
   ```bash
   dotnet test SportsSln.sln --no-build --verbosity normal
   ```

2. Review test results:
   - Total tests executed
   - Passed/Failed/Skipped counts
   - Execution time

3. If failures occur:
   - Analyze failure messages
   - Correlate with behavioral changes (UseExceptionHandler)
   - Determine if test or application code needs adjustment

**Test Categories to Validate:**
- Unit tests (business logic)
- Integration tests (database operations)
- Component tests (Blazor components)
- Authentication/authorization tests
- Repository pattern tests

**Success Criteria:**
- [ ] All tests execute
- [ ] 100% tests pass (or failures explained and addressed)
- [ ] No tests skipped unexpectedly
- [ ] Test execution time reasonable
- [ ] Code coverage maintained

---

### Level 3: Smoke Testing

**Objective:** Quick validation of critical application functionality.

**Scope:** SportsStore Blazor application

**Manual Tests:**

**Application Startup:**
- [ ] Application starts without errors
- [ ] No startup exceptions logged
- [ ] Serilog logging initializes correctly
- [ ] Database connection established

**Authentication & Identity:**
- [ ] Login page renders
- [ ] User can log in successfully
- [ ] User session maintained
- [ ] Logout functions correctly
- [ ] Registration process works (if applicable)
- [ ] Role-based access control functions

**Core Functionality:**
- [ ] Home page renders correctly
- [ ] Product catalog displays
- [ ] Shopping cart operations work
- [ ] Order processing functional
- [ ] Admin features accessible (if applicable)

**Data Operations:**
- [ ] Entity Framework queries execute
- [ ] CRUD operations complete successfully
- [ ] Database transactions work
- [ ] Data validation functions

**Error Handling:**
- [ ] Error pages display correctly (test UseExceptionHandler behavior)
- [ ] Exceptions logged to Serilog
- [ ] User-friendly error messages shown
- [ ] Application recovers gracefully

---

### Level 4: Comprehensive Validation

**Objective:** Thorough validation of upgraded solution.

**Performance Testing:**
- [ ] Application startup time acceptable
- [ ] Page load times unchanged or improved
- [ ] Database query performance maintained
- [ ] Blazor component rendering responsive

**Compatibility Testing:**
- [ ] Browser compatibility maintained
- [ ] JavaScript interop works correctly
- [ ] Static files served properly
- [ ] CSS/styling renders correctly

**Integration Testing:**
- [ ] Database migrations compatible
- [ ] Third-party service integrations work (if any)
- [ ] Logging sinks operational (Console, File, Seq)
- [ ] Configuration loading correct

**Security Testing:**
- [ ] Authentication mechanisms secure
- [ ] Authorization policies enforced
- [ ] No new security warnings
- [ ] Package vulnerabilities: 0

---

### Testing Execution Order

Following All-At-Once strategy and dependency structure:

**Sequential Validation:**
1. **Build SportsStore** → Must succeed first (leaf dependency)
2. **Build SportsStore.Tests** → Depends on SportsStore
3. **Run automated tests** → Validates both projects
4. **Smoke test application** → Runtime validation
5. **Comprehensive validation** → Final verification

---

### Test Failure Response Plan

**If Build Fails:**
1. Review compilation errors
2. Consult Breaking Changes Catalog
3. Apply fixes in dependency order (SportsStore first)
4. Rebuild incrementally
5. Document any unexpected issues

**If Tests Fail:**
1. Categorize failures (setup, assertion, behavioral)
2. Correlate with known behavioral changes
3. Determine if test or application needs adjustment
4. Fix and re-run
5. Update test documentation if patterns changed

**If Smoke Tests Reveal Issues:**
1. Check logs for exceptions or warnings
2. Reproduce issue in debugger
3. Correlate with API compatibility issues
4. Apply targeted fixes
5. Re-validate affected scenarios

---

### Validation Checkpoints

**Checkpoint 1: Post-Build**
- Solution builds successfully
- No errors or warnings
- All packages restored

**Checkpoint 2: Post-Test**
- All automated tests pass
- No test regressions
- Test execution stable

**Checkpoint 3: Post-Smoke-Test**
- Application runs successfully
- Critical workflows functional
- Error handling works correctly

**Checkpoint 4: Final**
- All success criteria met
- Documentation updated
- Ready for deployment

---

## Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | Risk | Dependencies | Package Updates | API Issues | LOC |
|---------|-----------|------|--------------|-----------------|------------|-----|
| SportsStore | Low | Low | 0 projects | 3 packages | 3 issues | 2,504 |
| SportsStore.Tests | Low | Low | 1 project | 0 packages | 0 issues | 545 |

### Phase Complexity Assessment

**Single Phase: Atomic Upgrade**

**Complexity: Low**

**Factors:**
- Small codebase (3K LOC total)
- Straightforward dependency structure
- Minimal package updates (3 packages, all from same vendor - Microsoft EF)
- Low API impact (3 issues, none binary incompatible)
- Both projects homogeneous (.NET 8 SDK-style)

**Key Activities:**
1. Project file updates (2 files)
2. Package updates (3 Entity Framework packages)
3. Dependency restoration
4. Build and fix compilation errors
5. Test execution and validation

### Resource Requirements

**Skills Needed:**
- .NET framework upgrade experience (mid-level)
- Entity Framework Core knowledge (basic-to-mid)
- Blazor/ASP.NET Core familiarity (basic)
- xUnit testing knowledge (basic)

**Parallel Capacity:**
- All updates can be performed in single pass
- Single developer can execute entire upgrade
- No coordination overhead between teams

**Relative Effort: Low**
- Simple solution structure
- Clear upgrade path
- Minimal unknowns
- Good tooling support (.NET SDK, EF migrations)

---

## Source Control Strategy

### Branching Strategy

**Source Branch:** `first-changes`
- Current working branch with .NET 8.0 codebase
- Remains unchanged during upgrade
- Fallback point if upgrade needs to be abandoned

**Upgrade Branch:** `upgrade-to-NET10`
- Dedicated branch for all .NET 10.0 upgrade work
- Isolates upgrade changes from main development
- Allows testing and validation before merge

**Branch Workflow:**
1. Ensure all pending changes on `first-changes` are committed/stashed
2. Create `upgrade-to-NET10` branch from `first-changes`
3. Perform all upgrade work on `upgrade-to-NET10`
4. Keep branch until upgrade fully validated
5. Merge to `first-changes` (or main) after success

---

### Commit Strategy

**All-At-Once Commit Approach:**

Given the atomic nature of this upgrade, prefer a single commit approach with clear, detailed message.

**Recommended Single Commit:**

```
feat: Upgrade solution to .NET 10.0 LTS

- Update SportsStore project to net10.0 target framework
- Update SportsStore.Tests project to net10.0 target framework
- Upgrade Microsoft.AspNetCore.Identity.EntityFrameworkCore to 10.0.3
- Upgrade Microsoft.EntityFrameworkCore.Design to 10.0.3
- Upgrade Microsoft.EntityFrameworkCore.SqlServer to 10.0.3
- Fix Identity configuration API compatibility issues
- Address UseExceptionHandler behavioral changes
- All builds successful, all tests passing

Breaking Changes:
- Identity Entity Framework stores configuration updated
- Exception handler middleware behavior validated

Validated:
- Solution builds with 0 errors, 0 warnings
- All automated tests pass
- Application starts and runs correctly
- Authentication/authorization functional
```

**Alternative: Staged Commits (if preferred)**

If you prefer checkpoint commits during the upgrade:

**Commit 1: Framework & Package Updates**
```
chore: Update target framework and packages to .NET 10.0

- Update both projects to net10.0
- Upgrade Entity Framework Core packages to 10.0.3
- Upgrade Identity EntityFrameworkCore to 10.0.3
```

**Commit 2: API Compatibility Fixes**
```
fix: Resolve .NET 10.0 API compatibility issues

- Fix Identity EntityFrameworkStores configuration
- Address source incompatibilities in Program.cs
```

**Commit 3: Validation & Testing**
```
test: Validate .NET 10.0 upgrade completion

- All tests passing
- Application functional
- Exception handling validated
```

---

### Review and Merge Process

**Pre-Merge Checklist:**
- [ ] All success criteria met (see Success Criteria section)
- [ ] Solution builds in Release configuration
- [ ] All tests pass in Release configuration
- [ ] Application tested manually
- [ ] No warnings or errors
- [ ] Documentation updated (if applicable)

**Merge Approach:**
- Create Pull Request from `upgrade-to-NET10` to `first-changes`
- Include summary of changes and validation results
- Reference this plan.md in PR description
- Wait for CI/CD validation (if configured)
- Merge after approval

**Post-Merge:**
- Tag the merge commit: `v10.0-migration-complete`
- Keep upgrade branch temporarily (1-2 weeks) in case of issues
- Monitor production for unexpected behavior
- Delete upgrade branch after stability confirmed

---

### Commit Best Practices

**Message Format:**
- Use conventional commit types: `feat:`, `fix:`, `chore:`, `test:`
- Include scope: `feat(framework): upgrade to .NET 10.0`
- Reference issue numbers if applicable
- List specific changes in bullet points

**What to Commit:**
- Project files (.csproj)
- Code fixes for compatibility
- Updated configurations
- Test updates
- Documentation changes

**What NOT to Commit:**
- bin/ and obj/ folders (should be in .gitignore)
- User-specific files (.vs/, *.user)
- Temporary files
- NuGet package files (packages/)

**Commit Frequency:**
- For atomic upgrade: Single commit preferred
- For staged approach: One commit per logical checkpoint
- **User controls Git** - follow user's commit preferences and workflow

---

## Success Criteria

### Technical Criteria

**Framework Migration:**
- [ ] SportsStore.csproj targets net10.0
- [ ] SportsStore.Tests.csproj targets net10.0
- [ ] Both projects successfully compiled

**Package Updates:**
- [ ] Microsoft.AspNetCore.Identity.EntityFrameworkCore updated to 10.0.3
- [ ] Microsoft.EntityFrameworkCore.Design updated to 10.0.3
- [ ] Microsoft.EntityFrameworkCore.SqlServer updated to 10.0.3
- [ ] All package dependencies resolved without conflicts
- [ ] No package downgrade warnings

**Build Success:**
- [ ] Solution builds without errors (`dotnet build SportsSln.sln`)
- [ ] Solution builds without warnings
- [ ] Release configuration builds successfully
- [ ] All projects produce valid output assemblies

**Test Success:**
- [ ] All tests in SportsStore.Tests project pass
- [ ] No test regressions introduced
- [ ] Test execution stable and repeatable
- [ ] Code coverage maintained at previous levels

**API Compatibility:**
- [ ] Identity EntityFrameworkStores configuration compiles
- [ ] UseExceptionHandler middleware functions correctly
- [ ] All source incompatibilities resolved
- [ ] Behavioral changes validated through testing

**Security:**
- [ ] No package vulnerabilities present
- [ ] No new security warnings introduced
- [ ] Authentication/authorization functional
- [ ] No sensitive data exposure

---

### Quality Criteria

**Code Quality:**
- [ ] No new compiler warnings introduced
- [ ] Code analysis passes (if configured)
- [ ] No code smell introductions
- [ ] Consistent coding patterns maintained

**Test Coverage:**
- [ ] Existing test coverage percentage maintained or improved
- [ ] All test categories passing (unit, integration)
- [ ] No tests disabled or commented out

**Documentation:**
- [ ] README updated with .NET 10.0 requirements (if applicable)
- [ ] Dependencies documented
- [ ] Breaking changes documented for team
- [ ] Configuration changes noted

**Performance:**
- [ ] Application startup time acceptable
- [ ] Blazor rendering performance maintained
- [ ] Database query performance unchanged or improved
- [ ] No performance regressions detected

---

### Process Criteria

**All-At-Once Strategy Adherence:**
- [ ] All projects upgraded simultaneously (no intermediate states)
- [ ] Single atomic operation completed
- [ ] Dependency order respected during validation
- [ ] No partial upgrade states

**Source Control:**
- [ ] All changes committed with clear messages
- [ ] Changes isolated on upgrade-to-NET10 branch
- [ ] Source branch (first-changes) unaffected
- [ ] Git history clean and understandable

**Validation Completeness:**
- [ ] All validation checkpoints passed
- [ ] Build validation successful
- [ ] Automated tests successful
- [ ] Smoke tests successful
- [ ] Manual validation completed

---

### Definition of Done

**The migration is complete when:**

1. ✅ **All projects successfully targeting .NET 10.0**
2. ✅ **All required packages updated to target versions**
3. ✅ **Solution builds with 0 errors and 0 warnings**
4. ✅ **All automated tests pass**
5. ✅ **Application runs and core functionality validated**
6. ✅ **No security vulnerabilities present**
7. ✅ **All API compatibility issues resolved**
8. ✅ **Behavioral changes tested and validated**
9. ✅ **Changes committed to upgrade branch**
10. ✅ **Documentation updated (if needed)**

**Ready for Merge When:**
- All above criteria met
- Stakeholder approval obtained (if required)
- Pre-merge checklist completed (see Source Control Strategy)

**Migration Success Indicator:**
> "The SportsStore Blazor application and its test project are fully operational on .NET 10.0 LTS with all Entity Framework Core packages updated to version 10.0.3, zero security vulnerabilities, and complete test coverage validation."
