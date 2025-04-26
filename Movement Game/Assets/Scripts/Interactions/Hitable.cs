using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hitable : MonoBehaviour
{
    public bool damageable = true;
    public bool knockback = true;
    public abstract void TakeDamage();
}
