# FallenFate Documentation

Table of contents for all project documentation files.

---

## Guides & Reference
| Document | Description |
|----------|-------------|
| [Unity Package Resolution Guide](2025-02-17_unity-package-resolution-guide.md) | How to prevent and fix Unity package version mismatch errors (Tilemaps, URP, etc.) |

## Code Reviews
| Document | Description |
|----------|-------------|
| [PR #415 -- Room2 Addons Review](CODE_REVIEW_PR415_Room2_Addons.md) | Code review for Room 2 colliders and addons |
| [PR #415 -- GitHub Comment](CODE_REVIEW_PR415_GITHUB_COMMENT.md) | GitHub comment for PR #415 |

## Audits
| Document | Description |
|----------|-------------|
| [2026-03-20 -- Sound System Compliance Audit](2026-03-20_sound-system-audit.md) | Full project scan of every .cs script for non-compliant sound playback. Identifies scripts using raw AudioClip/AudioSource/PlayOneShot instead of SoundFXManager + SoundDefinition, with lines to change and developer attribution. |

## Change Logs
| Document | Description |
|----------|-------------|
| [2026-03-12 -- Compile Error Library Cleanup](2026-03-12_compile-error-library-cleanup.md) | Removed editor/platform-only libraries from runtime scripts and replaced editor-only scene references with runtime-safe scene name checks |
| [2026-03-12 -- Grabber Screen Shake PR #464 Conflict Resolution](2026-03-12_grabber-screen-shake-pr-464-conflict-resolution.md) | Merge-conflict resolution notes for PR #464 (`Nathan'sEnemies-Week-9` into `main`) |
| [2026-02-24 -- Act1 Cutscene, Dialogue, and Tools](2026-02-24_act1-cutscene-dialogue-and-tools.md) | Consolidated changelog for scene, dialogue, transition, and sandbox utility updates |
| [2026-02-24 -- Jose's Dialogue System Fixes](2026-02-24_joses-dialogue-system-fixes.md) | Dialogue progression and conversation selection stability fixes for NPC and trigger dialogue flow |
| [PR #417 -- Combat Merge Changelog](MERGE_PR417_COMBAT_CHANGELOG.md) | Changelog for combat system merge |
| [PR #417 -- Combat Feedback](CODE_FEEDBACK_PR417_Combat_Feedback.md) | Code feedback on combat system changes |
