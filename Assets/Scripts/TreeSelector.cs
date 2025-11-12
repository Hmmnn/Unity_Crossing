using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] treeModels;
    private GameObject _model;

    private void Awake()
    {
        _model = Instantiate(treeModels[Random.Range(0, treeModels.Length)], transform);
        _model.transform.localScale = new Vector3(2f, 2f, 2f);
    }
}
