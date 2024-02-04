using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField, Tooltip("•œ‹AˆÊ’u")] private Transform _pos;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TagName.Player))
        {
            RespawnManager.Instance.RespawnPos = _pos;
        }
    }
}
