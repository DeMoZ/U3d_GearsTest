using UnityEngine;
using UnityEngine.UI;

public class StatView : MonoBehaviour
{
    [SerializeField] private Text _text = default;
    [SerializeField] private Image _icon = default;

    public void Init(string text, Sprite icon)
    {
        _text.text = text;
        _icon.sprite = icon;
    }
}