using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WinConditionIndicator : MonoBehaviour
{

    public string winTag = "WinObject";

    private BoxCollider winBox;
    private HashSet<Collider> winObjects;

    public bool isWinning
    {
        get {
            return winObjects.Count > 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        winBox = this.GetComponent<BoxCollider>();
        winObjects = new HashSet<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //called when something enters the trigger
    public void OnTriggerEnter(Collider other)
    {
        //if the object is not already in the list
        if (other.gameObject.CompareTag(this.winTag) && !winObjects.Contains(other))
        {
            winObjects.Add(other);
        }
    }

    //called when something exits the trigger
    public void OnTriggerExit(Collider other)
    {
        //if the object is in the list
        if (winObjects.Contains(other))
        {
            winObjects.Remove(other);
        }
    }
}
