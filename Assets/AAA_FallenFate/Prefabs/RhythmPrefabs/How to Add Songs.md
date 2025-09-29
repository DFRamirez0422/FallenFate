# How to Add a Song for use with Rhythm

> Please read [this article](https://support.unity.com/hc/en-us/articles/206484803-What-are-the-supported-Audio-formats-in-Unity) for the list of supported formats:
> - .aif
> - .wav
> - .mp3
> - .ogg

To add new song entries into the rhythm subsystem to use for combat and other aspects of the game as needed:

- Add your song in a format that is playable in Unity into a folder as decided by the team.
- Select the `MusicManager` prefab game object and go to the `MusicDictionary` component in the inspector. This will make your song available to every instance.
- Click the plus icon under `Music Entries` to add a new entry:
	- `Name` your song into something easy to identify for everyone on the team.
	- Drag and drop the song file from Unity's file browser into the `Music Clip` field.
	- Set the proper `Tempo` specified in quarter beats per minute.
		- If you don't know the tempo, you can use [Audacity](https://www.audacityteam.org/download/):
            - [Select a single beat of the song.](https://manual.audacityteam.org/man/audacity_selection.html#mouse)
            - [View the duration at the bottom bar.](https://manual.audacityteam.org/man/audacity_selection.html#selbar) Make sure you are in "Start and Length of Selection" mode.
            - Divide 60 by this duration as fractions of a second then round the resulting value. For example, if a beat lasted 0.25 seconds, your result should be 240.
            - [Make a new mono track via `Tracks -> Add New`.](https://manual.audacityteam.org/man/tracks_menu_add_new.html)
            - Go to [`Generate -> Rhythm Track`](https://manual.audacityteam.org/man/rhythm_track.html).
            - Write your value in the text box next to `Tempo (bpm)`
            - Hit generate then hit play to listen and verify the value is correct.
            - You may need to adjust your value manually.
- Done!