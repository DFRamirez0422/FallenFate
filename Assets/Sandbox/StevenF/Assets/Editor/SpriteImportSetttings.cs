using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpriteImportSettings : EditorWindow
{
    [MenuItem("Tools/Configure Ascendence Sprites")]
    public static void ConfigureSprites()
    {
        // Define sprite configurations
        var spriteConfigs = new Dictionary<string, SpriteConfig>
        {
            // Bush sprites - 96x32, 3 frames of 32x32
            { "Bush-Sheet_2_0", new SpriteConfig(32, 32, true) },
            { "Bush_Alt1-Sheet", new SpriteConfig(32, 32, true) },
            { "Bush_With_Flowers-Sheet_2_0", new SpriteConfig(32, 32, true) },
            { "BushWithFlowers_Alt1-Sheet", new SpriteConfig(32, 32, true) },
            { "Bush_with_snow-Sheet_2_0", new SpriteConfig(32, 32, true) },
            { "BushWithSnow_Alt1-Sheet", new SpriteConfig(32, 32, true) },
            { "BushWithSnow_Flowers-Sheet", new SpriteConfig(32, 32, true) },
            { "BushFlowersSnow_Alt1-Sheet", new SpriteConfig(32, 32, true) },
            
            // Flower sprites - 128x32, 4 frames of 32x32
            { "The_Flowers-Sheet1", new SpriteConfig(32, 32, true) },
            { "The_Flowers-Sheet2", new SpriteConfig(32, 32, true) },
            { "The_Flowers-Sheet3", new SpriteConfig(32, 32, true) },
            { "TheFlowers_Snow-Sheet1", new SpriteConfig(32, 32, true) },
            { "TheFlowers_Snow-Sheet2", new SpriteConfig(32, 32, true) },
            { "TheFlowers_Snow-Sheet3", new SpriteConfig(32, 32, true) },
            
            // Blood splatter - 128x32, 4 frames of 32x32
            { "blood-Sheet_1___2_R_wall__3___4_R_floor_", new SpriteConfig(32, 32, true) },
            
            // Dirt/Scratch textures - 384x64, 6 frames of 64x64
            { "Dirt_and_Scratch_Texture-Sheet", new SpriteConfig(64, 64, true) },
            
            // Tally marks - 1024x64, 16 frames of 64x64
            { "tally_marks_wall-Sheet", new SpriteConfig(64, 64, true) },
            
            // Single sprites (no slicing)
            { "File_Cabinet", new SpriteConfig(64, 64, false) },
            { "Warden_Office_Chair", new SpriteConfig(64, 64, false) },
            { "Warden_Desk", new SpriteConfig(32, 32, false) },
        };

        int configuredCount = 0;

        // Find all textures in the project
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            if (spriteConfigs.TryGetValue(fileName, out SpriteConfig config))
            {
                ConfigureSprite(path, config);
                configuredCount++;
                Debug.Log($"Configured: {fileName}");
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Sprite Import Complete",
            $"Configured {configuredCount} sprites for pixel art.\n\n" +
            "Settings applied:\n" +
            "• Filter Mode: Point (no filter)\n" +
            "• Compression: None\n" +
            "• Pixels Per Unit: 32\n" +
            "• Sprite sheets sliced automatically",
            "OK");
    }

    private static void ConfigureSprite(string path, SpriteConfig config)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;

        // Basic sprite settings
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 32; // Adjust if your game uses different scale
        importer.filterMode = FilterMode.Point; // Critical for pixel art!
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false; // Not needed for 2D pixel art

        // Set sprite mode
        if (config.isMultiple)
        {
            importer.spriteImportMode = SpriteImportMode.Multiple;

            // Read texture to get dimensions
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex != null)
            {
                int cols = tex.width / config.cellWidth;
                int rows = tex.height / config.cellHeight;

                List<SpriteMetaData> spriteSheet = new List<SpriteMetaData>();
                string baseName = System.IO.Path.GetFileNameWithoutExtension(path);

                int index = 0;
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        SpriteMetaData meta = new SpriteMetaData();
                        meta.name = $"{baseName}_{index}";
                        meta.rect = new Rect(
                            col * config.cellWidth,
                            tex.height - (row + 1) * config.cellHeight, // Unity Y is bottom-up
                            config.cellWidth,
                            config.cellHeight
                        );
                        meta.pivot = new Vector2(0.5f, 0.5f); // Center pivot
                        meta.alignment = (int)SpriteAlignment.Center;

                        spriteSheet.Add(meta);
                        index++;
                    }
                }

                importer.spritesheet = spriteSheet.ToArray();
            }
        }
        else
        {
            importer.spriteImportMode = SpriteImportMode.Single;
        }

        // Apply settings
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    private class SpriteConfig
    {
        public int cellWidth;
        public int cellHeight;
        public bool isMultiple;

        public SpriteConfig(int width, int height, bool multiple)
        {
            cellWidth = width;
            cellHeight = height;
            isMultiple = multiple;
        }
    }
}