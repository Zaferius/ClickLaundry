using System;
using UnityEngine;


public class CODE_DragAndDrop : MonoBehaviour
{
    // Paste those variables to GameManager

    //-- Mechanic Variables
    public float selectedObjectHeight;

    private float defaultHeight;

    private GameObject selectedObject;

    public LayerMask dragObjectsLayer, groundsLayer;

    private bool holding;

    private Vector3 pos1;
    //-- Mechanic Variables

    // Paste the codes to GameManager > Update > STATE.Play
    void Update() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, dragObjectsLayer)
            ) // if raycast hits gameObject select object & save y position
            {
                selectedObject = hit.transform.gameObject;

                defaultHeight = selectedObject.transform.position.y;

                holding = true;
            }
        }

        if (Input.GetMouseButton(0) && holding) // set objects position to mouse position and give y offset
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundsLayer))
            {
                pos1 = hit.point;

                pos1.y = selectedObjectHeight;

                selectedObject.transform.position = pos1;
            }
        }

        if (Input.GetMouseButtonUp(0) && holding) // Drop selected object & set selected object null
        {
            Vector3 dropPosition = selectedObject.transform.position;

            dropPosition.y = defaultHeight;

            selectedObject.transform.position = dropPosition;

            selectedObject = null;

            holding = false;
        }
    }
}
