using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayObject : MonoBehaviour
{
    public enum ObjectType
    {
        Fabric,
        Sock,
        Bra,
        Short,
        Slip
    }

    public ObjectType objectType;
    public GameObject productSprite;
    public bool isCorrectProduct;
    
    [HideInInspector] public Vector3 startingPos;
    [HideInInspector] public List<Transform> torusList = new List<Transform>();

    private void Start()
    {
        startingPos = transform.localPosition;
    }

    public void Selection()
    {
        if (objectType == ObjectType.Fabric)
        {
            StartCoroutine(DelayedSelectionBlop());
        }
        else
        {
            SoundManager.Instance.PlaySound("SelectionAlternative", 0.2f);
        }
      
    }

    IEnumerator DelayedSelectionBlop()
    {
        for (var i = 0; i < torusList.Count; i++)
        {
            var i1 = i;
            torusList[i].DOPunchScale(new Vector3(0.4f, 0.4f, 0.4f), 0.2f, 5).OnComplete(() =>
            {
                torusList[i1].DOScale(1, 0.2f);
            });
            
            SoundManager.Instance.PlaySoundSpecial("FabricBlopEffect", 0.1f);
            SoundManager.Instance.specialFloat += 0.05f;
            
            yield return new WaitForSeconds(0.035f);

            if (i == torusList.Count - 1)
            {
                foreach (var s in SoundManager.Instance._audioSources)
                {
                    s.pitch = 1;
                }
                SoundManager.Instance.specialFloat = 0f;
            }
        }
    }

    public void StartPosRemap()
    {
        startingPos = transform.localPosition;
    }

    public void SelfDestroy(float delay)
    {
        TimeManager.Instance.transform.DOMoveX(0, delay).OnComplete(() =>
        {
          Destroy(gameObject);
        });
    }
    
}
