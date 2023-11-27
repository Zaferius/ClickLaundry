using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductButton : MonoBehaviour
{
    public SewingMachine ownerMachine;
    private void Update()
    {
       Touch();
    }
    private void Touch()
    {
        if (!Input.GetMouseButtonDown(0) || ownerMachine.machineState != SewingMachine.MachineState.Done) return;
        
        var ray = GameManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
        var hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.gameObject == gameObject && GameManager.Instance.paintingAreaLayoutOrganizer.transform.childCount <= 3)
        {
            ToThePaintButton();
        }
    }
    private void ToThePaintButton()
    {
        ownerMachine.ResetMachine();

        var displayProductDefPos =   ownerMachine.displayProduct.transform.localPosition;
        
        ownerMachine.displayProduct.transform.DOScale(0.8f, 0.4f);
        ownerMachine.displayProduct.transform.DOMove(UIManager.Instance.paintButtonWorldPos, 0.4f).OnComplete(() =>
        {
            GameManager.Instance.readyToPaint++;
            if (GameManager.Instance.readyToPaint >= 1)
            {
                UIManager.Instance.StartCoroutine(UIManager.Instance.ColorizePaintButtonC(true));
            }
            
            var transform1 = ownerMachine.displayProduct.transform;
            transform1.localPosition = displayProductDefPos;
            transform1.localScale = Vector3.zero;
            ownerMachine.productDoneSparkle.Stop();

            UIManager.Instance.paintButton.transform.parent.DOShakeScale(0.2f, 0.25f).OnComplete(() =>
            {
                UIManager.Instance.paintButton.transform.parent.DOScale(1, 0.2f);
            });
            
            var product = Instantiate(ownerMachine.doneToPaintProduct, GameManager.Instance.paintingAreaLayoutOrganizer.transform, true);
           product.transform.localScale = Vector3.zero;
           product.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
        
            TimeManager.Instance.transform.DOMoveX(0, 0.025f).OnComplete(() =>
            {
                GameManager.Instance.paintingAreaLayoutOrganizer.ArrangeChildrenHorizontally();

                TimeManager.Instance.transform.DOMoveX(0, 0.25f).OnComplete(() =>
                {
                    foreach (Transform obj in GameManager.Instance.paintingAreaLayoutOrganizer.transform)
                    {
                        obj.GetComponent<PlayObject>().StartPosRemap();
                    }
                });
            });
            
        });
    }
}
