//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Interact : MonoBehaviour
//{
//    public LayerMask playerLayer;
//    public Crosshair crosshair;
//    public int interactableType;
//    public Interactable interactable;

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //float interactRay = Vector3.Distance(Camera.main.transform.position, playerPos.transform.position);
//        // Look at trigger raycast:
//        RaycastHit hit;
//        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 35, Color.red);
//        // Debug.DrawLine(Camera.main.transform.position, playerPos.transform.position + playerPos.transform.forward * 5, Color.red);
//        // Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 35, Color.red);
//        // if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 35, playerLayer))
//        // if (Physics.Raycast(Camera.main.transform.position, playerPos.transform.forward, out hit, interactRay + 10, playerLayer)) // 35 -> interactRay + 5
//        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 35, playerLayer))
//        {
//            if (hit.collider.GetComponent<Interactable>())
//            {
//                interactable = hit.collider.GetComponent<Interactable>();

//                // Change crosshair and interaction text
//                crosshair.HoverCrosshair();

//                // SWITCH CASE HERE

//                if (hit.collider.CompareTag("Item"))
//                    crosshair.HoverText("Pickup " + interactable.GetName().ToString());
//                else if (hit.collider.CompareTag("NPC"))
//                {
//                    crosshair.GetComponent<Crosshair>().HoverText("(Press " + inputManager.ReturnKeyInfo(5).ToString() + " to talk)");
//                }
//                else if (hit.collider.CompareTag("DataShard"))
//                {
//                    crosshair.GetComponent<Crosshair>().HoverText("Pickup " + hit.collider.gameObject.name + "\n(Press " + inputManager.ReturnKeyInfo(5).ToString() + ")");
//                }
//                else if (hit.collider.CompareTag("HackedTower"))
//                {
//                    crosshair.GetComponent<Crosshair>().HoverText("Fix " + hit.collider.gameObject.name + "\n(Press " + inputManager.ReturnKeyInfo(5).ToString() + ")");
//                }
//                else if (hit.collider.CompareTag("Captive"))
//                {
//                    crosshair.GetComponent<Crosshair>().HoverText("Rescue " + hit.collider.gameObject.name + "\n(Press " + inputManager.ReturnKeyInfo(5).ToString() + ")");
//                }
//                else crosshair.GetComponent<Crosshair>().HoverText(hit.collider.gameObject.name);

//                if (inputManager.GetKeyDown(KeyActions.Use))
//                {
//                    // Send the NPC info that you are the active player aka myCharacter
//                    if (hit.collider.gameObject.GetComponent<QuestNpcTest>() != null)
//                    {
//                        hit.collider.gameObject.GetComponent<QuestNpcTest>().player = gameObject;
//                        questHandler.interactedNPC = hit.collider.gameObject.GetComponent<QuestNpcTest>().npcNumber;
//                        questHandler.player = gameObject;
//                    }


//                    hit.collider.GetComponent<Trigger>().Pressed(gameObject);

//                    // Jos kert‰t‰‰n magic bottle! T‰n vois ehk‰ laittaa omaan skriptiin kuitenkin, niinkun oli aluksi
//                    if (hit.collider.gameObject.name == "Bottle")
//                    {
//                        shrekMagicBottle = 1;
//                    }
//                    /*
//                    if(hit.collider.gameObject.CompareTag("WeaponShard"))
//                    {
//                        startingAreaQuest1Collectables++;
//                    }
//                    */
//                }
//            }
//            else // No trigger, change to normal crosshair and no text
//            {
//                crosshair.ResetText();
//                crosshair.NormalCrosshair();
//            }
//        }
//        else // No trigger, change to normal crosshair and no text
//        {
//            if (crosshair != null)
//            {
//                crosshair.ResetText();
//                crosshair.NormalCrosshair();
//            }
//        }
//    }
//}
