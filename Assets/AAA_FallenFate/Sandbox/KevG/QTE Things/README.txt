SETUP

1. Create a Canvas (make sure to scale with screen size)

2. Make a Lane Group
	A- Inside the Canvas, create an empty GameObject
	B- Name it "QTE_Lanes" (or whatever you want)

3. Add Lanes
	A- Inside QTE_Lanes make a few UI images for the lanes (could be a simple white box image)
	B- Position them at the bottom of the screen
	C- Attach the QTE_Lane script
	D- Assign the key in the Inspector under ASSIGNED KEY
	EXTRA: You can add a text inside the UI images and position them under the lane

4. Add the Hitline and Hitbox
	A- Inside QTE_Lanes create a UI Image and name it "Hitline"
	B- Make it a thin horizontal rectangle across all lanes (this one is visible)
	C- Inside the Hitline, create another UI Image and name it "Hitbox"
	D- Make the Hitbox slightly thicker than the Hitline and set its color alpha to 0 (so it’s invisible) OR disable the image component
	E- This invisible Hitbox will be used for hit detection
	F- Assign the Hitbox image to the HitBox field in the QTE_Manager

5. Create a Note Prefab
	A- Create a UI image anywhere in the scene
	B- Name it QTE_Note (or whatever you want)
	C- Attach QTENote script
	D- Drag into the Project Window to create a prefab
	E- Delete prefab from scene
	E- Adjust the prefab to your liking

6. Create the Manager
	A- Create an empty GameObject and name it QTE_Manager (or whatever you want)
	B- Attach QTEManager script
	C- Assign the required items into the inspector box

How it works:

Press SPACE to spawn a random notes

The notes will fall downward into the lanes

Press the correct key you assigned it to

Depending what color you chose in the inspector for the Prefab will determine your feedback on the missed and hit notes.