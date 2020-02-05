using UnityEngine;
using System.Collections;

public class TouchTest : MonoBehaviour 
{
    void Update() 
	{
        int fingerCount = 0;
        foreach (Touch touch in Input.touches) 
		{
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;            
        }
		
        if (fingerCount > 0)
            print("User has " + fingerCount + " finger(s) touching the screen");        
    }
}