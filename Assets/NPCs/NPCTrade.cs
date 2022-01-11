using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCTrade : MonoBehaviour
{
    [SerializeField] ItemObject wanted;
    [SerializeField] int wantedQuantity;
    [SerializeField] int currentQuantity;

    [SerializeField] GameObject drop;

    [SerializeField] Canvas dialogBox;
    [SerializeField] TextMeshProUGUI dialog;

    private void OnTriggerStay(Collider other)
    {
        if (wanted == null) return;
        if (other.CompareTag("Item") && currentQuantity < wantedQuantity)
        {
            if (!other.isTrigger && other.GetComponent<ItemContainer>().itemObject == wanted)
            {
                Destroy(other.gameObject);
                currentQuantity++;

                if (currentQuantity >= wantedQuantity) { 
                    Instantiate(drop, GameObject.FindGameObjectWithTag("Player").transform.position + Vector3.up, Quaternion.identity);
                    dialog.text = "Thanks.";
                }
                else
                {
                    dialog.text = "I still need <color=\"red\">" + (wantedQuantity - currentQuantity).ToString() + "</color> more.";
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) dialogBox.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) dialogBox.gameObject.SetActive(false);
    }
}
