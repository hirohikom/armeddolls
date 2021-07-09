using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DrawCrosshair : MonoBehaviour {

	private GameObject AimObject;
	private Vector3 AimPoint;
	private Vector3 offset = new Vector3 (0, 0.8f, 0);
	private Vector3 cPos;
    public Image crosshairImge;
    private Camera uiCamera;
    private Vector2 localPos;
    RectTransform rectTransform;

	void Start ()
	{
        crosshairImge.enabled = false;
        uiCamera = GetComponent<Canvas>().worldCamera;
        rectTransform = GetComponent<RectTransform>();
	}

	void Update () {

		AimObject = AimingTarget.CurrentAimedObject;

		if (AimObject) 
		{
            AimPoint = AimObject.transform.position + offset;

			cPos = Camera.main.WorldToScreenPoint (AimPoint);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, cPos, uiCamera, out localPos);

            crosshairImge.enabled = true;
            
            crosshairImge.rectTransform.localPosition = localPos;
        }
		else
		{
			crosshairImge.enabled = false;
		}
	}
}
