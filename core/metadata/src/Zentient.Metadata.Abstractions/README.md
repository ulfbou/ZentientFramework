# Zentient.Metadata.Abstractions

**Forward-Compatible Abstractions Staging Area**

This package provides forward-compatible abstractions under the `Zentient.Abstractions.Metadata` namespace family. These will eventually be relocated into the root `Zentient.Abstractions` package once it is reopened for contributions. Consumers can rely on namespace stability.

- **Namespace alignment:** All contracts use `Zentient.Abstractions.Metadata.*` namespaces for zero-churn migration.
- **Purpose:** This is a staging area for abstractions that may graduate to core.
- **Migration:** When the core package is reopened, only the package reference will change—no code or namespace changes required.

> **Note:** If you are looking for implementation or concrete types, see the `Zentient.Metadata` and `Zentient.Metadata.Diagnostics` packages.
