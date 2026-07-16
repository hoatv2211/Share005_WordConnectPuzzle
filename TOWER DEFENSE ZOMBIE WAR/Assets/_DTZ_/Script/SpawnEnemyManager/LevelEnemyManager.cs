using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnemyManager : MonoBehaviour, IListener
{
    public static LevelEnemyManager Instance;
    public Transform[] spawnPositions;
    public EnemyWave[] EnemyWaves;
    int currentWave = 0;

    List<GameObject> listEnemySpawned = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    int totalEnemy, currentSpawn;
    // Start is called before the first frame update
    void Start()
    {
        if (GameLevelSetup.Instance)
            EnemyWaves = GameLevelSetup.Instance.GetLevelWave();

        //calculate number of enemies
        totalEnemy = 0;
        for (int i = 0; i < EnemyWaves.Length; i++)
        {
            for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
            {
                var enemySpawn = EnemyWaves[i].enemySpawns[j];
                totalEnemy += enemySpawn.numberWaves * enemySpawn.spawmAmountEnemyTogether;
            }
        }
        currentSpawn = 0;
    }

    IEnumerator SpawnEnemyCo()
    {
        for (int i = 0; i < EnemyWaves.Length; i++)
        {
            yield return new WaitForSeconds(EnemyWaves[i].wait);

            for (int j = 0; j < EnemyWaves[i].enemySpawns.Length; j++)
            {
                var enemySpawn = EnemyWaves[i].enemySpawns[j];
                yield return new WaitForSeconds(enemySpawn.delayOnBegin);
                for (int k = 0; k < enemySpawn.numberWaves; k++)
                {
                    if (enemySpawn.spawmAmountEnemyTogether == 1)
                    {
                        GameObject _temp = Instantiate(enemySpawn.enemy[Random.Range(0, enemySpawn.enemy.Length)], (Vector2)spawnPositions[Random.Range(0, spawnPositions.Length)].position, Quaternion.identity) as GameObject;
                        _temp.SetActive(false);
                        _temp.transform.parent = transform;

                        yield return new WaitForSeconds(0.1f);
                        _temp.SetActive(true);
                        listEnemySpawned.Add(_temp);

                        currentSpawn++;
                        MenuManager.Instance.UpdateEnemyWavePercent(currentSpawn, totalEnemy);
                    }
                    else
                    {
                        List<Transform> _pos = new List<Transform>(spawnPositions);
                        for(int m = 0; m < enemySpawn.spawmAmountEnemyTogether; m++)
                        {
                            int _index = Random.Range(0, _pos.Count);

                            GameObject _temp = Instantiate(enemySpawn.enemy[Random.Range(0, enemySpawn.enemy.Length)], (Vector2)_pos[_index].position, Quaternion.identity) as GameObject;
                            _temp.SetActive(false);
                            _temp.transform.parent = transform;
                            _temp.SetActive(true);
                            listEnemySpawned.Add(_temp);

                            currentSpawn++;
                            MenuManager.Instance.UpdateEnemyWavePercent(currentSpawn, totalEnemy);

                            _pos.RemoveAt(_index);
                            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                        }
                    }

                    yield return new WaitForSeconds(enemySpawn.waveRate * (Random.Range(0.9f, 1.1f)));
                }
            }
        }

        //check all enemy killed
        while (isEnemyAlive()) { yield return new WaitForSeconds(0.1f); }

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.Victory();
    }


    bool isEnemyAlive()
    {
        for(int i = 0; i< listEnemySpawned.Count;i++)
        {
            if (listEnemySpawned[i].gameObject != null && listEnemySpawned[i].activeInHierarchy)
                return true;
        }

        return false;
    }

    public void IGameOver()
    {
        StopAllCoroutines();
        //throw new System.NotImplementedException();
    }

    public void IOnRespawn()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOff()
    {
        //throw new System.NotImplementedException();
    }

    public void IOnStopMovingOn()
    {
        //throw new System.NotImplementedException();
    }

    public void IPause()
    {
        //throw new System.NotImplementedException();
    }

    public void IPlay()
    {
        StartCoroutine(SpawnEnemyCo());
        //throw new System.NotImplementedException();
    }

    public void ISuccess()
    {
        StopAllCoroutines();
        //throw new System.NotImplementedException();
    }

    public void IUnPause()
    {
        //throw new System.NotImplementedException();
    }
}

[System.Serializable]
public class EnemyWave
{
    public float wait = 3;
    public EnemySpawn[] enemySpawns;
}


