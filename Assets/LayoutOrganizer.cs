using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LayoutOrganizer : MonoBehaviour
{
    public float spacing = 10f;
    
    public enum LayoutType
    {
        SewingArea,
        PaintingArea
    }

    public LayoutType layoutType;

    private void Awake()
    {
        if (layoutType == LayoutType.SewingArea)
        {
            GameManager.Instance.sewingAreaLayoutOrganizer = this;
        }
        else
        {
            GameManager.Instance.paintingAreaLayoutOrganizer = this;
        }
    }


    private void Update()
    {
        // ArrangeChildrenHorizontally();

        if (Input.GetKeyDown(KeyCode.V))
        {
            ArrangeChildrenHorizontally();
        }
    }


    public void ArrangeChildrenHorizontally()
    {
        var currentX = 0f;
        
        foreach (Transform child in transform)
        {
            Vector3 size = child.localScale;

            Vector3 newPosition = new Vector3(currentX + size.x * 0.5f, 0f, 0f);
            child.DOLocalMove(newPosition, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                child.GetComponent<PlayObject>().startingPos = child.transform.localPosition;
            });
            
            currentX += size.x + spacing;
        }
    }
    
    public void ArrangeChildrenHorizontallyFast()
    {
        var currentX = 0f;
        
        foreach (Transform child in transform)
        {
            Vector3 size = child.localScale;

            Vector3 newPosition = new Vector3(currentX + size.x * 0.5f, 0f, 0f);
            child.DOLocalMove(newPosition, 0f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                child.GetComponent<PlayObject>().startingPos = child.transform.localPosition;
            });
            
            
            
            currentX += size.x + spacing;
        }
    }
}
