Here is the **perfect `CHANGELOG.md` template** for a Zentient Framework library—modeled after `Zentient.Endpoints`, aligned with Conventional Commits, developer-first principles, and ecosystem modularity:

---

# 📦 CHANGELOG

> All notable changes to this project will be documented in this file.
> The format is based on [Conventional Commits](https://www.conventionalcommits.org/) and this project adheres to [Semantic Versioning](https://semver.org/).

---

## \[Unreleased]

> **Planned changes not yet released**

### ✨ Added

* Initial roadmap items or planned APIs (e.g., `Zentient.{ModuleName}.Grpc`)
* Placeholder for future transport adapters or diagnostics hooks

### 🛠 Changed

* Placeholder for breaking or behavioral changes coming in future releases

### 🧹 Removed

* Deprecated behaviors, code paths, or obsolete APIs pending removal

---

## \[v0.1.0] — YYYY-MM-DD

> 🎉 First public release

### ✨ Added

* Introduced core abstractions: `I{ModuleInterface}`, `Zentient.{ModuleName}` base types
* Functional extensions for fluent usage with `IResult<T>`
* DI registration via `AddZentient{ModuleName}()`
* Basic observability hooks (e.g., logger/metadata support)

### 🔧 Infrastructure

* CI/CD pipeline using GitHub Actions with .NET 8 and .NET 9 matrix
* NuGet publishing with source link and deterministic builds
* Initialized full documentation structure (`README.md`, `docs/`, `docfx.json`)
* Applied `.editorconfig`, license, and code-style baselines

---

## \[v0.1.0-preview\.1] — YYYY-MM-DD

> 🧪 Internal pre-release for integrators and collaborators

### ✨ Added

* Preview version of `Zentient.{ModuleName}.Http` (or equivalent transport adapter)
* Early support for `Bind(...)` and exception-wrapping behavior
* Experimental DI hooks and extension patterns

---

## 💡 Versioning Guidelines

* **Patch (`x.y.Z`)**: Fixes and backward-compatible improvements
* **Minor (`x.Y.z`)**: New features, safe for all consumers
* **Major (`X.y.z`)**: Breaking changes, redesigns, or removed APIs

---

## 📜 Past Releases

> Earlier releases (if any) will be listed here in descending order once versioning is formalized.

---

> Maintained with ❤️ by [@ulfbou](https://github.com/ulfbou) and Zentient contributors.
