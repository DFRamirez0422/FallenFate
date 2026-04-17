#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Catches multiple StatTracker components in one scene file (data error).
/// Menu command also warns when StatTracker appears in more than one enabled build scene.
/// </summary>
public sealed class StatTrackerBuildAudit : IPreprocessBuildWithReport
{
    public const string StatTrackerScriptGuid = "d7d389c82b9e20f4fbd2b6fa2ea99432";

    public int callbackOrder => 0;

    [MenuItem("Tools/Fallen Fate/Validate StatTracker In Build Scenes")]
    public static void ValidateFromMenu()
    {
        ValidateAndLog(multiSceneWarning: true);
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        ValidateAndLog(multiSceneWarning: false);
    }

    static void ValidateAndLog(bool multiSceneWarning)
    {
        var scenePaths = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        int scenesWithAtLeastOne = 0;
        bool anyError = false;

        foreach (string path in scenePaths)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                continue;

            string text = File.ReadAllText(path);
            int count = Regex.Matches(text, Regex.Escape(StatTrackerScriptGuid)).Count;
            if (count > 1)
            {
                anyError = true;
                Debug.LogError(
                    $"[StatTracker] Scene file references StatTracker {count} times (duplicate components). Fix: {path}",
                    AssetDatabase.LoadAssetAtPath<Object>(path));
            }
            else if (count == 1)
            {
                scenesWithAtLeastOne++;
            }
        }

        if (anyError)
        {
            throw new BuildFailedException(
                "[StatTracker] One or more scenes contain multiple StatTracker components. Remove duplicates (see Console).");
        }

        if (multiSceneWarning && scenesWithAtLeastOne > 1)
        {
            Debug.LogWarning(
                "[StatTracker] StatTracker MonoBehaviour appears in " + scenesWithAtLeastOne +
                " enabled build scenes. Only one instance should carry state at runtime (singleton); " +
                "remove duplicate HealthTracker/StatTracker objects from level scenes and keep a single bootstrap (e.g. Main Menu). " +
                "Run this check after adding persistence objects to scenes.",
                null);
        }
        else if (multiSceneWarning && !anyError)
        {
            Debug.Log("[StatTracker] Build scene audit: no duplicate StatTracker components per file. See any warnings above for multi-scene placement.");
        }
    }
}
#endif
