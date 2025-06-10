using UnityEngine;
using UnityEngine.UI;

public class GlobalButtonSound : MonoBehaviour
{
    public AudioSource clickAudio;

    void Start()
    {
        // Encuentra todos los botones en la escena (incluso inactivos)
        Button[] allButtons = FindObjectsOfType<Button>(true);

        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(PlayClickSound);
        }
    }

    void PlayClickSound()
    {
        if (clickAudio != null && !clickAudio.isPlaying)
        {
            clickAudio.Play();
        }
    }
}
