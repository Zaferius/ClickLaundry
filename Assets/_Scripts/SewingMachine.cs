using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SewingMachine : MonoBehaviour
{
    public bool isActive;
    public int requiredLevel;
    public enum MachineType
    {
        Sock,
        Bra,
        Short,
        Slip
    }

    public MachineType machineType;

    public enum MachineState
    {
        Locked,
        Ready,
        Working,
        Done
    }

    public MachineState machineState;

    [HideInInspector] public List<Transform> sewingMachineFabrics = new List<Transform>();

    [Space(5)] 
    public GameObject doneToPaintProduct;
    public float workTime;
    public float workTimer;
    
    [Space(5)]
    public DOTweenAnimation fabricHolderAnim;
    
    [Header("+ Needle +")]
    public Transform needle;
    private float _needleYPos;
    public DOTweenAnimation needleAnim;

    [Header("+ Product +")] 
    public Transform displayProduct;
    public GameObject productButton;
    public Transform productCircles;
    public SpriteRenderer productInnerCircle;
    public SpriteRenderer productFillSprite;
    public DOTweenAnimation productWorkingSpriteAnim;
    public ParticleSystem productDoneSparkle;

    [Header("+ FX +")]
    public ParticleSystem featherFX;
    public ParticleSystem sewingDoneFx;
    
    
    [Header("+ Lock Side +")]
    public GameObject lockedObjectsHolder;
    public TextMeshPro requiredLevelText;

    private void Start()
    {
        SetMachineStats();
        CheckMachine();
        PrepareMachine();
    }
    private void Update()
    {
        if (!isActive) return;
        workTimer -= Time.deltaTime;
        
        if (needle.transform.localPosition.y < needleAnim.endValueV3.y + 0.05f)
        {
            featherFX.Play();
            SoundManager.Instance.PlaySound("SewPop", 0.05f);
        }

        if (!(workTimer <= 0)) return;
        isActive = false;
        workTimer = workTime;
        DeActivate();
    }
    private void PrepareMachine()
    {
        displayProduct.transform.localScale = Vector3.zero;
        _needleYPos = needle.transform.position.y;
        workTimer = workTime;
    }
    public void Activate()
    {
        StartCoroutine(DelayedActivation());
    }
    private void DeActivate()
    {
        needleAnim.DOPause();
        fabricHolderAnim.DOPause();
        needle.transform.DOMoveY(_needleYPos, 0.2f).SetEase(Ease.OutQuad);
        machineState = MachineState.Done;
        ProductReady();
    }
    private IEnumerator DelayedActivation()
    {
        FabricBlop();
        yield return new WaitForSeconds(0.2f);
        isActive = true;
        machineState = MachineState.Working;
        needleAnim.DOPlay();
        fabricHolderAnim.DOPlay();
        StartCoroutine(DelayedUsingFabric());
        displayProduct.transform.DOScale(1, workTime);

        productWorkingSpriteAnim.DOPlay();
        productFillSprite.material.DOFloat(0, "_Arc1", workTime).SetEase(Ease.Linear);
        
    }
    private void FabricBlop()
    {
        StartCoroutine(DelayedFabricBlop());
    }
    IEnumerator DelayedFabricBlop()
    {
        foreach (var t in sewingMachineFabrics)
        {
            t.transform.localScale = Vector3.one;
            t.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f, 5);
            
            yield return new WaitForSeconds(0.02f);
        }
    }
    private IEnumerator DelayedUsingFabric()
    {
        foreach (var t in sewingMachineFabrics)
        {
            yield return new WaitForSeconds(workTime / sewingMachineFabrics.Count);

            t.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear);
        }
    }
    private void ProductReady()
    {
        productWorkingSpriteAnim.DOPause();
        productInnerCircle.DOColor(ColorMaster.Instance.productSpriteInnerFlashColor, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetId("CircleColor" + transform.name);
        productCircles.transform.DOScale(0.85f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetId("CircleScale" + transform.name);
        productDoneSparkle.Play();
        sewingDoneFx.Play();
        SoundManager.Instance.PlaySound("SewingDone", 0.1f);
    }
    public void ResetMachine()
    {
        isActive = false;
        machineState = MachineState.Ready;
        DOTween.Pause("CircleColor" + transform.name);
        DOTween.Pause("CircleScale" + transform.name);
        productInnerCircle.DOColor(ColorMaster.Instance.productInnerCircleDefaultColor, 0.2f);
        productCircles.DOScale(0.8f, 0.2f);
        productFillSprite.material.DOFloat(360, "_Arc1", 0).SetEase(Ease.Linear);
    }
    private void SetMachineStats()
    {
        foreach (var p in FinanceManager.Instance.products.Where(p => machineType.ToString() == p.product.ToString()))
        {
            workTime = p.sewTime;
        }
    }
    private void CheckMachine()
    {
        ChangeStatus(LevelManager.Instance.currentLevelNumber >= requiredLevel);
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
