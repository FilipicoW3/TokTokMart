# Gameplay refresh verification — 0.5.1

Unity 6000.3.9f1, Windows x64 and Android ARM64. Existing scenes, supplied artwork, friend UI assets and save keys are retained.

## Spill reproduction and repair

Before the change, repeatedly running the goblin encounter produced seven puddles with three overlapping pairs (`before-spill-repro.txt`). The spawn index repeated the same three positions. Removing one layer left another visible puddle.

All spill creation now checks spacing, the floor and recently cleaned patches. Mopping captures the actual hazard and its position rather than relying on a mutable list index. Four successful strokes clear every layer of that patch, award one cleanup and retire the corresponding visuals immediately. Other puddles remain. The template stays hidden and retry restores the authored hazards.

## Checks

| Suite | Passed | Evidence |
| --- | ---: | --- |
| Spill, progression and active bosses | 131 | `Final/checks.txt` |
| Existing release regressions | 74 | `Regression/regression.txt` |
| Scoring, save preservation and routes | 57 | `Scoring/verification.txt` |
| Mobile input and supplied artwork | 60 | `Mobile/results.txt` |
| Authored Canvas and native controls | 13 | `Canvas/canvas-verification.txt` |

All 335 editor assertions passed. The legacy scoring capture was updated to exercise the current goals HUD and short retry feedback; the 25-point deduction is asserted directly.

`Playthroughs/boss-playthroughs.txt` records successful navigation and real interaction inputs for all four bosses: six Ray punches, three circuits, manual rear shots and three returned cans. Bosses cannot resolve through their old automatic timers.

`Shifts/shift-playthroughs.txt` records complete shifts 1, 5, 10 and 20 with real-time customers, partner AI, chaos, task timing and closing deadlines. The input replay uses normal navigation and finishes any mop task before collecting/delivering stock. Required task totals increase from 5 to 9, 14 and 24. Each shift passed.

Packaged Windows captures and gameplay reports are in `Standalone-Desktop` (1600×900) and `Standalone-Phone` (1740×820). Both passed all 131 gameplay checks with exit code 0. The phone-sized run is a desktop aspect-ratio/input check.

The Android APK built with zero errors: package `com.toktokmart.demo`, version 0.5.1/code 6, API 26–35, ARM64. Signature verification and 16 KB alignment checks passed; its signer matches 0.5.0 and VIBRATE permission is present. Actual APK size is 52,004,071 bytes. The build report's larger total includes intermediate/debug output that is excluded from the release. Windows ZIP size is 64,207,871 bytes, with debug-only folders excluded. Package reports and hashes are in `../Builds/0.5.1` and the release checksum file.

No Android device was connected. Physical Android gameplay and vibration testing remain pending.
