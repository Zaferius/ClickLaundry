using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Instance Method
    public static UIManager Instance;
    private void InstanceMethod()
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
    
    #endregion

    [Header("+ Objective Panel + ")]
    public Transform objectiveSliderParent;

    [Header("+ Coin + ")]
    public GameObject coinHolder;
    public TextMeshProUGUI coinText;
    public Vector3 coinPngWorldPos;
    [Header("+ Paint Button + ")] 
    public Button paintButton;
    public List<TextMeshProUGUI> letters = new List<TextMeshProUGUI>();
    public Vector3 paintButtonWorldPos;
    [Header("+ Sew + ")] 
    public Button sewButton;
    [Header("+ Buy Fabric + ")] 
    public Button buyFabricButton;
    public TextMeshProUGUI fabricCostText;
    #region Constant
    [HideInInspector]public TextMeshProUGUI levelIndex;
    [HideInInspector]public GameObject levelComplete,levelFailed,confetti;
    [HideInInspector]public List<string> winTexts = new List<string>();
    [HideInInspector]public List<string> failTexts = new List<string>();
    [HideInInspector]public TextMeshProUGUI failText,winText;
    #endregion
    private void Awake()
    {
        #region Instance Method
        InstanceMethod();
        #endregion
    }

    public void _GameStart()
    { 
        paintButtonWorldPos = GameManager.Instance.cam.ScreenToWorldPoint(paintButton.transform.position);

        levelIndex.enabled = true;
        coinHolder.SetActive(true);
        GameManager.Instance.gameState = GameManager.GameState.Play;
        
        paintButton.onClick.AddListener(ToPaintArea);
        sewButton.onClick.AddListener(ToSewArea);
        buyFabricButton.onClick.AddListener(GameManager.Instance.BuyFabric);
    }

    public void _GameWin()
    {
        levelIndex.enabled = false;

        winText.text = winTexts[Random.Range(0, winTexts.Count)];
        
        levelComplete.SetActive(true);
        confetti.SetActive(true);
    }

    public void _GameLose()
    {
        levelIndex.enabled = false;
        
        failText.text = failTexts[Random.Range(0, failTexts.Count)];
        
        levelFailed.SetActive(true);
    }
    
    public void SetLevelIndex()
    {
        levelIndex.text = "Level " + LevelManager.Instance.currentLevelNumber;
    }

    public IEnumerator ColorizePaintButtonC(bool isTrue)
    {
        if (isTrue)
        {
            for (var i = 0; i < letters.Count; i++)
            {
                letters[i].transform.DOPunchScale(new Vector3(0.2f,0.2f,0.2f), 0.2f);
                letters[i].DOColor(ColorMaster.Instance.paintButtonColors[i], 0.2f);
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            foreach (var t in letters)
            {
                t.DOColor(Color.white, 0.2f);
            }
        }
       
    }

    private void ToPaintArea()
    {
        GameManager.Instance.cam.transform.parent.DOMoveX(10, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            if (coinPngWorldPos == Vector3.zero)
            {
                coinPngWorldPos = GameManager.Instance.cam.ScreenToWorldPoint(coinHolder.transform.GetChild(0).position);
            }
        });

        paintButton.transform.parent.gameObject.SetActive(false);
        sewButton.transform.parent.gameObject.SetActive(true);
        buyFabricButton.transform.parent.gameObject.SetActive(false);
        
        SoundManager.Instance.PlaySound("Whoosh1", 0.4f);
    }
    
    private void ToSewArea()
    {
        GameManager.Instance.cam.transform.parent.DOMoveX(0, 0.25f).SetEase(Ease.OutBack);
        sewButton.transform.parent.gameObject.SetActive(false);
        paintButton.transform.parent.gameObject.SetActive(true);
        buyFabricButton.transform.parent.gameObject.SetActive(true);
        
        SoundManager.Instance.PlaySound("Whoosh2", 0.4f);
    }
}
