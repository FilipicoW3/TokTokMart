# PR #1 integration

Incoming PR: https://github.com/MAYKELL07/TokTokMart/pull/1

Friend's head: `a2716b5fcd076688a07678da067825ee15d7e1f4`.
Retained release: `a8a4ec11edb1138476062a17e4bb67dc23b3315f` (0.5.0).

The PR includes stock/timer icons, TextMesh Pro resources, and restored copies of older scripts/tools. It does not include a changed scene or Canvas prefab. The current PR contents were requested for integration.

The four older script copies are identical to their pre-rename versions and share GUIDs with the current canonical files. They were consolidated into CanvasUIBinding, GameArtLibrary, CampaignScoring and MainMenuAndHudUI. Fourteen identical historical utility copies remain consolidated under Tools/Legacy. The incoming assets and their metadata are retained.

The stock icon now appears in the brief, restock HUD and task progress display; the timer icon appears beside closing time. Existing native labels accept TextMeshProUGUI as well as Unity UI Text, preserve the assigned font, and respect Bind Game Data. Normal play still uses authored objects only.

Preservation audit: 21 core files match 0.5.0 exactly. All 3,838 existing Canvas binding keys and rectangles are retained, with two new authored bindings. There are no duplicate asset GUIDs.

Verification: ten TMP/resource checks, 74 release regressions, 13 Canvas checks and 60 touch checks pass. The Windows validation build succeeds with zero build errors, and its standalone 74-check suite passes. Reports and captures are stored in this folder.
