using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [SerializeField, Tooltip("‰Á‘¬“x")] private float _addSpeed;

    public float GetSpeed() { return _addSpeed; }
}
