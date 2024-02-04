using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UIElements;

public class MoveFloorController : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField, Tooltip("�ړ��o�R�n")] private Transform[] _pos;
    [SerializeField, Tooltip("�ړ�����")] private float _time;

    // ---------------------------- Field





    // ---------------------------- UnityMessage
    private void Start()
    {
        //  �ړ��o�R�n������
        Vector3[] position = new Vector3[_pos.Length];
        for (int i = 0; i < _pos.Length; i++)
        {
            position[i] = _pos[i].position;
        }

        StartCoroutine(Move(position)); //  �ړ��X�V
    }




    // ---------------------------- PublicMethod





    // ---------------------------- PrivateMethod
    /// <summary>
    /// �ړ��X�V
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private IEnumerator Move(Vector3[] position)
    {
        while (true)
        {
            //  �ړ��A�j���[�V�����Đ�
            transform.DOPath
                (position
                , _time
                , PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .SetOptions(true)
                .SetLink(gameObject);

            yield return new WaitForSeconds(_time);
            yield return null;
        }
    }
}
