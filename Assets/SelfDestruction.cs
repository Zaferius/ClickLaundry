using DG.Tweening;
using UnityEngine;

public class SelfDestruction : MonoBehaviour
{
    public float delay;
    private void Start()
    {
        TimeManager.Instance.transform.DOMoveX(0, delay).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    
}
