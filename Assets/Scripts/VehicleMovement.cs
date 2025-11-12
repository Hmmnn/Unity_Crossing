using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject[] vehicleModels;

    private GameObject _model;
    public float _moveSpeed { get; set; }
    public bool _goLeft { get; set; }
    private float _xLimit = 12f;

    private Vector3 _lookDir = Vector3.zero;
    private GameObject _owner;

    private void Awake()
    {
        _model = Instantiate(vehicleModels[Random.Range(0, vehicleModels.Length)], transform);
 
        _model.SetActive(false);
    }

    void Start()
    {
        
    }

    void Update()
    {
        _model.SetActive(gameObject.activeSelf);

        transform.position += _lookDir * _moveSpeed * Time.deltaTime;

        if(transform.position.x > _xLimit || transform.position.x < -_xLimit)
        {
            gameObject.SetActive(false);
            _model.SetActive(false);
            _owner.GetComponent<VehicleGenerator>().RemoveDeactive(gameObject);
        }
    }

    public void Activate(float moveSpeed, bool isLeft, Vector3 initialPos, GameObject ownerPath)
    {
        _moveSpeed = moveSpeed;
        transform.position = initialPos;
        _goLeft = isLeft;
        _owner = ownerPath;

        if (_goLeft)
        {
            transform.localRotation = Quaternion.LookRotation(Vector3.left);
            _lookDir = Vector3.left;
        }
        else
        {
            transform.localRotation = Quaternion.LookRotation(Vector3.left * -1f);
            _lookDir = Vector3.left * -1f;
        }

        gameObject.SetActive(true);
    }
}
