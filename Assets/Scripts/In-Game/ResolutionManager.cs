using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResolutionManager : MonoBehaviour
{

    public Camera GameCamera;

    void Awake() {
        GameCamera.GetComponent<PixelPerfectCamera>().refResolutionX = Screen.width;
        GameCamera.GetComponent<PixelPerfectCamera>().refResolutionY = Screen.height;
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
