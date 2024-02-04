using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseEnum;

public class SpawnerController : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField, Tooltip("�X�C�b�`")] private Switch _generate;
    [SerializeField, Tooltip("�N������")] private int _spawnLimit;
    [SerializeField, Tooltip("�G�I�u�W�F�N�g")] private GameObject _enemy;
    [SerializeField, Tooltip("�����Ԋu")] private Vector2 _generateTime;

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
    /// ��������
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnemyGeneration()
    {
        while (true)
        {
            //  �����Ԋu
            var time = Random.Range(_generateTime.x, _generateTime.y);
            yield return new WaitForSeconds(time);

            if (_generate == Switch.ON && _sr.isVisible)    //  ��������
            {
                //  ����
                var instance = Instantiate(_enemy, transform.position, Quaternion.identity);

                //  ����������
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
