using UnityEngine;
using UnityEngine.UI;

public class MusicSliderBinder : MonoBehaviour
{
    private void Start()
    {
        Slider slider = GetComponent<Slider>();
        if (slider == null) return;

        MusicManager manager = FindObjectOfType<MusicManager>();
        if (manager != null)
        {
            manager.BindSlider(slider);
        }
    }
}
