using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseEnum;

public class SpawnerController : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField, Tooltip("スイッチ")] private Switch _generate;
    [SerializeField, Tooltip("湧き制限")] private int _spawnLimit;
    [SerializeField, Tooltip("敵オブジェクト")] private GameObject _enemy;
    [SerializeField, Tooltip("生成間隔")] private Vector2 _generateTime;

    // ---------------------------- Field
    private SpriteRenderer _sr;
    private List<GameObject> _totalCount = new() { };




    // ---------------------------- UnityMessage
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(EnemyGeneration());
    }




    // ---------------------------- PublicMethod





    // ---------------------------- PrivateMethod
    /// <summary>
    /// 生成制御
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyGeneration()
    {
        while (true)
        {
            //  生成間隔
            var time = Random.Range(_generateTime.x, _generateTime.y);
            yield return new WaitForSeconds(time);

            if (_generate == Switch.ON && _sr.isVisible)    //  生成判定
            {
                //  生成
                var instance = Instantiate(_enemy, transform.position, Quaternion.identity);

                //  生成数制限
                _totalCount.Add(instance);
                if (_totalCount.Count > _spawnLimit)
                {
                    if (_totalCount[0] != null)
                    {
                        _totalCount[0].GetComponent<IEnemyDamageable>().Die();
                    }
                    _totalCount.RemoveAt(0);
                }
            }

            yield return null;
        }
    }
}
