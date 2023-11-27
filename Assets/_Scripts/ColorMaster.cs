using System.Collections.Generic;
using UnityEngine;

public class ColorMaster : MonoBehaviour
{
    public static ColorMaster Instance;

    public Color productInnerCircleDefaultColor;
    public Color productSpriteInnerFlashColor;
    public List<Color> paintButtonColors;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
}
