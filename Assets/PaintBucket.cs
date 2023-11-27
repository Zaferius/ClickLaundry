using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PaintBucket : MonoBehaviour
{
    public bool isActive;
    public int requiredLevel;
    public enum MachineState
    {
        Locked,
        Ready,
        Working,
    }

    public MachineState machineState;
    
    public float workTime;
    public float workTimer;
    public int coinToCollect;
    
    [HideInInspector] public Color bucketColor;
     public PlayObject product;
     public PlayObject productTemp;

    [Header("+ Transform +")] 
    public Transform productPos;

    [Header("+ Work +")] 
    public Transform circleHolder;
    public SpriteRenderer circleFillSprite;

    [Header("+ FX +")] 
    private Tween _paintingAnim;
    private Tween _paintingAnim2;
    private Tween _bucketShakeAnim;
    public ParticleSystem boilingEffect;
    
    [Header("+ Lock Side +")]
    public GameObject lockedObjectsHolder;
    public TextMeshPro requiredLevelText;

    private void Start()
    {
        CheckMachine();
        bucketColor = GetComponent<MeshRenderer>().materials[1].color;
        circleHolder.transform.localScale = Vector3.zero;
    }
    
    void Update()
    {
        if (!isActive) return;
        workTimer -= Time.deltaTime;

        if (!(workTimer <= 0)) return;
        isActive = false;
        workTimer = workTime;
        FinishColoring();
    }

    public void ToBucket(PlayObject obj)
    {
        product = obj;

        SetPaintBucketStats();
        
        product.transform.DOLocalMove(productPos.transform.position, 0.2f).OnComplete(() =>
        {
           _paintingAnim = product.transform.DOLocalMoveY(product.transform.localPosition.y + 0.15f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
           _paintingAnim2 = product.transform.DOLocalRotate(new Vector3(0, 0, 20), 1.35f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
           _bucketShakeAnim = transform.DOScale(new Vector3(1.05f,0.95f,1.05f), 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        });

        product.transform.DOScale(1, 0.2f);
        product.transform.parent = null;
        product.gameObject.layer = 0;
        ColorizeProduct();
    }

    private void ColorizeProduct()
    {
        isActive = true;
        machineState = MachineState.Working;
        SoundManager.Instance.PlaySound("BubbleLoop", 0.03f, true);
        boilingEffect.Play();
        circleHolder.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
        product.transform.GetChild(0).GetComponent<MeshRenderer>().material.DOColor(bucketColor, workTime);
        circleFillSprite.material.DOFloat(0, "_Arc1", workTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            circleFillSprite.material.DOFloat(360, "_Arc1", 0);
        });

    }

    private void FinishColoring()
    {
        machineState = MachineState.Ready;
        SoundManager.Instance.StopSound("BubbleLoop");
        circleHolder.transform.DOScale(0, 0.2f).SetEase(Ease.InBack);
        transform.DOScale(1, 1.5f).SetEase(Ease.OutQuad);
        boilingEffect.Stop();
        _paintingAnim.Kill();
        _paintingAnim2.Kill();
        _bucketShakeAnim.Kill();
        LevelSpecial.Instance.CheckObjectiveMatch(product);

        productTemp = product;
        
        productTemp.transform.DOScale(1.5f, 1.75f);
        productTemp.transform.DOLocalMove(new Vector3(10,0,-5), 1.75f).OnComplete(() =>
        {
            TimeManager.Instance.transform.DOMoveX(0, 0.3f).OnComplete(() =>
            {
                SoundManager.Instance.PlaySound("PoofCut", 0.15f);
                productTemp.transform.DOScale(0,0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Instantiate(GameManager.Instance.productToCoinFx, productTemp.transform.position, Quaternion.identity);

                    StartCoroutine(DelayedCoins());
                    productTemp.SelfDestroy(3);
                });
            });
        });
        
    }

    private IEnumerator DelayedCoins()
    {
        if (!productTemp.isCorrectProduct)
        {
            coinToCollect /= 2;
        }
        
        var randomAmount = Random.Range(3, 6);
        for (var i = 0; i < randomAmount; i++)
        {
            var coinObj = Instantiate(FinanceManager.Instance.coinObject);
            coinObj.transform.position = productTemp.transform.localPosition;

            coinObj.transform.localScale = Vector3.zero;
            coinObj.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);

            var randomDir = new Vector3(10 + Random.Range(-2f, 2f), Random.Range(-2f, 2f), -5 + Random.Range(-2f, 2f));
            coinObj.transform.DOLocalMove(randomDir, 0.2f).SetEase(Ease.OutQuad);
            
            TimeManager.Instance.transform.DOMoveX(0, 0.21f).OnComplete(() =>
            {
                coinObj.transform.DOMove(UIManager.Instance.coinPngWorldPos, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    Destroy(coinObj.gameObject);
                    FinanceManager.Instance.GainCoin(coinToCollect / randomAmount);
                });
            });

            if (i == randomAmount - 1)
            {
                LevelSpecial.Instance.CheckGameStatus();
            }
            
            yield return new WaitForSeconds(0.06f);
        }
      
    }

    private void SetPaintBucketStats()
    {
        foreach (var p in FinanceManager.Instance.products.Where(p => product.objectType.ToString() == p.product.ToString()))
        {
            workTime = p.coloringTime;
            coinToCollect = p.productCost;
        }

        workTimer = workTime;
    }

    private void CheckMachine()
    {
        if (LevelManager.Instance.currentLevelNumber >= requiredLevel)
        {
            ChangeStatus(true);
        }
        else
        {
            ChangeStatus(false);
        }
    }

    private void ChangeStatus(bool isUnlocked)
    {
        if (isUnlocked)
        {
            machineState = MachineState.Ready;
            lockedObjectsHolder.gameObject.SetActive(false);
        }
        else
        {
            machineState = MachineState.Locked;
            lockedObjectsHolder.gameObject.SetActive(true);
            requiredLevelText.text = "LEVEL" + "" + requiredLevel;
        }
    }
    
}
