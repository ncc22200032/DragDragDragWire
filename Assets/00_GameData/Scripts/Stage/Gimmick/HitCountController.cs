using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCountController : MonoBehaviour
{
    [SerializeField, Tooltip("�����F")] private Color _initColor;
    [SerializeField, Tooltip("�ω��F")] private Color _setColor;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagName.Player))
        {
            gameObject.layer = LayerName.UnHit;
            gameObject.GetComponent<SpriteRenderer>().color = _setColor;
            PlayerController.Instance.SetUnHookHit();
        }
    }
}
