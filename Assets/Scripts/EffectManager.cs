using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] effects;

    [SerializeField]
    private string[] effectNames;

    private Dictionary<string, List<ParticleSystem>> _particlePool = new Dictionary<string, List<ParticleSystem>>();

    private void Awake()
    {
        for (int i = 0; i < effects.Length; ++i)
        {
            List<ParticleSystem> list = new List<ParticleSystem>();
            for (int j = 0; j < 10; ++j)
            {
                GameObject obj = Instantiate(effects[i]);
                obj.SetActive(false);
                list.Add(obj.GetComponent<ParticleSystem>());
            }
            _particlePool.Add(effectNames[i], list);
        }
    }

    void Start()
    {
        
    }

    public void PlayParticle(string name, Vector3 position)
    {
        foreach(var obj in _particlePool[name])
        {
            if (obj.gameObject.activeSelf) continue;

            obj.gameObject.transform.position = position;
            obj.gameObject.SetActive(true);
            obj.Play();

            break;
        }
    }
}
