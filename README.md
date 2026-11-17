# TokTokMart demo 0.5.1

Unity 6.3 LTS **6000.3.9f1**. Open `Assets/GokilKisah/Scenes/TokMartPitch.unity` and press Play.

Prebuilt Android APK and Windows x64 ZIP: [GitHub release 0.5.1](https://github.com/MAYKELL07/TokTokMart/releases/tag/v0.5.1-demo). Extract the Windows ZIP before running `TokTokMart.exe`.

This update clears stacked puddles, gives Uncle Ray a real goblin fight, adds active mechanics to every boss, and makes later shifts longer with more required tasks and less customer patience. It retains the supplied customer and menu artwork, simple scoring screens, button motion, star bursts and mobile vibration.

## November 17 gameplay update (0.5.1)

- A completed mop minigame clears the captured patch even if spill indexes change. Stacked puddles clear together; goblin and customer mess cannot immediately reappear on freshly cleaned ground. The reusable spill template stays hidden.
- Goblins roam and make mess. Call Ray, stun goblins with Interact and let him chase and punch them. K-Kung needs three circuits restored, Salmah must be dodged and shot from behind, and Bo needs three cans returned. Ground warnings and hit reactions show the consequences.
- Every level adds one task and a longer closing deadline. Customer patience starts at 42 seconds and falls toward a 16-second minimum. The partner covers checkout during boss fights, while neglected queues and spills build chaos.
- Friend's PR #1 artwork and TMP compatibility remain integrated. Existing unlocks, scores, art and authored UI layouts are retained.

## November 17 update (0.5.0)

- Cleaning removes the selected spill and its matching visual together. The opening puddle stays gone; retries restore original hazards.
- Rush shifts bring faster arrivals and a shorter clock; restock shifts start with low shelves; cleanup shifts supply more puddles; chaos shifts add pressure and returning encounters. Every fifth shift retains its milestone boss. Goals grow in bounded tiers.
- Briefings show the focus, three goals and closing time. Results show stars, score and one short tip. The long perfect-score guide and repeated point tables have been removed.
- Buttons lift on hover, compress when pressed and pop on release. Earned result stars pop in sequence with colored bursts and sound. Controls and effects remain authored in the editable Canvas.
- Settings > Gameplay > Vibration switches mobile feedback on or off. Android uses short pulses for taps, tasks, misses and results; unsupported hardware is handled quietly.
- Supplied Girl, Zomboy and Zomgirl idle/walk sheets are used for customers. The cat uses its supplied static art with a small movement bob. Source hashes are recorded in `Verification/Release05/art-import.json`.

## Collaborating with a friend

### First setup

1. The repository owner invites your GitHub username in **Settings > Collaborators**; accept the invitation. Without write access, fork the repository and open pull requests from your fork.
2. Install Git (or GitHub Desktop), Unity Hub and **Unity 6000.3.9f1**. Add Android Build Support only if building APKs. Match the version in `ProjectSettings/ProjectVersion.txt`; do not upgrade the project independently.
3. Clone `https://github.com/MAYKELL07/TokTokMart.git` into your own local folder. Avoid a shared OneDrive/Dropbox folder for the working copy.
4. In Unity Hub choose **Add project from disk**, select the clone and let packages import. Open `Assets/GokilKisah/Scenes/TokMartPitch.unity`, then press Play.
5. In Editor Settings use **Visible Meta Files** and **Force Text** serialization. Commit each asset together with its `.meta` file. Rename/move assets in Unity's Project window to preserve GUID references.

### Daily workflow

Use one branch per change. In GitHub Desktop: fetch/pull main, create a branch, edit, review the diff, commit, push, then open a pull request. Equivalent commands:

```sh
git switch main
git pull --ff-only
git switch -c feature/your-change
# Edit in Unity, stop Play Mode and save scenes/prefabs.
git status
git add <specific-files-and-their-meta-files>
git commit -m "Describe the change"
git push -u origin feature/your-change
```

Ask the other person to review the pull request before merging. After merge, switch to main and pull again. Never force-push main. If pull reports local changes, commit them on your feature branch or stash them before switching; do not discard work to resolve it.

Agree on ownership before editing: one person per scene or prefab at a time. Prefer editing separate prefabs instead of both editing the main scene. If a YAML scene/prefab conflict occurs, stop and resolve it together in Unity; do not blindly choose one whole file. UnityYAMLMerge (Smart Merge) is bundled with the editor and can be configured as a Git merge tool.

Commit `Assets/`, `Packages/`, `ProjectSettings/` and relevant documentation. Keep `Library/`, `Temp/`, logs, local toolchains, signing keys and generated builds out of Git. Share APKs and the complete Windows ZIP through GitHub Releases. Keep source-art credits and licenses when adding assets.

### UI work without rebuilding screens in code

Use the saved Canvas in `TokMartPitch.unity` or edit `Assets/GokilKisah/Resources/TokMartEditableUI.prefab`. See [the UI editing guide](UI-ADJUSTMENTS.md). Existing screens and controls must be authored before play: the game no longer creates missing UI objects or components during normal play. Scripts update data and actions; designers own the saved hierarchy, fonts, artwork and sibling order.

Before committing, test the main menu, level selection, character selection, shift briefing, settings/sliders, a task and pause/resume. For UI changes also check a wide desktop aspect ratio and a phone landscape aspect ratio. Attach screenshots to the PR and say what you tested. Android changes still need a real phone check before release.

### Code and tool map

| File or folder | Responsibility |
| --- | --- |
| `StoreGameController.cs` | Core controller and shared gameplay state; related systems are partial files |
| `MainMenuAndHudUI.cs` | Menu and HUD state/data bindings |
| `CampaignScoring.cs`, `ShiftScore.cs` | Campaign results, scoring and star rules |
| `CanvasUIBinding.cs` | Connects gameplay to the authored Canvas |
| `TokMartCanvas.cs`, `TokMartCanvasElement.cs` | Canvas element lookup, game-data and interaction bindings |
| `GameArtLibrary.cs` | Runtime art lookup/cache; resource folder names stay stable |
| `Editor/` | Editor-only import, build and migration tools |
| `Tools/Legacy/` | Historical one-off source patches; do not run on the current project |
| `Verification/` | Recorded checks and captures; not game content |

Use names describing responsibility rather than dates. Preserve hidden UI binding keys and asset GUIDs when refactoring. A new gameplay button needs both an authored control and an explicit code binding. Do not run the historical Canvas migration to apply normal visual edits: it can overwrite hand-authored work.

## November 11 update (0.4.0)

- New Bigbesty typeface and the supplied task cards, star, touch controls, counter and storage-box artwork.
- Main-menu level selection with locked shifts, saved best scores and up to three stars per shift.
- Fixed objective points plus visible accuracy, speed and chaos bonuses. The briefing, scoring guide and results show the same thresholds. Money is separate from the star score.
- Customer patience bars, task deadlines, countdown warnings and explicit feedback for missed timing and wrong receipt totals.
- Editable Canvas screens and native buttons/sliders, with the existing gameplay bindings retained.
- Returning bosses respect the selected shift's milestone difficulty when replaying older levels.

See [the design and scoring notes](GAME-DESIGN-NOVEMBER11.md) and [verification summary](Verification/November11/summary.txt). All 127 scoring, Canvas and touch checks passed. Full shifts 1 and 5 completed with perfect scores (1050/1050 and 1900/1900).

## Android demo

The installable demo targets **Android 8.0+ (API 26), ARM64, landscape**. It uses a local demo/debug signature and is intended for direct device testing, rather than store submission. Build output: `Builds/Android/TokTokMart-Demo.apk`.

- Drag the left joystick to move.
- Hold Sprint while moving; tap Interact to pick up stock or begin a nearby task.
- Tap the task buttons/keypad to restock, clean and complete checkout.
- Tap Pause, then **Controls**, to change joystick size, action-button size, separate left/right padding and both control heights. The preview updates immediately. Tap **Back** to save. **Reset control layout** restores defaults.
- Desktop preview: WASD/arrows, Shift and E/Space. Enable touch controls in Settings > Gameplay.

Menus and sliders read Input System pointer/touch input directly. Gameplay tracks separate fingers for movement, held sprint and interaction. Pausing or losing focus clears held controls.

## Build

Install Android Build Support for the exact editor version, with OpenJDK 17 and Android NDK r27c. Use the Unity-bundled SDK: command-line tools 16.0, platform-tools 36.0.0, build-tools 36.0.0 and Android API 35. Configure these under Unity Preferences > External Tools.

Choose **Gokil Kisah > Build Android Demo APK**. This helper excludes unused AI inference shader variants from the demo; other builds retain the installed package behavior. Alternatively:

```powershell
& 'PATH_TO_UNITY/Editor/Unity.exe' -batchmode -quit -projectPath . -buildTarget Android -executeMethod GokilKisah.Editor.MobileDemoBuild.BatchBuild -logFile Builds/android-build.log
```

For a machine with separate toolchains, optionally create the git-ignored `Tools/android-toolchain.local.json` with `sdk`, `ndk` and `jdk` directory paths. Never commit signing keys or machine-specific credentials.

## Art and verification

All 44 supplied files are preserved under `Assets/GokilKisah/ArtSources/November2026`, including editable AI/FLA files, alternate frame rates and previews. The full supplied smooth sequences (49 Sam frames, 49 Zack frames, 41 Toyol frames) and mobile-sized UI are loaded at runtime. **Gokil Kisah > Import November Mobile Artwork** regenerates the runtime sprites without changing source art.

Source hashes, Play Mode checks and screenshots are in `Verification/MobileDemo`. Tests cover menu touch, sliders, simultaneous controls, pause/release, saved layout, loaded art and animation. The customer regression covers ten simulated minutes. A physical Android device test is still required; no phone was connected during development.

Additional art/audio/font credits: `Assets/GokilKisah/ASSET-CREDITS.md` and the included third-party license files. Supplied TokTokMart artwork retains its original ownership.

## Movement and shift goals

Gaits follow actual ground distance after collisions, with uninterrupted phase when switching between walking and sprinting. Idle uses the supplied standing artwork. Footstep contacts follow the same stride; blocked movement, pause and teleports produce no extra footsteps. The supplied walk cycles are retimed for sprinting; no separate run drawings were supplied.

Every shift opens with a short goal briefing before its timer starts. Each level adds one required task and a longer closing deadline. The partner covers checkout during required restocking and boss fights. Required cleanup work stays available in small batches. Customer patience starts at 42 seconds and decreases with the level, to a minimum of 16 seconds; neglected queues and spills also build chaos.

| Shift | Focus | Customers | Restocks | Cleanups | Time |
| --- | --- | ---: | ---: | ---: | ---: |
| 1 | Welcome | 3 | 1 | 1 | 2:52 |
| 2 | Lunch rush | 4 | 1 | 1 | 3:06 |
| 3 | Stock up | 3 | 3 | 1 | 3:30 |
| 4 | Mop up | 3 | 1 | 4 | 3:49 |
| 5 | Chaos + boss | 4 | 2 | 3 | 4:12 |
| 10 | Chaos + boss | 6 | 4 | 4 | 5:42 |
| 20 | Chaos + boss | 11 | 6 | 7 | 8:42 |

Complete all targets and the required boss to unlock the next shift. Existing unlocks and best scores are retained. Replaying older shifts uses the new balance.

Bosses require player actions rather than waiting for a timer:

- **Goblins:** call Uncle Ray, then use Interact near goblins to stun them. Ray chases and punches stunned goblins; each needs two hits. Roaming goblins leave separate puddles. A completed mop minigame clears every layer of its captured patch and briefly protects it from fresh mess.
- **K-Kung:** restore three circuits around the shop while dodging marked attacks. Each circuit relights more of the shop.
- **Miss Salmah:** collect the nail gun, bait her rush, dodge and move behind her. Tap Interact to fire during the opening.
- **Bo:** find and kick three cans back before their individual deadlines while avoiding marked throws.

See [the gameplay verification record](Verification/GameplayRefresh/summary.md) for the spill reproduction, boss playthroughs and longer-shift checks.
