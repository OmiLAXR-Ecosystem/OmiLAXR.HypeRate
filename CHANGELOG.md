# Changelog

## [1.0.5] - 2026-11-09

### Added
- `IHeartRateSocket` interface for the connection that delivers heart rate updates.
- `HypeRateProvider.Socket` property to replace the HypeRate websocket, e.g. in tests. It defaults to the bundled HypeRate SDK.

### Changed
- Updated package version metadata.

---

## [1.0.4] - 2025-06-04

### Changed
- Lowered the minimum Unity version to 2020.3.
- Replaced the `.meta` files of the bundled SDK scripts.

### Removed
- `Learner Pipeline (HypeRate)` and `OmiLAXR.HypeRate` prefabs from `Resources/Prefabs`.
- The sample scene of the HypeRate SDK.

---

## [1.0.3] - 2025-06-04

### Fixed
- Namespace conflicts when the HypeRate SDK is already in the project. The bundled SDK now lives in `OmiLAXR.HypeRate.SDK` and its types are `internal`.

---

## [1.0.2] - 2024-12-02

### Added
- `LICENSE` file.

### Fixed
- `HypeRateProvider` no longer throws on application quit when no socket was opened.

### Removed
- Dependency on `com.unity.nuget.newtonsoft-json`.

---

## [1.0.1] - 2024-11-11

### Changed
- Moved the prefabs to `Resources/Prefabs`.
- Updated the README.

---

## [1.0.0] - 2024-11-05

First release of OmiLAXR.HypeRate.

### Added
- `HypeRateProvider`, a `HeartRateProvider` that reads the heart rate from the HypeRate websocket.
- Bundled and adjusted source of the HypeRate Unity SDK.
- `Learner Pipeline (HypeRate)` and `OmiLAXR.HypeRate` prefabs.
