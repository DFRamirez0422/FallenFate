using UnityEngine;

public class AutoSaveTrigger : MonoBehaviour
{
    public AutoSaveUI ui;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SaveManager.instance.SaveGame();
        ui.ShowAutoSave();
    }
}