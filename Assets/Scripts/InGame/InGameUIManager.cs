using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    public void OnHitRespawnButton()
    {
        PlayerSpawnManager.Instance.RespawnPlayersServerRpc();
    }
}
