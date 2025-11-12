using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance { get; private set; }

    public const int _halfSize = 12;

    [Header("Field Prefab")]
    [SerializeField]
    private GameObject groundPrefab;
    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private GameObject pathPrefab;

    [SerializeField]
    private int startZ = 18;

    [Header("Path")]
    [SerializeField]
    private int _curPathProb = 0;
    private const int _minPathProb = 1;

    [Header("Player")]
    [SerializeField]
    private GameObject player;

    // player
    private PlayerController _playerController;

    // Map Spawn
    private List<GameObject> _grounds = new List<GameObject>();
    private int _gridSize = 2;
    private int _obstacleCount = 0;

    private int _lastPos = 0;
    private int _destroyIndex = 0;

    #region Unity Method
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _playerController = player.GetComponent<PlayerController>();

        _lastPos = startZ;
        while (_lastPos < 24)
        {
            _lastPos += _gridSize;
            AddGrassPart(_lastPos);
        }

        _obstacleCount = 12 * 2 / _gridSize;
    }

    void Update()
    {
        float nextZ = _playerController.nextZ;

        while (nextZ > _lastPos - 24f)
        {
            int conv = (int)(_lastPos + _gridSize);

            if(Random.Range(1, 10) <= _curPathProb)
            {
                AddPathPart(conv);

                _curPathProb = _minPathProb;
            }
            else
            {
                AddGrassPart(conv);

                ++_curPathProb;
            }
        }
    }

    private void LateUpdate()
    {
        DestroyPassedPart();
    }

    #endregion

    #region Utils

    private void AddGrassPart(int newZ)
    {
        GameObject newGround = Instantiate(groundPrefab, transform);
        newGround.transform.position = new Vector3(0f, 0f, newZ);
        _grounds.Add(newGround);

        _lastPos = newZ;
    }

    private void AddPathPart(int newZ)
    {
        GameObject newPath = Instantiate(pathPrefab, transform);
        newPath.transform.position = new Vector3(0f, 0f, newZ);
        _grounds.Add(newPath);

        _lastPos = newZ;
    }

    public bool CheckBlock(Vector3 playerPosition)
    {
        // -에서 시작하는 x 포지션이랑 0에서 시작하는 index 일치시키기
        // 막히면 true 통과하면 false

        float playerconvX = 0;
      
        playerconvX = playerPosition.x + (float)_halfSize;
        
        int indexX = (int)playerconvX / _gridSize;
        int indexZ = (int)playerPosition.z / _gridSize;

        if (playerPosition.z <= startZ)
        {
            if (playerPosition.z < 0) return true;
            if (playerPosition.x < -6 || playerPosition.x > 6)
                return true;
            return false;
        }

        int startArea = startZ / _gridSize;
        ObstacleGenerator obsGen = null;
        if (_grounds[indexZ - startArea - 1].TryGetComponent<ObstacleGenerator>(out obsGen))
        {
            return obsGen.CheckObastacle(indexX);
        }

        return false;
    }

    public bool BreakBlock(Vector3 attackPosition)
    {
        float attackConvX = 0;

        attackConvX = attackPosition.x + (float)_halfSize;

        int indexX = (int)attackConvX / _gridSize;
        int indexZ = (int)attackPosition.z / _gridSize;

        if (attackPosition.z <= startZ)
        {
            return false;
        }

        int startArea = startZ / _gridSize;
        ObstacleGenerator obsGen = null;
        if (_grounds[indexZ - startArea - 1].TryGetComponent<ObstacleGenerator>(out obsGen))
        {
            return obsGen.BreakObstacle(indexX);
        }
        
        return false;
    }

    private void DestroyPassedPart()
    {
        // 기준을 잡고 그 이전에 만든 맵은 없애기
        // 플레이어 위치에서 5 정도 뒤면 Destroy
        Vector3 playerPos = player.transform.position;
        int indexZ = (int)playerPos.z / _gridSize;
        indexZ -= startZ / _gridSize;
        
        if(_destroyIndex < indexZ - 5)
        {
            for(int i = _destroyIndex; i < indexZ - 5; ++i)
            {
                Destroy(_grounds[i]);
            }

            _destroyIndex = indexZ - 5;
        }
    }

    public void InitializePlayMap()
    {
        foreach(GameObject obj in _grounds)
            if (obj != null) Destroy(obj);
        _grounds.Clear();

        _lastPos = startZ;
        while (_lastPos < 24)
        {
            _lastPos += _gridSize;
            AddGrassPart(_lastPos);
        }
    }

    private void OnDrawGizmos()
    {
        float playerPosition = player.transform.position.z;
        Debug.DrawRay(new Vector3(-_gridSize, 0, playerPosition), Vector3.forward * 20f, Color.red);
        Debug.DrawRay(new Vector3(_gridSize, 0, playerPosition), Vector3.forward * 20f, Color.red);

        Debug.DrawRay(new Vector3(-12, 0, playerPosition + _gridSize), Vector3.right * 20f, Color.red);
        Debug.DrawRay(new Vector3(-12, 0, playerPosition + _gridSize * 2), Vector3.right * 20f, Color.red);
        Debug.DrawRay(new Vector3(-12, 0, playerPosition + _gridSize * 3), Vector3.right * 20f, Color.red);
        Debug.DrawRay(new Vector3(-12, 0, playerPosition + _gridSize * 4), Vector3.right * 20f, Color.red);
        Debug.DrawRay(new Vector3(-12, 0, playerPosition + _gridSize * 5), Vector3.right * 20f, Color.red);
    }

    #endregion
}
