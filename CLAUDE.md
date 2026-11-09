# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Working rules (versioning, CHANGELOG, `.meta` files, licenses, compatibility) are in @AGENTS.md. Follow them.

## What this is

`com.rwth.unity.omilaxr.hyperate`: Module that feeds live heart rate from the HypeRate service (phone or watch app → WebSocket) into OmiLAXR's heart-rate monitor.

## Commands

There is no command-line build or linter: the package compiles inside a Unity project. Paths below are relative to the workspace root (`..`).

- **Develop:** open `OmiLAXR_Dev/` in Unity 6000.3.13f1. Its `Packages/manifest.json` links `com.rwth.unity.omilaxr.hyperate` from disk (`file:`), so edits recompile live.
- **Tests** (`Tests/`, assembly `OmiLAXR.HypeRate.Tests.Runtime`): the test asmdef is a runtime assembly, so the tests are PlayMode-only. Run them in the Unity Test Runner (PlayMode tab).
- **CLI test run.** Batch mode fails while the editor has the project open. In that case, run against an APFS copy (`cp -c -R OmiLAXR_Dev <scratch>/Dev`).

  ```bash
  /Applications/Unity/Hub/Editor/6000.3.13f1/Unity.app/Contents/MacOS/Unity -batchmode \
    -projectPath OmiLAXR_Dev -runTests -testPlatform PlayMode \
    -assemblyNames OmiLAXR.HypeRate.Tests.Runtime -testResults results.xml
  # single test: replace -assemblyNames … with -testFilter OmiLAXR.HypeRate.Tests.<Class>.<Method>
  ```

## Architecture

- **`HypeRateProvider`** (`Runtime/HypeRateProvider.cs`) is a core `HeartRateProvider` (`OmiLAXR.Actors.HeartRate`). Attach it to a pipeline's `Heart Rate Monitor`. It connects, joins the device's heartbeat channel and forwards `HeartRateUpdated` values.
- **`IHeartRateSocket`** is the seam to the socket. The default is the SDK singleton `HypeRate.GetInstance()`, a `System.Net.WebSockets` client that speaks the Phoenix-channel protocol in `Runtime/SDK` (`Network`, `Channels`, data packages).
