using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerHealth : MonoBehaviour
{
    private int maxHealth;
    internal int currentHealth;
    internal bool isDead = false;
}
