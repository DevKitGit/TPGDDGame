using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spritemaskanimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    public SpriteMask spritemask; // Reference to the Sprite Mask
    void Start()
    {
        StartCoroutine(AnimateSpriteMask()); // We start the animation as soon as the Game runs
    }
    IEnumerator AnimateSpriteMask()
    {
        while (true)
        {
            if(spritemask.sprite != spriteRenderer.sprite)
            {
                spritemask.sprite = spriteRenderer.sprite;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
