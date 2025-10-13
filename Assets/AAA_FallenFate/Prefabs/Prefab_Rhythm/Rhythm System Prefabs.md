# Rhythm System Prefabs

| Prefab               | Status     | Description                                                                                                                         |
| -------------------- | ---------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| MusicManager         | Working    | The main manager object controlling the music system in its entirety, from lookups to playback.                                     |
| MusicStaff           | Working    | The music staff UI itself. Still somewhat of a prototype but in working condition for including into the rest of the game codebase. |
| MusicStaffBeatMarker | Internal   | UI markers to denote when to hit a beat.                                                                                            |
| RhythmBonusJudge     | Working    | Empty object that controls the general rhythm that returns whether or not a beat was hit and how well.                              |
| RhythmTester         | Testing    | Empty object containing a script to control the display of variables, demonstrating the rhythm system's intended usage.             |
| UIPulse              | Deprecated | UI to demonstrate when to hit a beat. Deprecated due to newer requirements.                                                         |
