Minimap setup instructions

Note - If you do not want to see the player icon in the scene view go to layers and deselect the minimap icon layer. This will not affect the game view.

Video tutorial on how to implement the minimap -

https://youtu.be/v24Ys5eodH4

How to implement the minimap steps

1. Highlight MiniMap camera in the hierarchy of the Act 1 scene and copy it. Paste it on your scene's MainCamera to make it a child.
2. Highlight MiniMap in the hierarchy of the Act 1 scene and copy it and it's children. Paste it into your scenes canvas.
3. Position the MiniMap camera to be above the player.
4. Change the MiniMap camera's size to look good. 15 is a good starting point. 
5. Make sure the MiniMap camera's Culling Mask has the player deselected.
6. Make sure the layer of the player icon layer is set to MiniMapIcon and also that it is activated.
7. Maker sure Main Camera's culling mask should have the MiniMapIcon layer deselected

	Assets and Hierarchy Object Checklist

	Assets
1. MiniMapRenderTexture - You can create a render texture to make it. You can find it in Assets -> Sandbox -> RodneyT -> RT_Assets -> MiniMapRenderTexture
2. Player Icon Sprite - I made a 2d circle for this. You can find it in Assets -> Sandbox -> RodneyT -> Sprite_Assets -> Player Icon
3. Circle Sprite - I made a 2d circle for this. Sprite (2D and UI) | Settings - Sprite Mode = Single - Pixels Per Unit = 256 - Filter Mode = Point (no filter) - Compression = None | You can find it in Assets -> Sandbox -> RodneyT -> Sprite_Assets -> Circle

	Hierarchy Objects
	
1. Main Camera - Culling Mask -> Deselected MiniMapIcon
2. Minimap Camera - It is a duplicated main camera without the audio listener and Cinemachine Brain. You can find it as the child of the Main Camera in act 1 in the hierarchy.
3. MiniMapIcon - Child of the Player | Layer -> MiniMapIcon | Material -> Sprite-Unlit-Default | Sprite -> Player Icon | 
4. MiniMap (Image) - Size = 256x256 | Anchor Point = Top Right | Offset = Pos X -30 Pos Y -30 - Delete the image component. 
5. Border (Image) - Child of MiniMap | Size = 256x256 | Size -> 1.1 on all the scales| Source Image -> Add Circle Sprite and change color to gray | Hierarchy Order -> Move above the MapShape |
6. MapShape (Image) - Child of MiniMap | Source Image = Circle | Add Mask component - Settings - Add mask graphic 
7. MiniMapTexture (Raw Image)- Child of MapShape | In Texture -> Add the MiniMapRenderTexture we created earlier | Size = 256x256 |
8. Background (Image) - Child of MapShape | Source Image = Circle | Size = 256x256 | Hierarchy Order -> Move above the MiniMapTexture | 


	Settings for the assets and hierarchy objects

1. MiniMapRenderTexture - Size = 512X512
2. Player Icon Sprite - Color = Red - Sprite Mode = Single - Pixels Per Unit = 256x256 - Filter Mode = Point (no filter) - Compression = None
3. Circle Sprite - Sprite Mode = Single - Pixels Per Unit = 256x256 - Filter Mode = Point (no filter) - Compression = None
4. Main Camera - This is our main camera
5. Minimap Camera - It is a duplicated main camera without the audio listener and Cinemachine Brain. You can find it as the child of the Main Camera in act 1 in the hierarchy.
6. Minimap Icon - It is a empty game object with a sprite renderer on it - You can find it on the player prefab. Settings - Layer -> MiniMapIcon
7. MiniMap (Image) - Create a UI -> Image | This is a parent for the minimap to position things correctly So we dont need an image on there | You can find one on the canvas in act 1 |
8. Border - Right click MiniMap and Create UI -> Image to make it a child of MiniMap - This to outline the minimap | You can find one on the canvas in act 1 |
9. MapShape (Image) - Right click MiniMap and Create a UI -> Image to make it a child of MiniMap | This is for the shape of the map. You can find one on the canvas in act 1 | I use a circle in my sandbox assets which is in Assets -> Sandbox -> RodneyT -> RT_Assets -> MiniMapRenderTexture |
10. MiniMapTexture - Right click MapShape and Create UI -> Raw Image to make it a child of MapShape - This can take the render texture as its source image | The render texture we created earlier is in Assets -> Sandbox -> RodneyT -> RT_Assets -> MiniMapRenderTexture
11. Background - Right click MapShape and Create UI -> Image to make it a child of MapShape - This is for the background of the minimap | You can find one on the canvas in act 1 |





