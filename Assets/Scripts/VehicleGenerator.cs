using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGenerator : MonoBehaviour
{
    [SerializeField]
    private float vehicleSpeed = 3f;
    //[SerializeField]
    private float spawnInterval = 3f;
    private float _baseInterval = 3f;

    private VehiclePool _pooling = null;

    private float _timeAcc = 0f;
    private List<GameObject> _vehicles = new List<GameObject>();
    
    private bool _goLeft;

    private void Awake()
    {
        _goLeft = Random.Range(0, 2) == 0 ? false : true;
    }

    void Start()
    {
        _pooling = GameObject.Find("VehiclePool").GetComponent<VehiclePool>();
        Debug.Assert(_pooling, "Vehicle Pool Missing");
    }

    void Update()
    {
        _timeAcc += Time.deltaTime;

        if(_timeAcc > spawnInterval)
        {
            _timeAcc -= _timeAcc;
            spawnInterval = _baseInterval + Random.Range(-1f, 1f);

            float xPos = _goLeft ? 11f : -11f;
            _vehicles.Add(_pooling.ActivateObject(vehicleSpeed, _goLeft, new Vector3(xPos, 0f, transform.position.z), gameObject));
        }
    }

    private void OnDestroy()
    {
        if (_pooling == null) return;

        foreach (GameObject obj in _vehicles)
        {
            if(obj != null)
                obj.SetActive(false);
        }

        _vehicles.Clear();
    }

    //public void Deactivate()
    //{
    //    foreach (GameObject obj in _vehicles)
    //        obj.SetActive(false);
    //
    //    _vehicles.Clear();
    //}

    public void RemoveDeactive(GameObject obj)
    {
        _vehicles.Remove(obj);
    }
}
