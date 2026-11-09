# AGENTS.md

How to work in `com.rwth.unity.omilaxr.hyperate`. Architecture and commands are in `CLAUDE.md`. Workspace-wide context is in `../CLAUDE.md`.

## Repository and releases

- This folder is its own git repo inside the `omilaxr-ecosystem` workspace. Run git commands here.
- Work for a release goes on a branch `vX.Y.Z`, which is merged into `main` when done.
- Commit messages: `type(scope): summary`, where type is `feat`, `fix`, `refactor`, `test` or `docs`. Do not commit unrelated working-tree changes that you did not make.
- There is no publish workflow in this repo. Releases are done manually.

## CHANGELOG

- There is no `CHANGELOG.md` yet. With the first user-visible change, create one in the core format (`# Changelog`, then `## [X.Y.Z] - YYYY-MM-DD` with `### Added/Changed/Fixed`), plus its `.meta`.
- Group entries under `### Added`, `### Changed`, `### Fixed` (and `### Removed`/`### Deprecated` when needed). Write one line per change, in English, and name types and members in backticks.
- Mark breaking changes explicitly with **Breaking:** and say what users must change.
- Changes that only touch tests or CI need no entry. Refactors need one if they add public API, such as new interfaces or overloads.

## Version bump

- SemVer: patch for fixes, minor for new backward-compatible API, major for breaking changes.
- Update every place that holds the version, in one commit:
  - `package.json`
  - `CHANGELOG.md` once it exists

## Core dependency

- `package.json` pins the core (`com.rwth.unity.omilaxr`). Do not raise it on routine version bumps.
- Raise it when this package starts using core API that is newer than the pin, or when the core had an important or breaking change. Also update README lines that mention the core version.

## Unity package rules

- Every new file and folder needs a committed `.meta`. Let Unity create it (open `OmiLAXR_Dev`), or copy a neighbor's `.meta` with a fresh random 32-hex `guid`. Never delete or regenerate existing `.meta` files: prefabs, scenes and asmdefs reference their GUIDs. Folders ending in `~` (`Samples~`) have no `.meta`.
- The minimum Unity version is `2020.3` (`package.json` `unity`), but development happens on Unity 6. Put version-specific APIs behind `#if UNITY_2023_1_OR_NEWER` (or similar).
- Serialized field names are part of the scene and prefab format. Rename them only with `[FormerlySerializedAs]`.
- New components get `[AddComponentMenu("OmiLAXR / …")]`. Copy the path from a component of the same kind.
- License: MIT (`LICENSE`), because the vendored HypeRate SDK is MIT. There is no SPDX header convention here, and `package.json` has no `license` field.
- Add or adjust tests in `Tests/` with every behaviour change. Name test files `<Subject>_Tests.cs`, name methods `Subject_Does_Something`, and use `// Arrange / Act / Assert` comments.

## HypeRate-specific rules

- `Runtime/SDK` is an adapted copy of the HypeRate Unity SDK, kept in the namespace `OmiLAXR.HypeRate.SDK` so it does not clash with a separately installed SDK. Keep it `internal`; tests reach it through `InternalsVisibleTo` (`Runtime/AssemblyInfo.cs`).
- Tests must not open real WebSockets. Inject a fake `IHeartRateSocket` through `HypeRateProvider.Socket`.
