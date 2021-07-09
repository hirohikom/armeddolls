using UnityEngine;
using System.Collections;

public class MyFirstTouch : MonoBehaviour {

	// Subscribe to events
	void OnEnable(){
		EasyTouch.On_TouchStart += On_TouchStart;
	}
	// Unsubscribe
	void OnDisable(){
		EasyTouch.On_TouchStart -= On_TouchStart;
	}
	// Unsubscribe
	void OnDestroy(){
		EasyTouch.On_TouchStart -= On_TouchStart;
	}
	// Touch start event
	public void On_TouchStart(Gesture gesture){
//		Debug.Log( "Touch in " + gesture.position);
	}
}
