using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class LaboSelectedCamera : MonoBehaviour {

	public static GameObject[] TargetArray;
	
	public Vector3 CameraPivotPosition = new Vector3(0, 1.0f, 0);
	public bool AlignCameraWhenMoving = true;
	public float TouchXSensitivity = 0.6f;
	public float TouchYSensitivity = 0.6f;
	public float RotateSensitivity = 0.6f;
	public float PinchSensitivity = 0.02f;
	public float MouseXSensitivity = 12.0f;
	public float MouseYSensitivity = 12.0f;
	public float KeyHorizontalSensitivity = 14.0f;
	public float KeyVirticalSensitivity = 0.6f;
	public float RightstickXSensitivity = 20.0f;
	public float RightstickYSensitivity = 1.0f;
	public float MouseYMin = -30.0f;
	public float MouseYMax = 60.0f;
	public float MouseScrollSensitivity = 6.0f;
	public float MouseSmoothTime = 0.08f;
	public float DpadSensitivity = 0.03f;
	public float MaxDistance = 30.0f;
	public float DistanceSmoothTime = 0.7f;
	public float RiseDistance = 2.0f;
	public float MinimumDistance = 0.25f;
	public bool InvertMouse = true;

	public static bool onTouch = false;
	public static bool onMoveView = false;
	public static bool onPlayer = false;

	//private Vector3 _cameraPivotPosition;
	private Vector3 _desiredPosition;
	private float _desiredDistance = 2.5f;
	private float _distanceSmooth = 8.5f;
	private float _distanceCurrentVelocity;
	private bool _alignCameraWithCharacter = false;
	public static float _mouseX = 0;
	public static float _mouseXSmooth = 0;
	private float _mouseXCurrentVelocity;
	private float _mouseY = 0;
	public static float _mouseYSmooth = 0;
	private float _mouseYCurrentVelocity;
	public static float _desiredMouseY = 0;

	private Transform PlayerTrfm;
	private Transform ArmLTrfm;
	private Transform ArmRTrfm;
	private Transform CameraTrfm;

	private float mouseYMinLimit;
	private float closestDistance;
	private float lookUpDegrees;
	private float characterRotation;
	private float cx, cz, actualDistance, horizontalDistance;

	private Ray ray;
	private RaycastHit rht;


	private void Start()
	{
		CameraTrfm = Camera.main.transform;
		// Check if there is a main camera in the scene to use
		if (Camera.main == null) 
		{
			GameObject mainCamera = new GameObject ("Main Camera");
			mainCamera.AddComponent<Camera> ();
			mainCamera.tag = "MainCamera";
		}

		TargetArray = null;
		
		TargetArray =
			GameObject.FindGameObjectsWithTag ("Player")
				.OrderBy (g => Vector3.Distance (g.transform.position, CameraTrfm.position))
				.ToArray ();
		
		PlayerTrfm = TargetArray [0].transform;
		CameraPivotPosition = PlayerTrfm.position + new Vector3(0, 1.0f, 0);
		CameraTrfm.LookAt (CameraPivotPosition);
	}

	void OnEnable(){
		EasyTouch.On_TouchStart += On_TouchStart;
		EasyTouch.On_TouchUp += On_TouchUp;
		EasyTouch.On_SwipeStart += On_SwipeStart;
		EasyTouch.On_Swipe += On_Swipe;
		EasyTouch.On_SwipeEnd += On_SwipeEnd;
		_mouseXSmooth = 0;
		_mouseYSmooth = 0;
	}
	
	void OnDisable(){
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}

	void UnsubscribeEvent(){
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchUp -= On_TouchUp;
		EasyTouch.On_SwipeStart -= On_SwipeStart;
		EasyTouch.On_Swipe -= On_Swipe;
		EasyTouch.On_SwipeEnd -= On_SwipeEnd;
		_mouseXSmooth = 0;
		_mouseYSmooth = 0;
	}

	private void On_TouchStart(Gesture gesture){
		onTouch = true;
	}

	private void On_TouchUp(Gesture gesture){
		onTouch = false;
	}

	private void On_SwipeStart(Gesture gesture){
		onMoveView = true;
		onTouch = true;
		ray = Camera.main.ScreenPointToRay (gesture.position);
		if (Physics.Raycast (ray, out rht, 100)) {
			if (rht.collider.tag == "Player") {
				onPlayer = true;
			}
		}
	}

	private void On_Swipe(Gesture gesture){
		onMoveView = true;
		if (onPlayer) 
		{
			_mouseX += gesture.deltaPosition.x * RotateSensitivity;
			float _x = gesture.deltaPosition.x * RotateSensitivity;
			float _y = gesture.deltaPosition.y * RotateSensitivity;
			PlayerTrfm.Rotate(0, _x, 0);
			ArmLTrfm.Rotate(_y, 0, 0);
			ArmRTrfm.Rotate(_y, 0, 0);
		}
		else
		{
			_mouseX += gesture.deltaPosition.x * TouchXSensitivity;
			if (InvertMouse)
				_desiredMouseY -= gesture.deltaPosition.y * TouchYSensitivity;
			else
				_desiredMouseY += gesture.deltaPosition.y * TouchYSensitivity;
		}
	}

	private void On_SwipeEnd(Gesture gesture){
		onMoveView = false;
		onPlayer = false;
		onTouch = false;
	}


	private void LateUpdate() 
	{
		if (TargetArray == null)
        {

		TargetArray =
			GameObject.FindGameObjectsWithTag ("Player")
				.OrderBy (g => Vector3.Distance (g.transform.position, CameraTrfm.position))
				.ToArray ();

		PlayerTrfm = TargetArray [0].transform;
		CameraPivotPosition = PlayerTrfm.position + new Vector3(0, 1.0f, 0);
        }
		CameraTrfm.LookAt (CameraPivotPosition);

		// Check if the camera lies on the ground
		bool mouseYConstrained = Physics.Linecast(CameraTrfm.position,
		                                          CameraTrfm.position + Vector3.down);
		mouseYConstrained = mouseYConstrained && CameraTrfm.position.y < CameraPivotPosition.y;

		mouseYMinLimit = _mouseY;

		if (!onTouch){
			//マウスによる視点操作
/*			if (Input.GetMouseButton(0)) {
				onMoveView = true;
				_mouseX += Input.GetAxis("Mouse X") * MouseXSensitivity;
				if (InvertMouse)
					_desiredMouseY -= Input.GetAxis("Mouse Y") * MouseYSensitivity;
				else
					_desiredMouseY += Input.GetAxis("Mouse Y") * MouseYSensitivity;
			} else {*/
				onMoveView = false;
				//ジョイスティックによる視点操作
/*				_mouseX = _mouseXSmooth + inputDevice.RightStick.X * RightstickXSensitivity;
				if (InvertMouse)
					_desiredMouseY -= inputDevice.RightStick.Y * RightstickYSensitivity;
				else
					_desiredMouseY += inputDevice.RightStick.Y * RightstickYSensitivity;*/
//			}
		}
		
		//キーボードによる視点操作
		if (Input.GetKey(KeyCode.RightArrow))
			_mouseX = _mouseXSmooth + KeyHorizontalSensitivity;
		if (Input.GetKey(KeyCode.LeftArrow))
			_mouseX = _mouseXSmooth - KeyHorizontalSensitivity;
		if (InvertMouse){
			if (Input.GetKey(KeyCode.UpArrow))
				_desiredMouseY -= KeyVirticalSensitivity;
			if (Input.GetKey(KeyCode.DownArrow))
				_desiredMouseY += KeyVirticalSensitivity;
		} else {
			if (Input.GetKey(KeyCode.UpArrow))
				_desiredMouseY += KeyVirticalSensitivity;
			if (Input.GetKey(KeyCode.DownArrow))
				_desiredMouseY -= KeyVirticalSensitivity;
		}
		

		if (mouseYConstrained) {
			_mouseY = Mathf.Clamp(_desiredMouseY, Mathf.Max(mouseYMinLimit, MouseYMin), MouseYMax);
			_desiredMouseY = Mathf.Max(_desiredMouseY, _mouseY - 90.0f);
		} else {
			_mouseY = Mathf.Clamp(_desiredMouseY, MouseYMin, MouseYMax);
			_desiredMouseY = Mathf.Min(_desiredMouseY, MouseYMax);
		}

/*		if (inputDevice.DPad.Up){
			_desiredDistance -= DpadSensitivity;
		} else if (inputDevice.DPad.Down){
			_desiredDistance += DpadSensitivity;
		}*/

		// Get the scroll wheel input
		_desiredDistance = _desiredDistance - Input.GetAxis("Mouse ScrollWheel") * MouseScrollSensitivity;
		_desiredDistance = Mathf.Clamp(_desiredDistance, 0, MaxDistance);

		// Align camera when moving forward or backwards
		if (AlignCameraWhenMoving && _alignCameraWithCharacter)
			AlignCameraWithCharacter();

		_mouseXSmooth = Mathf.SmoothDamp(_mouseXSmooth, _mouseX, ref _mouseXCurrentVelocity, MouseSmoothTime);
		_mouseYSmooth = Mathf.SmoothDamp(_mouseYSmooth, _mouseY, ref _mouseYCurrentVelocity, MouseSmoothTime);

		Vector3 newCameraPosition;
		// Compute the desired position
		_desiredPosition = GetCameraPosition(_mouseYSmooth, _mouseXSmooth, _desiredDistance);
		// Compute the closest possible camera distance by checking if there is something inside the view frustum
		closestDistance = (new RPGClipPlane(_desiredPosition, CameraPivotPosition)).CheckViewFrustum();
		
		// Compute the new camera position
		newCameraPosition = GetCameraPosition(_mouseYSmooth, _mouseXSmooth, _desiredDistance);

		CameraTrfm.position = newCameraPosition;
		CameraTrfm.RotateAround(CameraPivotPosition, Vector3.up, LaboUnselectedCamera.MovingAngle * LaboUnselectedCamera.PosNumber);
		// Check if we are in third or first person and adjust the camera rotation behavior
		if (_distanceSmooth > MinimumDistance)
			CameraTrfm.LookAt(CameraPivotPosition);
		else
			CameraTrfm.eulerAngles = new Vector3(_mouseYSmooth, _mouseXSmooth, 0);
		
		if (mouseYConstrained) {			
			lookUpDegrees = _desiredMouseY - _mouseY;
			CameraTrfm.Rotate(Vector3.right, lookUpDegrees);
		}
	}

	private Vector3 GetCameraPosition(float xAxisDegrees, float yAxisDegrees, float distance) {
		Vector3 offset = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(xAxisDegrees, yAxisDegrees, 0);

		return CameraPivotPosition + rotation * offset;
	}

	public void Rotate(float degree) {
		_mouseX += degree;
	}

	public void SetAlignCameraWithCharacter(bool on) {
		_alignCameraWithCharacter = on && !Input.GetMouseButton(0) && !Input.GetMouseButton(1);
	}

	private void AlignCameraWithCharacter() {
		characterRotation = PlayerTrfm.eulerAngles.y;
		// Shift the character rotation offset so it fits the interval (-180,180]
		if (characterRotation > 180f) {
			characterRotation = characterRotation - 360f;
		}
		// Compute how many full rotations we have done with the camera and the offset to being behind the character
		float offsetToCameraRotation = CustomModulo(_mouseX, 360);
		float numberOfFullRotations = (_mouseX - offsetToCameraRotation) / 360;
		
		if (_mouseX < 0) {
			if (offsetToCameraRotation < -180 + characterRotation)
				numberOfFullRotations--;
		} else {
			if (offsetToCameraRotation > 180 + characterRotation)
				// The shortest way to rotate behind the character is to fulfill the current rotation
				numberOfFullRotations++;
		}

		_mouseX = numberOfFullRotations * 360 + characterRotation;
	}

	private float CustomModulo(float dividend, float divisor) {
		if (dividend < 0)
			return dividend - divisor * Mathf.Ceil(dividend / divisor);	
		else
			return dividend - divisor * Mathf.Floor(dividend / divisor);
	}

}
