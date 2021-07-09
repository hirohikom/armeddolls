using UnityEngine;
using System.Collections;


public class LaboUnselectedCamera : MonoBehaviour {
	
	public Vector3 CameraPivotPosition = new Vector3(0, 0.75f, 0);
	public float StartHeight = 2.0f;
	public float StartDistanse = 30.0f;
	public float MouseSmoothTime = 2.0f;
	public float DistanceSmoothTime = 0.8f;

	private Vector3 _desiredPosition;
	private float _desiredDistance;
	private float _distanceSmooth = 0;
	private float _distanceCurrentVelocity;
	public static float _mouseX = 0;
	private float _mouseXSmooth = 0;
	private float _mouseY = 0;
	private float _mouseYSmooth = 0;

	private Transform CameraTrfm;
	
	public float UnselectedDistance = 8.5f;
	public float UnselectedHeight = 2.0f;

	public static float MovingAngle = 45.0f;

    public static int PosNumber = 0;
	
	
	private void Awake() 
	{
		CameraTrfm = Camera.main.gameObject.transform;
		CameraTrfm.position = new Vector3 (0, StartHeight, StartDistanse);
		CameraTrfm.LookAt (CameraPivotPosition);
		_mouseYSmooth = StartHeight;
		_mouseXSmooth = 0;
		_distanceSmooth = StartDistanse;
	}
	
	private void Start() 
	{
		ResetView ();
	}

	private void Update()
	{
		//ジョイスティックによる視点操作
 /*		if (inputDevice.DPadLeft.WasPressed)
        {
            PosNumber++;
            if (PosNumber > 7)
                PosNumber = 0;
               _mouseX = MovingAngle * PosNumber;
        }
        else if (inputDevice.DPadRight.WasPressed)
        {
            PosNumber--;
            if (PosNumber < 0)
                PosNumber = 7;
            _mouseX = MovingAngle * PosNumber;
        }*/

        //キーボードによる視点操作
		if (Input.GetKeyDown (KeyCode.LeftArrow))
        {
            PosNumber++;
            if (PosNumber > 7)
                PosNumber = 0;
            _mouseX = MovingAngle * PosNumber;
        }
		else if (Input.GetKeyDown (KeyCode.RightArrow))
        {
            PosNumber--;
            if (PosNumber < 0)
                PosNumber = 7;
            _mouseX = MovingAngle * PosNumber;
        }
    }

    private void LateUpdate()
    {
		_mouseXSmooth = Mathf.MoveTowardsAngle(_mouseXSmooth, _mouseX, MouseSmoothTime);
		_mouseYSmooth = Mathf.MoveTowardsAngle(_mouseYSmooth, _mouseY, MouseSmoothTime);

		Vector3 newCameraPosition;
		// Compute the desired position
		_desiredPosition = GetCameraPosition(_mouseYSmooth, _mouseXSmooth, _desiredDistance);
		// The camera is not constricted (anymore) so smooth the distance change
		_distanceSmooth = Mathf.SmoothDamp(_distanceSmooth, _desiredDistance, ref _distanceCurrentVelocity, DistanceSmoothTime);
		// Compute the new camera position
		newCameraPosition = GetCameraPosition(_mouseYSmooth, _mouseXSmooth, _distanceSmooth);
		
		CameraTrfm.position = newCameraPosition;
		// Check if we are in third or first person and adjust the camera rotation behavior
		CameraTrfm.LookAt(CameraPivotPosition);
	}
	
	private Vector3 GetCameraPosition(float xAxisDegrees, float yAxisDegrees, float distance) 
	{
		Vector3 offset = new Vector3(0, 0, -distance);
	
		Quaternion rotation = Quaternion.Euler(xAxisDegrees, yAxisDegrees, 0);
		
		return CameraPivotPosition + rotation * offset;
	}
	
	public void ResetView() 
	{
		_mouseY = UnselectedHeight;

		_desiredDistance = UnselectedDistance;
	}
	
}
