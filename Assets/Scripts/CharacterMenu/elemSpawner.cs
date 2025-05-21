using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elemSpawner : MonoBehaviour
{
    public CharacterMenu _characterMenu;

    public GameObject prefab;


    public void FillUIElements()
    {
        Instantiate(prefab, transform.position, transform.rotation);
    }
}
