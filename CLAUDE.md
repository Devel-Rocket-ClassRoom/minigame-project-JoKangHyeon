# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Contact Contract** is a Unity 6 (6000.3.15f1) dice-based roguelike — Yahtzee-style hand evaluation wrapped in a Balatro-style run economy. The player rolls dice, locks them into hand slots (족보) to score, clears score goals to advance, and spends coins in a shop on cards and relics that bend the rules. The run is endless: it continues until the player fails a score goal.

Current build: **v0.2.0** (see `git tag`).

## Development Environment

This is a Unity project. There are no CLI build/test commands — all development is done through the Unity Editor. Open `Assets/_Project/` as the working directory within Unity.

- ScriptableObjects are created via **Assets > Create** menu items registered with `[CreateAssetMenu]`.
- Asset data lives in `Assets/_Project/Data/`.
- Localization strings live in `Assets/StreamingAssets/Localization/ko_kr.csv` and are read through `StringTable` (default language `ko_kr`).
  - **DO NOT edit `ko_kr.csv` locally.** It is synced from an external web spreadsheet, so any local change is overwritten on the next sync. If a string needs adding/changing/removing, **do not modify the file — tell the user it must be changed in the source spreadsheet** and state exactly which keys/rows.
- Inspector-authored polymorphic lists use `[SerializeReference]` + a custom `[SerializeReferenceDropdown]` attribute, so concrete subclasses (Hand/Dice/Card/Relic) can be picked per element.
- The design doc (기획서) is the project's single source of truth (SSOT) but is **maintained outside this repository**. Code comments reference its sections (e.g. `§3.1`, `§2.8.3.0`) — those numbers point to that external doc and cannot be resolved from anything in this repo, so don't search for them here.

## Run / Round / Cycle terminology

Three nested gameplay units, used consistently across code, design doc, and issues:

- **Run** (`RunState`) — one full playthrough/slot. Holds the player's dice, hands, relics, cards, consumables, coin, level, and score.
- **Round** (`RoundState`) — one score goal. Clones the Run's dice and hands so per-round mutations don't leak. Ends when every hand slot is used; clears if `currentScore >= demoScoreCut[level]`, otherwise game over.
- **Cycle** (`CycleState`) — one scoring action (rolling, rerolling, then locking dice into a single slot). Ends when a slot is scored.

## Architecture

### State machine
`Assets/_Project/Script/Managers/GameState/`

`RunState` → `RoundState` → `CycleState`, plus `ShopState` between rounds. `GameManager` (`Managers/GameManager.cs`) is the MonoBehaviour entry point: it holds all the definition SOs, wires UI/canvases, drives `Update()` input (dice raycast, reroll, debug cheats), and owns `currentRun`. `RestartGame()` builds a fresh `RunState`, calls `Setup(starting)`, subscribes core handlers, and starts the first round.

- `RunState.Setup` resolves starting dice/hands/relics **by name** from the definition SOs.
- `RoundState.Init` clones Run dice/hands into the round, then starts the first cycle.
- `CycleState` manages `dicesRemain` (roll pool) and `dicesSetted` (locked into the slot, **hard cap 5** — see `SetDice`). Rerolls are limited by `RunState.rerollPerCycle`.

### EventBus
`Assets/_Project/Script/Managers/EventBus.cs`

Static, priority-ordered pub/sub — the backbone of card/relic effects. Higher `priority` fires first; relic priority constants live in `Relic.cs` (`c_priorityFirstRollEffect`, `c_priorityBrokenRune`, `c_priorityGoldenMirror`, `c_priorityDefault`). Subscribe/Unsubscribe/Clear made during a publish are deferred until the publish stack unwinds (`executionDepth`). `EventType` values: `OnCycleStart`, `OnFirstRollComplete`, `OnRollComplete`, `OnSlotScored`, `OnSlotScoreFixed`, `OnFirstScoreOfRound`, `OnRoundStart`, `OnRoundClear`, `OnCycleEnd`, `OnGameOver`. `EventBus.Clear()` is called on restart.

### Foundation (abstract contracts)
`Assets/_Project/Script/ParentClass/`

- **`Hand.cs`** — Abstract base. `GetDiceScore`/`GetEffectiveDices` are abstract; scoring runs over the *effective* dice subset. Carries `baseScoreMultiplier` and a `slot` back-reference; `ScoreMultiplier = baseScoreMultiplier * slot.slotLevel`. `GetResult()` returns a `HandResult` struct (dices, effectiveDices, baseScore, isAchived). `Clone()`/`CloneInstance()` is the per-round duplication pattern.
- **`Dice.cs`** — Abstract `Dice` + concrete `NormalDice` (1–6). A `Dice` owns a `List<DiceFace>` and a `DiceObject prefab`. `DiceFace` supports per-cycle value overrides (`OverrideValue`/`valueOverriden`, reset by `ResetForCycle`) and `OnRolled`/`OnSelected` callbacks — this is how cards hook face rolls. `ForceSetDice` can create a temp face for out-of-range values.
- **`Relic.cs`** — Abstract; `OnObtain(GameManager)` / `OnRemove()`. Relics generally subscribe to EventBus in `OnObtain`. Carries rarity, cost, `RelicCategory`, sprite, flavor text, and the priority constants.
- **`Card.cs`** — Abstract; `OnObtain`, optional `OnCycleStart`/`OnRoundStart`, `OnDisplay(RunState)` (shop-display-time init, e.g. random target), and `CanBuy(RunState)` (grays out + blocks purchase). Names/descriptions are localization keys resolved via `StringTable`.
- **`Consumable.cs`** — Abstract `OnUse`/`OnAdd`/`OnRemove`. Inventory exists on `RunState`; shop generation is still a TODO.
- **`Starting.cs`** — A run preset: `name`, lists of starting hand/dice/relic **names**.

### Enums
`Assets/_Project/Script/Defines.cs` also holds tuning constants (`c_startingCoin`, `c_levelPerGroup`, etc.) and theme colors.

- **`HandType`**: None, Numbers, Choice, FullHouse, SmallStraight, BigStraight, SmallAlignment, LargeAlignment
- **`Rarity`**: Common, Rare, Epic
- **`RelicCategory`**: DiceManipulation, ScoreBoost, Resource

### Hands (concrete)
`Assets/_Project/Script/Hands/Hands.cs` — `NumbersHand`, `ChoiceHand`, `FullHouseHand`, `SmallAlignmentHand` (4-of-a-kind), `BigAlignmentHand` (5-of-a-kind), `SmallStraightHand` (4 consecutive), `BigStraightHand` (5 consecutive). All score by summing their effective dice. `HandSlot` (`Hands/HandSlot.cs`) wraps a `Hand` with `slotLevel` and `currentScore`; slot level multiplies score and is what upgrade cards raise.

### Cards (concrete)
`Assets/_Project/Script/Cards/Cards.cs` — tarot-themed, named by suit/number (e.g. `CardP1` 펜타클 에이스, `CardW4` 완드 4, `CardM4` IV 황제). Patterns: dice-face mutation (`SetFace`/`SetFaceValue`), per-round-limited resource hooks via `OnRoundStart` + `DiceFace.OnRolled`, slot upgrades (Lv 1→2→3), and slot add/replace. Random targets are chosen in `OnDisplay`. Cards are always `Clone()`d before display/obtain to avoid mutating SO originals.

### Relics (concrete)
`Assets/_Project/Script/Relics/` — one file per relic: `BrokenChain`, `CrackedRune`, `CrystalOrb`, `FortuneTellersFinger`, `GoldenMirror`, `StarChalice`, `ThreadOfFate`, `WatchGlass`, plus `AlchemistsTouchstone` / `ObsidianScales` (stub) in `Relics.cs`. Most subscribe to an `EventType` in `OnObtain` and mutate score/coin/dice. Policy: **relics are never removed mid-run** — they reset only via fresh `RunState` on restart, so several `OnRemove` bodies are intentionally empty.

### ScriptableObject configuration
`Assets/_Project/Script/SO/` — registries, each with a `Find(name)` lookup: `HandDefinitionSO` (uses `HandData` wrapper subclasses to author hands in-inspector), `DiceDefinitionSO`, `RelicDefinitionSO`, `CardDefinitionSO`, `StartingDefinitionSO`, and `ShopRarityDefinitionSO` (rarity weights). Assets live in `Assets/_Project/Data/`.

### Shop
`ShopState` rolls a weighted set of cards/relics by rarity (`raritySpread`), skipping items the player already owns and falling back across rarities when a tier is exhausted. Reroll cost scales with `rerollCount` and run level (`GetRerollCost`/`GetMultiplier`).

### Dice physics
`Assets/_Project/Script/DicePhysics/` — `DiceObject` is the 3D rigidbody die (only dice are 3D; board/cards/UI are 2D). `RollManager.DeterministicRoll` runs scripted physics simulations to pre-resolve the landing face so the visible roll matches the already-decided logical value.

### UI / Canvas
`Assets/_Project/Script/Canvas/` — `ShopCanvas`, `DiceSelectCanvas`, `FaceSelectCanvas`, `HandSelectCanvas` and their UIObjects, driven through `GameManager.Start*Select` callback methods. `Tooltip` (`Common/`) handles dice/hand/card/relic hover info. `SlotUI` lives under `Script/Test/` (still the working slot view).

### Other
`Utility/StringTable.cs` (CSV localization, CsvHelper), `Utility/MouseSpinDice.cs` + `FallowObject.cs` (dice interaction/follow), `SaveData/SaveData.cs` (save schema scaffolding — versioned, not yet wired).

## Conventions

- **Name-based wiring**: starting setups and SO lookups resolve by `name` string, not direct references.
- **Clone before mutate**: hands/dice/cards are cloned when entering a round or shop slot so SO/run originals stay clean.
- **5-dice slot cap**: `dicesSetted` never exceeds 5 regardless of how many dice the player owns; only the roll pool grows.
- **Effects via EventBus**: prefer subscribing to an `EventType` with an appropriate priority over hard-coding into the state machine.

## Notable TODOs / gaps

- Consumable shop generation (`ShopState.Init` ends with a `//TODO`) and consumable use flow.
- Unused-hand → coin conversion at round clear (`RoundState.RoundEnd` TODO).
- `ObsidianScales` relic is an unimplemented stub (throws `NotImplementedException`).
- `SaveData` is schema-only; no serialization/load path yet.
- Debug input lives in `GameManager.Update` (`R` reroll, `F` Fortune Teller's Finger, `T`/`C` score/coin cheats) — to be relocated/removed before release.
