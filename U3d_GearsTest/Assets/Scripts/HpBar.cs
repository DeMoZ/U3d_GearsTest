using System;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image _image = default;
    [SerializeField] private DamageText _textPrefab = default;

    private Transform _anchor;
    private Camera _mainCamera;

    public void Init(Transform anchor)
    {
        _anchor = anchor;
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        (transform as RectTransform).position = _mainCamera.WorldToScreenPoint(_anchor.position);
    }

    public void SetHp(float relativeValue)
    {
        _image.fillAmount = relativeValue;
    }

    public void ShowDamage(int value)
    {
        var hpText= Instantiate(_textPrefab, transform);
        hpText.Init(value);
    }
}