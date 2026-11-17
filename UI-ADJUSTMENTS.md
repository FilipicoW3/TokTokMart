# Manually editing TokTokMart UI

## Quick start: UI Layout Editor

1. Open `Assets/GokilKisah/Scenes/TokMartPitch.unity` and stop Play mode.
2. Choose **TokTokMart > UI > Open UI Layout Editor**. The window stays open when you select other UI objects.
3. Click a screen preview, such as **Main Menu**, **Shift Briefing**, **Shift Results**, or **Credits**. Use the **Scene** tab to edit, or the **Game** tab to see its screen appearance without starting Play.
4. Expand **TokMartEditableUI > Design (1600 x 900)** and select the object or whole button group. Press **T** for the Rect tool. Drag to position/resize, or enter precise **Pos X / Pos Y / Width / Height** in its Rect Transform. Resize a composite button using the parent group's Scale so artwork, label and hit area move together.
5. Save with **Ctrl+S**. Press the Unity **Play** button to test. Stop Play before making permanent adjustments. The separate Windows demo is an older compiled copy; it will not reflect these editor changes.

The menu's existing solid fill now has a **Tok Mart Horizontal Fade** component. Its black alpha goes from **Left Opacity = 1** to **Right Opacity = 0** across the existing 630-pixel rectangle. Use **Select Fill - Position / Size / Color** in the layout editor to adjust its Rect Transform or Raw Image color. **Hide menu fill** removes it from previews and play. For any other decorative element or group, enable **Hide From Game** on its Tok Mart Canvas Element. The normal GameObject active checkbox is managed by gameplay and is not a persistent hide setting. You may delete decorative bindings permanently, but restoring them requires the archive and missing-binding warnings can result; hiding is reversible and keeps binding keys valid.

The layout editor includes **Credits text** fields for the heading, names/roles/sources, and footer. These edit the actual authored Text components, with game-data binding disabled for those three labels. Press **Preview Credits** to inspect the result and save the scene. Font, size, placement, and color remain editable in the normal Inspector. Existing attribution text is retained until you replace it.

Obsolete screen groups from older UI revisions are archived in `UI-Backups/Before-UI-Workspace-20261117`. This contains the full original prefab, the on-disk scene, and a separate snapshot including the previously unsaved editor changes. The active Canvas contains current screens. Existing authored placement and sizing are preserved. No Windows or Android player build is part of this update.

The attached shift-screen mockup is a visual reference only; the live briefing data and existing authored layout are retained for testing.

The active scene now uses the TokMartEditableUI Canvas prefab in Assets/GokilKisah/Resources. The original IMGUI drawing code is retained as a fallback; the editable Canvas is the default screen renderer. Existing gameplay actions remain connected through a binding layer. World-space shop labels stay with the world renderer.

## Edit a screen
1. Stop Play mode.
2. Select TokMartEditableUI in the Hierarchy. Scroll to the Tok Mart Canvas component and use its screen preview buttons.
3. Expand Design (1600 x 900), then the screen group. Groups are named for their screen, button label, text, or artwork.
4. Move an entire UiButton group to keep its artwork, label and hit area together. Move the Joystick, Sprint or Interact groups for touch controls.
5. Edit Text components for wording, font, size, alignment and color. Edit Image/RawImage components for artwork and color. Use RectTransforms for placement and dimensions; use group Scale to resize a composite control.
6. Save the scene. Use the prefab Overrides menu to apply changes to the shared prefab, or open the prefab directly to edit it.

Other screens are inactive until gameplay needs them. The preview buttons only control edit-mode visibility; gameplay chooses the active screen automatically. Edit outside Play mode so changes persist.

## Data bindings
Tok Mart Canvas Element keeps the connection to game data. Leave Bind Game Data enabled for counters, timers, receipt values and feedback. Static text and artwork can be edited directly. Disable Bind Game Data only when you intentionally want a fixed authored value. Leave Animate Layout enabled on progress bars, joystick knobs and task timing markers; disable it to freeze a visual at its authored location.

The gameplay controller controls visibility; the saved hierarchy controls sibling draw order. Move or resize existing elements without deleting their binding components, changing hidden keys, or reparenting bound elements. Add decorative objects freely within a screen group. New gameplay actions still need a code connection.

## Mobile sizing
Store Game Controller > Mobile UI sizing retains Mobile Hud Scale and Default Touch Scale. In-game Settings > Controls retains player control size/position sliders. Moving the authored mobile control groups moves their touch hit areas too.

## Recovery and validation
Store Game Controller > Editable Canvas UI > Use Original Imgui is an emergency visual fallback. Keep it off for the editable Canvas.
Original scripts and scene are backed up under UI-Backups. Verification/EditableCanvasFinal contains original-versus-Canvas captures and button, slider, character selection and mobile hit-area check results. This is a retained Canvas renderer with a compatibility binding pass, not a rewrite of gameplay or the world renderer.

## Authored-only runtime

Normal play does not create Canvas objects, controls or missing UI components. Assign the scene Canvas in Store Game Controller > Editable UI. Run **TokTokMart > UI > Validate and prepare authored scene** once for an older scene to save an EventSystem and check missing script references. A missing binding produces a warning instead of silently generating a new control.

Manually selected fonts are preserved. Existing static text, images, colors, positions and sibling order remain authored; live counters and animations continue through their binding components. Disable **Bind Game Data** for a fixed custom visual and **Animate Layout** for a fixed placement. Do not rename hidden binding keys. You can rename visible GameObject names and move a whole control group. Reparenting bound elements requires updating the binding design.

The compatibility presentation pass still supplies gameplay state, button actions and animated values. It is not a fully separate presenter architecture yet. World labels remain drawn by the world renderer. Historical screen generation is an explicit Editor migration only, not part of normal play or the design workflow.

## Button and star motion

Buttons animate their authored group through `TokMartButtonMotion`. Move the whole group when adjusting placement so the art, label and hit area remain together. Result stars use `TokMartStarBurst`; its delay controls the stagger, and its spark objects remain editable children of the star.

The release authoring helper captures and restores existing rectangles before saving. Animated layout values must not become the next authored baseline, or control sizes and positions will drift on later play sessions.

## TextMesh Pro labels

The existing bindings accept both Unity UI Text and TextMesh Pro labels. To convert a label, keep its GameObject, RectTransform and Tok Mart Canvas Element; replace its Text component with TextMeshProUGUI in the Editor. Choose the TMP font and turn off its Raycast Target for a decorative label. Counters, feedback colors and changing sizes keep their existing binding keys; the presenter preserves the assigned TMP font. Disable Bind Game Data for fixed designer text.

Commit the edited scene or Canvas prefab and its metadata with UI changes. Adding TMP fonts or restoring older script filenames alone does not include an edited screen. Canonical runtime files remain CanvasUIBinding, GameArtLibrary, CampaignScoring and MainMenuAndHudUI; historical utilities stay under Tools/Legacy.
