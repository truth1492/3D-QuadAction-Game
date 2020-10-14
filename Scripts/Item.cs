using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Coin, Heart, Weapon};
    public Type type;
    public int value;

    void Update()
    {
        transform.Rotate(Vector3.up * 40 * Time.deltaTime);
    }

}
