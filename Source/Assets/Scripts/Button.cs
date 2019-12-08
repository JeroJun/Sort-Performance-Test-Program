using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Button : MonoBehaviour, IPointerClickHandler
{
    private int index;

    // Reference
    public TextMeshProUGUI fileNameText;

    private void OnValidate()
    {
        if (fileNameText == null) fileNameText = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(int _index, string fileName)
    {
        index = _index;
        fileNameText.text = fileName;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        UI.instance.LoadText(index);
    }
}
