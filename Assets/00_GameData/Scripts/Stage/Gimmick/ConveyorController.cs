using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [SerializeField, Tooltip("�����x")] private float _addSpeed;

    public float GetSpeed() { return _addSpeed; }
}
