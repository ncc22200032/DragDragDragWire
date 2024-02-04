using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UIElements;

public class MoveFloorController : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField, Tooltip("移動経由地")] private Transform[] _pos;
    [SerializeField, Tooltip("移動時間")] private float _time;

    // ---------------------------- Field





    // ---------------------------- UnityMessage
    private void Start()
    {
        //  移動経由地初期化
        Vector3[] position = new Vector3[_pos.Length];
        for (int i = 0; i < _pos.Length; i++)
        {
            position[i] = _pos[i].position;
        }

        StartCoroutine(Move(position)); //  移動更新
    }




    // ---------------------------- PublicMethod





    // ---------------------------- PrivateMethod
    /// <summary>
    /// 移動更新
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private IEnumerator Move(Vector3[] position)
    {
        while (true)
        {
            //  移動アニメーション再生
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
