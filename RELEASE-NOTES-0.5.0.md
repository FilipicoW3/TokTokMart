# TokTokMart Demo 0.5.0

- Fixed the opening puddle and incorrect spill removal when mopping.
- Added rush, restock, cleanup and chaos shift profiles with bounded progression.
- Replaced long perfect-score explanations with short briefings and clear results.
- Added button hover/press/release motion and sequential star pops with bursts and sound.
- Added optional mobile vibration in Settings > Gameplay.
- Added the supplied cat, TokTok girl and two pocong customers, including supplied idle/walk animations where available.
- Installed the supplied main-menu signs and signpost from Assets 12-15.

Windows: extract the complete ZIP, then run `TokTokMart.exe`. Keep the data folder and runtime files with the EXE.

Android: install the APK on an Android 8.0+ ARM64 device. This demo uses the existing debug signing certificate, for direct device testing.

Validation: 147 gameplay, Canvas and touch checks passed. Five complete shifts passed with three stars. The built Windows game passed its 74-check suite at 1600x900 and 1560x720. Both builds completed with zero build errors. APK signature, upgrade certificate, vibration permission, archive integrity and 16 KiB alignment passed. Evidence is under `Verification/Builds/0.5.0`. No physical Android device was connected, so vibration feel still needs a phone check.
