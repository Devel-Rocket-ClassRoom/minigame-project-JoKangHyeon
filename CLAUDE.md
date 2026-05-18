# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Contact Contract** is a Unity 6 (6000.3.15f1) dice-based hand evaluation game — think Yahtzee-style gameplay where players roll dice and form scoring combinations (hands). The game features configurable starting setups and relic power-ups.

## Development Environment

This is a Unity project. There are no CLI build/test commands — all development is done through the Unity Editor. Open `Assets/_Project/` as the working directory within Unity.

- ScriptableObjects are created via **Assets > Create** menu items registered with `[CreateAssetMenu]`
- Asset data lives in `Assets/_Project/Data/`

## Architecture

The codebase follows a layered design:

### Layer 1: Foundation (Abstract Contracts)
`Assets/_Project/Script/ParentClass/`

- **`Hand.cs`** — Abstract base for all hand types. `GetDiceScore(List<int>)` is abstract (subclasses implement scoring). `IsAchived(List<int>)` checks if the hand pattern is matched. `SetDice` + `GetCurrentHandScore` manage bound dice state.
- **`Dice.cs`** — Abstract base for dice. `RollDice()` is abstract; `GetDice()`/`SetDice(int)`/`ResetDice()` manage state.
- **`Relic.cs`** — Data-only struct: `name`, `description`. Trigger logic not yet implemented.
- **`Starting.cs`** — Composes a game variant: `startingHands`, `startingDices`, `startingRelics`.

### Layer 2: Enum Definitions
`Assets/_Project/Script/Defines.cs`

- **`HandType`**: None, Numbers, Choice, FullHouse, SmallStraight, BigStraight, SmallAlignment, LargeAlignment
- **`RelicTiming`**: OnRoll (only value; used for future relic triggering logic)

### Layer 3: Concrete Hand Implementations
`Assets/_Project/Script/Hands/Hands.cs`

Eight hand types, all inheriting from `Hand`:

| Class | Pattern | Score |
|---|---|---|
| `NumbersHand` | Count of `numTarget` face | `numTarget × count` |
| `ChoiceHand` | Any dice | Sum of all |
| `FullHouseHand` | 3-of-a-kind + pair | `(v×3) + (v×2)` |
| `SmallAlignmentHand` | 4-of-a-kind | `value × 4` |
| `BigAlignmentHand` | 5-of-a-kind | `value × 5` |
| `SmallStraightHand` | 4 consecutive | Sum of sequence |
| `BigStraightHand` | 5 consecutive | Sum of sequence |

### Layer 4: ScriptableObject Configuration
`Assets/_Project/Script/SO/`

- **`HandDefinitionSO`** — Registry of `List<Hand>` available in a run. Menu: `HandDefinition`.
- **`StartingDefinitionSO`** — List of `Starting` game presets. Menu: `StartingDefinition`.

## Key Gaps (Not Yet Implemented)

- **Concrete `Dice` class** — Only the abstract exists; needs a `StandardDice` (or similar) with `RollDice()` and lock/reroll state.
- **GameManager** — No orchestration: no turn flow, dice rolling pipeline, hand evaluation trigger, or relic activation.
- **Relic effect system** — `RelicTiming.OnRoll` is defined but nothing invokes relics.

## Known Issues

- `SmallAlighmentHand` class name has a typo (should be `SmallAlignmentHand`).
- `NumbersHand.numTarget` needs `public` or `[SerializeField]` for Unity serialization.
- `FullHouseHand` pair detection logic (line ~82) may have an off-by-one — verify with test cases.
