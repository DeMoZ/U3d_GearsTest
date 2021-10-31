using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Text _text = default;
    [SerializeField] private Color _damageColor = Color.red;
    [SerializeField] private Color _hilColor = Color.green;

    public void Init(int value)
    {
        Color color;

        if (value < 0)
        {
            color = _damageColor;
            _text.text = $"-{Mathf.Abs(value)}";
        }
        else
        {
            color = _hilColor;
            _text.text = $"+{Mathf.Abs(value)}";
        }

        _text.color = color;
        Invoke(nameof(Kill), 1);
    }


    private void Update() =>
        (transform as RectTransform).position += Vector3.up * _speed;

    private void Kill() =>
        Destroy(gameObject);
}