using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle countdown for beginning of each round
/// </summary>
public class Countdown : MonoBehaviour
{
    [SerializeField] Sprite[] countdownSprites;
    private void Awake()
    {
        RoundAssembler.TriggerStartFirstRound += StartCountdown;
        RoundAssembler.TriggerStartNewRound += StartCountdown;
    }

    private void StartCountdown() => StartCoroutine(CountdownEnumerator(3));
    private IEnumerator CountdownEnumerator(int total)
    {
        Image image = GetComponent<Image>();
        image.sprite = countdownSprites[2];
        while (true)
        {
            float c = 0;
            total--;
            
            // counter will add up until it reaches1 seconds
            while (c < 1)
            {
                c += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, c);
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;

            }
            if (total == 0)
            {
                yield break;
            }
            else
            {
                image.sprite = countdownSprites[total - 1];
                yield return null;
            }
        }
    }
}
