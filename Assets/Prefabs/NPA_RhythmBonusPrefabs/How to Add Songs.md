# How to Add New Songs for Rhythm

> Please read [this article](https://support.unity.com/hc/en-us/articles/206484803-What-are-the-supported-Audio-formats-in-Unity) for the list of supported formats:
.aif
.wav
.mp3
.ogg

To add new song entries into the rhythm subsystem to use for combat and other aspects of the game as needed:

- Add your song in a format that is playable in Unity into a predefined folder.
- Select the `MusicManager` game object and go to the `MusicDictionary` component in the inspector.
- Click the plus icon under entries to add a new entry:
	- `Name` your song into something easy to identify for everyone on the team.
	- Drag and drop the song file from Unity's file browser into the `Clip` field.
	- Set the proper `Tempo` specified in beats per minute.
- Done!