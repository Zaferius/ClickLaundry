using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelSpecial : MonoBehaviour
{
    #region Instance Method
    public static LevelSpecial Instance;
    
    [Range(0,5)]
    public int startingFabric;
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
    
    [System.Serializable]
    public class Objective
    {
        public Color color;
        public enum Product
        {
            Sock,
            Bra,
            Short,
            Slip
        }

        public Product product;
        [HideInInspector] public GameObject objectiveObject;
    }
    
    
    public List<Objective> objectives = new List<Objective>();
  
    public int fabricCost;

    private void Awake()
    {
        InstanceMethod();
    }

    void Start()
    {
        CreateObjectiveTable();
        PrepareFabrics();
    }

    private void PrepareFabrics()
    {
        for (var i = 0; i < startingFabric; i++)
        {
            var fabric = Instantiate(GameManager.Instance.fabricProduct, new Vector3(0,15,0), Quaternion.identity);
            var transform1 = fabric.transform;
            transform1.gameObject.layer = 0;
            transform1.parent = GameManager.Instance.sewingAreaLayoutOrganizer.transform;
            fabric.startingPos = transform1.localPosition;
            transform1.gameObject.layer = 6;
        }

        GameManager.Instance.sewingAreaLayoutOrganizer.ArrangeChildrenHorizontallyFast();
    }

    private void CreateObjectiveTable()
    {
        foreach (var o in objectives)
        {
            foreach (var p in GameManager.Instance.products)
            {
                if (o.product.ToString() == p.objectType.ToString())
                {
                    var t = Instantiate(p.productSprite, UIManager.Instance.objectiveSliderParent, true);
                    o.objectiveObject = t;
                    t.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                    t.transform.GetChild(0).GetComponent<Image>().color = o.color;
                }
            }
        }
    }

    private static float ColorSimilarityCalculation(Color color1, Color color2)
    {
        var red = color1.r - color2.r;
        var green = color1.g - color2.g;
        var blue = color1.b - color2.b;
        var result = Mathf.Sqrt(red * red + green * green + blue * blue);
        return result;
    }

    public void CheckGameStatus()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.Play) return;
        if (objectives.Count == 0)
        {
            GameManager.Instance.GameWin();
        }
    }
    public void CheckObjectiveMatch(PlayObject product)
    {
        for (var i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].product.ToString() != product.objectType.ToString()) continue;
            
            var calc = ColorSimilarityCalculation(objectives[i].color, product.transform.GetChild(0).GetComponent<MeshRenderer>().material.color);

            if (!(calc <= 0.45f)) continue;
            
            SoundManager.Instance.PlaySound("CorrectItem", 0.3f);
            product.transform.GetChild(2).gameObject.SetActive(true);
            product.isCorrectProduct = true;
            
            objectives[i].objectiveObject.transform.DOScale(0,0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Destroy(objectives[i].objectiveObject);
                objectives.Remove(objectives[i]);
            });

            break;
        }
    }
}
