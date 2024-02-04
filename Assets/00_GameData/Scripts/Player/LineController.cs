using UnityEngine;

public class LineController : MonoBehaviour
{
    #region ---------------------------- SelializeField
    [SerializeField, Tooltip("�v���C���[")] private PlayerController _player;
    [SerializeField, Tooltip("�`�ʃ^�C�v")] private PlayerController.LineType _type;

    #endregion -------------------------

    #region ---------------------------- Field
    private LineRenderer _line;
    private Vector3[] _edge;

    #endregion -------------------------



    #region ---------------------------- UnityMessage
    private void Awake()
    {
        _line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Draw();
    }

    #endregion -------------------------



    #region ---------------------------- Private
    /// <summary>
    /// �`��
    /// </summary>
    private void Draw()
    {
        switch(_type)
        {
            case PlayerController.LineType.ROPE:
                _edge = _player.GetEdge(PlayerController.LineType.ROPE);
                break;

            case PlayerController.LineType.INDICATOR:
                _edge = _player.GetEdge(PlayerController.LineType.INDICATOR);
                break;

            default:
                break;
        }
        _line.SetPositions(_edge);
    }

    #endregion -------------------------
}
