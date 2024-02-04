using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;
    // ---------------------------- SerializeField
    [SerializeField, Tooltip("復活地点")] private Transform _pos;

    // ---------------------------- Field
    private Transform _respawnPos;

    // ---------------------------- Property
    /// <summary>
    /// リスポーン地点取得更新
    /// </summary>
    public Transform RespawnPos
    {
        get { return _respawnPos; }
        set { _respawnPos = value; }
    }


    // ---------------------------- UnityMessage
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RespawnPos = _pos;
    }
}
