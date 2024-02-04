using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;
    // ---------------------------- SerializeField
    [SerializeField, Tooltip("�����n�_")] private Transform _pos;

    // ---------------------------- Field
    private Transform _respawnPos;

    // ---------------------------- Property
    /// <summary>
    /// ���X�|�[���n�_�擾�X�V
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
