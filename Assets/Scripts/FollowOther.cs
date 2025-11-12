using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOther : MonoBehaviour
{
    public enum FOLLOWAXIS { X, Y, Z, ALL}

    [SerializeField]
    private GameObject followTarget;
    public FOLLOWAXIS followMode = FOLLOWAXIS.ALL;

    private Transform _followTransform;

    void Start()
    {
        _followTransform = followTarget.GetComponent<Transform>();
        Debug.Assert(_followTransform, "Follow Target Missing");
    }

    void Update()
    {
        switch(followMode)
        {
            case FOLLOWAXIS.X:
                transform.position = new Vector3(_followTransform.position.x, transform.position.y, transform.position.z);
                break;
            case FOLLOWAXIS.Y:
                transform.position = new Vector3(transform.position.x, _followTransform.position.y, transform.position.z);
                break;
            case FOLLOWAXIS.Z:
                transform.position = new Vector3(transform.position.x, transform.position.y, _followTransform.position.z);
                break;
            case FOLLOWAXIS.ALL:
                transform.position = _followTransform.position;
                break;
        }
    }
}
