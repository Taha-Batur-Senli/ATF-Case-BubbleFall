using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class checkUI : MonoBehaviour
{
    [SerializeField] private gameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //gameManager.canShoot = true;
        }
        else
        {
            //gameManager.canShoot = false;
        }
    }

    private void OnMouseUp()
    {
        
    }
}