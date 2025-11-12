using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject obstacleModel;
    private float obstacleProbability = 25f;
    [SerializeField]
    private int obstacleCount = 12;

    private List<GameObject> _trees = new List<GameObject>();
    private List<float> _positionX = new List<float>();

    #region Unity Method
    void Start()
    {
        float startX = -12f;
        int curCnt = 0;

        while(startX <= 12f)
        {
            if (curCnt > obstacleCount) break;

            float rand = Random.Range(0f, 1f) * 100f;
            if (rand <= obstacleProbability)
            {
                GameObject obs = Instantiate(obstacleModel);
                obs.transform.localPosition = new Vector3(startX, 0f, transform.position.z);
                _trees.Add(obs);
                _positionX.Add(startX);

                ++curCnt;
            }
            else
                _trees.Add(null);

            startX += 2f;
        }
    }

    public void OnDestroy()
    {
        foreach(GameObject obj in _trees)
            Destroy(obj);
        _trees.Clear();
    }
    #endregion

    #region Utils
    public bool CheckObastacle(int index)
    {
        return (_trees[index] != null) ? true : false;
    }

    public bool BreakObstacle(int index)
    {
        if (_trees[index] != null)
        {
            Destroy(_trees[index]);
            _trees[index] = null;
            return true;
        }

        return false;
    }
    #endregion

    private void OnDrawGizmos()
    {
        //foreach(var pos in _positionX)
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawSphere(new Vector3(pos, 0f, transform.position.z), 1f);
        //}
    }
}
