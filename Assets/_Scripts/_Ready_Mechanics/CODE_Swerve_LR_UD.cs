using UnityEngine;

public class CODE_Swerve_LR_UD : MonoBehaviour
{
     // Paste those variables to EX_GameManager

     //-- Mechanic Variables
     public float speedSideways;
     public float speedForward;
     
     private bool holding;

     private Vector3 pos1, pos2;

     public GameObject player;

     private Rigidbody playerRb;
     //-- Mechanic Variables
     
     void Start() // Paste the codes to EX_GameManager > Start
     {
         playerRb = player.GetComponent<Rigidbody>();
     }

     void Update() // Paste the codes to EX_GameManager > Update > STATE.Play
     {
         if (Input.GetMouseButtonDown(0))
         {
             pos1 = GameManager.Instance.GetMousePosition();

             holding = true;
         }

         if (Input.GetMouseButton(0) && holding) //set players velocity on X and Z axis
         {
             pos2 = GameManager.Instance.GetMousePosition();

             Vector3 delta = pos1 - pos2;

             pos1 = pos2;

             playerRb.velocity =
                 new Vector3(Mathf.Lerp(playerRb.velocity.x, -delta.x * speedSideways, 5f * Time.deltaTime),
                     playerRb.velocity.y, Mathf.Lerp(playerRb.velocity.y, -delta.y * speedForward, 5f * Time.deltaTime));
         }

         if (Input.GetMouseButtonUp(0))
         {
             holding = false;

             playerRb.velocity = Vector3.zero;
         }
     }
}
