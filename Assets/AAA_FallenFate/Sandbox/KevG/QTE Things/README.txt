Setup:

1. Create a Canvas (make sure to scale with screen size)

2. Make a Lane Group	
	A- Inside the Canvas, create an empty GameObject
	B- Name it "QTE_Lanes" (or whatever you want)

3. Add Holders
	A- Inside QTE_Lanes make a few UI images for the holders (could be a simple white box image)
	B- Position them at the bottom of the screen
	C- Attach the QTE_Holder script
	D- Assign the key in the Inspector under ASSIGNED KEY
EXTRA: You can add a text inside the UI images and position them under the holder

4. Create a Note Prefab 
	A- Create a UI image anywhere in the scene
	B- Name it QTE_Note (or whatever you want)
	C- Attach QTENote script
	D- Drag into the Project Window to create a prefab
	E- Adjust the prefab to your liking

5. Create the Spawner
	A- Create an empty GameObject and name it QTE_Spawner (or whatever you want)
	B- Attach QTESimpleSpawner script
	C- Assign the required items into the inspector box


How it works:

Press SPACE to spawn a random note

The note will fall downward into the holder

Press the correct key you assigned it to

Depending what color you chose in the inspector for the Prefab will determine your feedback on the missed and hit notes.
	