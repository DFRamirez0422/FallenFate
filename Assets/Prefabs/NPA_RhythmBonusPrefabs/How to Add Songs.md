# How to Add New Songs for Rhythm

> Please read [this article](https://support.unity.com/hc/en-us/articles/206484803-What-are-the-supported-Audio-formats-in-Unity) for the list of supported formats:
.aif
.wav
.mp3
.ogg

To add new song entries into the rhythm subsystem to use for combat and other aspects of the game as needed:

- Add your song in a format that is playable in Unity into a predefined folder.
- Select the `MusicManager` game object and go to the `MusicDictionary` component in the inspector.
- Click the plus icon under `Music Entries` to add a new entry:
	- `Name` your song into something easy to identify for everyone on the team.
	- Drag and drop the song file from Unity's file browser into the `Music Clip` field.
	- Set the proper `Tempo` specified in quarter beats per minute.
		- If you don't know the tempo, you can use Audacity to select a single beat of the song, view the duration, divide 60 by this duration as fractions of a second, then round the resulting value.
		- Then, in Audacity, make a new mono track, go to Generate -> Rhythm Track, then write the tempo to verify the value is correct.
- Done!