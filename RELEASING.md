# RNGenie Release Guide

## Versioning & Tags
- **Core:** `core-vMAJOR.MINOR.PATCH[-prerelease]` (e.g. `core-v0.2.0`, `core-v0.2.0-alpha.3`)
- **Distributions:** `dist-vMAJOR.MINOR.PATCH[-prerelease]` (e.g. `dist-v0.1.0`)
- CI packs on **tag pushes** only. Normal pushes/PRs do not pack.

## Prerequisites
- Branch protection green on `master` (Style + Build & Test).
- `NUGET_API_KEY` secret exists (only needed when we enable publish).
- Each packable project has `icon.png` and correct NuGet metadata.
- CI workflow: `.github/workflows/dotnet.yml` (packs on `core-v*`/`dist-v*`).

---

## A) Pre-release (optional, test the pipeline)
1. Merge your work to `master`.
2. Create a **pre-release tag** from the GitHub web UI:
   - **Releases → Draft a new release**
   - Tag: `core-v0.2.0-alpha.1` (or `dist-v…`)
   - Title: `RNGenie.Core 0.2.0-alpha.1`
   - Mark as **Pre-release** (checkbox).
   - Publish.
3. CI will:
   - Run style/build/tests.
   - **Pack** `.nupkg`, upload as **Actions artifact**, and attach to the **Release → Assets**.
4. Download the `.nupkg` from the Release page and sanity-check locally:
   ```bash
   dotnet tool install --global nugettool # or use nuget CLI
   # Inspect package metadata
   ```

---

## B) Stable Release
1. Ensure `master` is green.
2. Create a tag via **Releases → Draft a new release**:
   - Tag: `core-v.0.2.0` (or `dist-v0.1.0`).
   - Title and notes (highlights, breaking changes).
   - Publish.
3. CI will produce and attach the .nupkg to the Release.
4. Once publishing is enabled, CI will also push to NuGet automatically.

---

## C) Hotfix Release
1. Branch from the last stable tag's commit (or `master` if appropriate).
2. Fix → PR → merge to `master`.
3. Tag: `core-vX.Y.Z+1` (e.g. `core-v0.2.1`).
4. Follow **Stable Release** steps.

---

## D) Post-release housekeeping
- Verify the Release **Assets** contain the expected `.nupkg`.
- (If publishing enabled) Confirm on nuget.org (package page shows new version).
- Create/Update `CHANGELOG.md` (optional but recommended).
- Bump `VersionPrefix` in `.csproj` **only if** you use it for alpha CI builds.

---

## E) Troubleshooting
- **Pack didn't run**: check the tag prefix exactly matches `core-v*` or `dist-v*`.
- **Icon/README missing in NuGet page**: ensure `PackageIcon`/`PackageReadmeFile` are set and the files are packed (`Pack = "true" PackagePath=""`).
- **Branch protection blocks merge**: resolve conversations, ensure required checks pass.
- **Need to re-use a version**: you can re-upload assets to the GitHub Release, but NuGet won't allow overwriting-use a new version.

---