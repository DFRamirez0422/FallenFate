# Creating an Actor SO

To properly display a portrait in the dialogue boxes as well as define which actors have been talked to for conditional dialogue, you must create an actor SO.

The actor must have a name of up to a variable number of characters that can fit in the text box in the UI as well as a default portrait to be displayed only if a dialogue box emotion isn't found.

Emotion portraits tell the game which image sprite should be displayed given an emotion option given in the dialogue SO. Currently, the following options are declared:

- `Idle`
- `Idle_Talk`
- `Sad`
- `Sad_Talk`
- `Quizzical`
- `Quizzical_Talk`

Please be advised that you should unique emotions per portrait element; in other words, you can have either zero or one of "Sad", "Sad_Talk", "Idle", and so on, and having multiple identital emotions is undefined behavior.

# Creating a new Dialogue SO

## Lines

You can define any number of "lines" elements to be each displayed in a dialogue box. The maximum number of text lines is three per dialogue box. Additionally, you can specify which actor SO to be used as well as their emotion, both of which define the actor portrait to be shown next to the dialogue box. There is no restriction for text in the dialogue box, except that null and control characters may not work properly; Unicode text in different languages display properly given a font that can support them.

There is no need to create an option to end dialogue; the last dialogue box will always either display up to four options if they are defined or a button to automatically terminate dialogue.

## Options

If you are intending to display multiple options in a dialogue box, this is the place to do so! Please note that the options will only display after the last dialogue box and that you may only have up to four options at a time.

When a new option is added, you can select an option to be performed upon selection. These options include:

- `NewDialogue`: Branches to a new dialogue tree. This is the **default** behavior when creating a new Dialogue SO.
- `ChangeScene`: Changes to a new scene given in the parameter labeled "Scene Name."

## Action upon Dialogue Ending

Once a dialogue tree has ended, there are several options that you may pick that define the behavior to be done after. These options include:

- `EndDialogue`: Ends the dialogue tree and performs no other action. This is the **default** behavior when creating a new Dialogue SO.
- `NewDialogue`: Starts a new dialogue tree under "New Dialogue" immediately after.
- `ChangeScene`: Changes to a new scene given in the parameter labeled "Scene Name."
- `SetObjectsActive`: Sets active to all game objects in the scene given in the list "Objects to Activate."
- `InstantiateObjects`: Instantiates all prefabs given in the list "Objects to Instantiate." The spawn position of the new prefabs are TBD as of this time of writing.