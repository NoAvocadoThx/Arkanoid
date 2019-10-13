using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For creating the corners of edge collider according to the screen size
public class BoxEdge : MonoBehaviour
{
    public Camera cam;
    public EdgeCollider2D col;
    // Start is called before the first frame update
    void Start()
    {
        //set 4 corners of the box aka edge colliders
        Vector3 botLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 botRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        Vector3 upLeft = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        Vector3 upRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        col.points = new Vector2[5] { botLeft, upLeft, upRight, botRight, botLeft };
    }

}
