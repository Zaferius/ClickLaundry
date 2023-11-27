using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Instance Method

    public static TimeManager Instance;


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

    private void Awake()
    {
        #region Instance Method

        InstanceMethod();

        #endregion

    }
    
    /*
     TimeManager.Instance.transform.DOMoveX(0, 0.1f).OnComplete(() => { clutchText.GetComponent<Text>().text = "3"; clutch += 1; });
     */
    
    /*
      Camera.main.DOShakePosition(0.5f, 0.15f);
     */
}
