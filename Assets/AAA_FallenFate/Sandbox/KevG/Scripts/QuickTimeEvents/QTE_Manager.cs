using UnityEngine;

public class QTE_Manager : MonoBehaviour
{
    public QTE_Note qtePrefab;           // drag the prefab of the note
    public RectTransform parentCanvas;   // drag your canvas or root rect

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // when you press [SPACE] a note will spawn
            SpawnQTE();                      // call the function to create the note
    }

    public void SpawnQTE()
    {
        var note = Instantiate(qtePrefab, parentCanvas); // create a copy of the note prefab inside the canvas
        note.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // center the note
    }
}

