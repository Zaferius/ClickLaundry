using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Instance Method / GameState
    public static GameManager Instance;
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
    
    public enum GameState
    {
        Play,
        Pause,
        Win,
        Lose,
        StartMenu,
    }
    public GameState gameState;
    
    #endregion


    public enum GameScreen
    {
        SewingArea,
        PaintingArea
    }

    public GameScreen gameScreen;
    
    [Header("+ General  +")]
    public int readyToPaint;
    [Space(5)] 

    public float selectedObjectHeight;

    private float defaultHeight;

    public GameObject selectedObject;

    public LayerMask dragObjectsLayer, groundsLayer, machineLayer, paintBucketLayer;

    public bool holding;

    private Vector3 pos1;

    public float dragSpeed;

    [HideInInspector] public Camera cam;

    [Header("+ Products +")]
    public PlayObject fabricProduct;
    public List<PlayObject> products = new List<PlayObject>();
    public ParticleSystem productToCoinFx;

    [Header("+ Layout Organizers +")] 
    public LayoutOrganizer sewingAreaLayoutOrganizer;
    public LayoutOrganizer paintingAreaLayoutOrganizer;

    [Header("+ Selection Effects +")]
    public float rotationTiltSpeed;
    public float rotationTiltAmount;
    [Space(5)] 
    [Range(0.05f,1)]
    public float scaleTiltSmoothness;
    public float scaleTiltAmount;
    private void Awake()
    {
        #region Instance Method
        InstanceMethod();
        #endregion
        
    }

    private void Start()
    {
        cam = Camera.main;
        Application.targetFrameRate = 120;
    }

    private void Update()
    {
        if (gameState == GameState.Play)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, dragObjectsLayer))
                {
                    selectedObject = hit.transform.gameObject;

                    defaultHeight = selectedObject.transform.position.y;

                    holding = true;
                    
                    selectedObject.GetComponent<PlayObject>().Selection();
                }
            }

            if (Input.GetMouseButton(0) && holding && selectedObject != null) 
            {
                RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundsLayer))
                {
                    pos1 = hit.point;

                    selectedObject.transform.position = Vector3.Lerp(selectedObject.transform.position, new Vector3(pos1.x,pos1.y, -3), Time.deltaTime * dragSpeed);

                    var rotation = Mathf.Sin(Time.time * rotationTiltSpeed) * rotationTiltAmount;
                    selectedObject.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
                    
                    var targetScale = Mathf.PingPong(Time.time * scaleTiltAmount, scaleTiltAmount) + 1;
                    var smoothedScale = Mathf.Lerp(selectedObject.transform.localScale.x, targetScale, scaleTiltSmoothness * Time.deltaTime);
                    selectedObject.transform.localScale = new Vector3(smoothedScale, smoothedScale, smoothedScale);
                }
            }
            
            if (Input.GetMouseButtonUp(0) && holding && selectedObject != null) 
            {
                Vector3 dropPosition = selectedObject.transform.position;

                dropPosition.y = defaultHeight;

                if (selectedObject.GetComponent<PlayObject>().objectType == PlayObject.ObjectType.Fabric)
                {
                    if (Physics.Raycast(selectedObject.transform.position, selectedObject.transform.forward, out var hit, Mathf.Infinity, machineLayer))
                    {
                        if (hit.collider != null)
                        {
                            var sewingMachine = hit.transform.GetComponent<SewingMachine>();

                            if (sewingMachine.machineState == SewingMachine.MachineState.Ready )
                            {
                                sewingMachine.Activate();
                                Destroy(selectedObject);
                                selectedObject = null;
                                
                                TimeManager.Instance.transform.DOMoveX(0, 0.1f).OnComplete(() =>
                                {
                                    sewingAreaLayoutOrganizer.ArrangeChildrenHorizontally();
                                });
                            }
                            else
                            {
                                ObjectDenied();
                            }
                        }
                    } 
                    else
                    {
                        ObjectDenied();
                    }
                }
                else
                {
                    if (Physics.Raycast(selectedObject.transform.position, selectedObject.transform.forward, out var hit, Mathf.Infinity, paintBucketLayer))
                    {
                        if (hit.collider != null)
                        {
                            var paintBucket = hit.transform.GetComponent<PaintBucket>();

                            if (paintBucket.machineState == PaintBucket.MachineState.Ready)
                            {
                                readyToPaint--;
                                if (readyToPaint <= 0)
                                {
                                    UIManager.Instance.StartCoroutine(UIManager.Instance.ColorizePaintButtonC(false));
                                }
                                paintBucket.ToBucket(selectedObject.GetComponent<PlayObject>());
                                TimeManager.Instance.transform.DOMoveX(0, 0.1f).OnComplete(() =>
                                {
                                    paintingAreaLayoutOrganizer.ArrangeChildrenHorizontally();
                                });
                            }
                            else
                            {
                                ObjectDenied();
                            }
                        }
                        else
                        {
                            ObjectDenied();
                        }
                    }
                    else
                    {
                        ObjectDenied();
                    }
                }
                holding = false;
            }
        }
    }

    private void ObjectDenied()
    {
        selectedObject.transform.rotation = Quaternion.Euler(0f, 0f, 0);

        selectedObject.transform.DOLocalMove(selectedObject.GetComponent<PlayObject>().startingPos, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            selectedObject.transform.DOScale(1, 0.1f);
            selectedObject.transform.eulerAngles = Vector3.zero;
            selectedObject = null;
        });
    }

    public void BuyFabric()
    {
        if (FinanceManager.Instance.totalCoin >= LevelSpecial.Instance.fabricCost && sewingAreaLayoutOrganizer.transform.childCount <= 4) 
        {
            SoundManager.Instance.PlaySound("BuyFabric", 0.1f);
            FinanceManager.Instance.GainCoin(-LevelSpecial.Instance.fabricCost);
            var fabric = Instantiate(fabricProduct, new Vector3(0,15,0), Quaternion.identity);
            fabric.transform.gameObject.layer = 0;

            fabric.transform.parent = sewingAreaLayoutOrganizer.transform;
        
            TimeManager.Instance.transform.DOMoveX(0, 0.05f).OnComplete(() =>
            {
                sewingAreaLayoutOrganizer.ArrangeChildrenHorizontally();
                TimeManager.Instance.transform.DOMoveX(0, 0.2f).OnComplete(() =>
                {
                    fabric.startingPos = fabric.transform.localPosition;
                    fabric.transform.gameObject.layer = 6;
                });
            });
        }
    }

    #region Win/Lose
    
    public void GameWin()
    {
        gameState = GameState.Win;
        
        TimeManager.Instance.transform.DOMoveX(0, 1.75f).OnComplete(() =>
        {
            UIManager.Instance._GameWin();

            var randomClip = SoundManager.Instance.soundClips.Where(c => c.ToString().Contains("levelComplete")).ToList();
            SoundManager.Instance.PlaySound(randomClip[Random.Range(0,randomClip.Count)]);
        });
        
      
    }

    public void GameLose()
    {
        gameState = GameState.Lose;
        ///////////////////////////
        UIManager.Instance._GameLose();
    }
    #endregion
    
    #region Constant Methods
    
    public static float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        var inverse = false;
        var timing = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = true;
            timing -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > timing : tangle < timing;
        if (!result)
            angle = min;
        inverse = false;
        tangle = angle;
        var tax = max;
        if (angle > 180)
        {
            inverse = true;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tax -= 180;
        }
        result = !inverse ? tangle < tax : tangle > tax;
        if (!result)
            angle = max;
        return angle;
    }
    
    public Vector2 GetMousePosition()
    {
        var pos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

        return pos;
    }
    
    #endregion
}
