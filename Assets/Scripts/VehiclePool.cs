using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePool : MonoBehaviour
{
    [SerializeField]
    private GameObject vehiclePrefab;

    private List<GameObject> _vehiclePool = new List<GameObject>();
    private int _poolCount = 30;

    private void Awake()
    {
        for (int i = 0; i < _poolCount; ++i)
        {
            GameObject instance = Instantiate(vehiclePrefab);
            instance.SetActive(false);
            _vehiclePool.Add(instance);
        }
    }
    
    public GameObject ActivateObject(float moveSpeed, bool isLeft, Vector3 initialPos, GameObject ownerPath)
    {
        foreach (GameObject obj in _vehiclePool)
        {
            if (obj.activeSelf) continue;
            VehicleMovement movement = obj.GetComponent<VehicleMovement>();
            movement.Activate(moveSpeed, isLeft, initialPos, ownerPath);

            return obj;
        }

        return null;
    }
}
