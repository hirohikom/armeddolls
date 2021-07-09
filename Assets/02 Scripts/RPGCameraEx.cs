using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RPGCameraEx : MonoBehaviour {

    public bool online = true;
	
	public Vector3 CameraPivotLocalPosition = new Vector3(0, 1.0f, 0);
	public bool AlignCameraWhenMoving = true;
	public float startHeight = 100.0f;
	public float startDistanse = 100.0f;
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

	public KeyCode ResetSwitch = KeyCode.R;
	public float StartCameraX = 0;
	public float StartCameraY = 10.0f;
	public float StartDistance = 2.5f;
	public GameObject playerObject;

	public static bool onTouch = false;
	public static bool onMoveView = false;
	public static bool onPlayer = false;

	private Vector3 _cameraPivotPosition;
	private Vector3 _desiredPosition;
	private float _desiredDistance;
	private float _distanceSmooth = 0;
	private float _distanceCurrentVelocity;
	private bool _alignCameraWithCharacter = false;
	private float _mouseX = 0;
	private float _mouseXSmooth = 0;
	private float _mouseXCurrentVelocity;
	private float _mouseY = 0;
	private float _mouseYSmooth = 0;
	private float _mouseYCurrentVelocity;
	private float _desiredMouseY = 0;

	private Transform PlayerTrfm;
	private Transform ArmLTrfm;
	private Transform ArmRTrfm;
	private Transform CameraTrfm;

	private int PlayerLayerIdx; //"Player"レイヤーのindex

	private float mouseYMinLimit;
	private float closestDistance;
	private float lookUpDegrees;
	private float characterRotation;
	private float cx, cz, actualDistance, horizontalDistance;

	private Ray ray;
	private RaycastHit rht;

    private EventTrigger eventTrigger;

    public PhotonView photonView;


	private void Awake() 
    {
        if (online && !photonView.isMine)
        {
            Destroy(this);
        }

        //GameManager.LoadedScene = true;

		PlayerTrfm = this.gameObject.transform;
//        ArmLTrfm = GameObject.Find("shoulder-arm_L").transform;
//        ArmRTrfm = GameObject.Find("shoulder-arm_R").transform;
        CameraTrfm = Camera.main.transform;

		// Check if there is a main camera in the scene to use
		if (Camera.main == null) {
			GameObject mainCamera = new GameObject("Main Camera");
			mainCamera.AddComponent<Camera>();
			mainCamera.tag = "MainCamera";
		}
		
		//"Player"レイヤーのindexを取得
		PlayerLayerIdx = LayerMask.NameToLayer ("Player");

		CameraTrfm.position = new Vector3 (0, startHeight, startDistanse);
		CameraTrfm.LookAt (_cameraPivotPosition);
		_mouseYSmooth = startHeight;
		_mouseXSmooth = 0;
		_distanceSmooth = startDistanse;
	}

	private void Start() {
        GameManager.LoadedScene = true;

        if (!online || photonView.isMine)
        {
            StartCoroutine(SetEventTriggers());
        }

        ResetView();
	}

    IEnumerator SetEventTriggers()
    {
        while (!GameManager.UIReady)
        { }

        //Get EventTrigger and Make List of Event
        eventTrigger = GameObject.Find("StatesFrame").GetComponent<EventTrigger>();
        eventTrigger.triggers = new List<EventTrigger.Entry>();

        //Add PointerDown Evevt
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { ResetView(); });
        eventTrigger.triggers.Add(entry);

        yield return null;
    }

        //EasyTouch Operations
        void OnEnable(){
		EasyTouch.On_TouchStart += On_TouchStart;
		EasyTouch.On_TouchUp += On_TouchUp;
		EasyTouch.On_SwipeStart += On_SwipeStart;
		EasyTouch.On_Swipe += On_Swipe;
		EasyTouch.On_SwipeEnd += On_SwipeEnd;
		EasyTouch.On_TouchStart2Fingers += On_TouchStart2Fingers;
		EasyTouch.On_PinchIn += On_PinchIn;
		EasyTouch.On_PinchOut += On_PinchOut;
		EasyTouch.On_PinchEnd += On_PinchEnd;
//		EasyTouch.On_LongTap += On_LongTap;
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
		EasyTouch.On_TouchStart2Fingers -= On_TouchStart2Fingers;
		EasyTouch.On_PinchIn -= On_PinchIn;
		EasyTouch.On_PinchOut -= On_PinchOut;
		EasyTouch.On_PinchEnd -= On_PinchEnd;
//		EasyTouch.On_LongTap -= On_LongTap;
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
//			ArmLTrfm.Rotate(_y, 0, 0);
//			ArmRTrfm.Rotate(_y, 0, 0);
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

	private void On_TouchStart2Fingers( Gesture gesture){
		onMoveView = true;
		EasyTouch.SetEnablePinch(true);
	}

	private void On_PinchIn(Gesture gesture){
		onMoveView = true;
		_desiredDistance -= gesture.deltaPinch * PinchSensitivity;
	}
	
	private void On_PinchOut(Gesture gesture){
		onMoveView = true;
		_desiredDistance += gesture.deltaPinch * PinchSensitivity;
	}
	
	private void On_PinchEnd(Gesture gesture){
		onMoveView = false;
	}

//	private void On_LongTap(Gesture gesture){
//		ResetView ();
//	}


	private void LateUpdate() {

		// Set the camera's pivot position in world coordinates
		_cameraPivotPosition = PlayerTrfm.position + CameraPivotLocalPosition;
		// Check if the camera lies on the ground
		bool mouseYConstrained = Physics.Linecast(CameraTrfm.position,
		                                          CameraTrfm.position + Vector3.down);
		mouseYConstrained = mouseYConstrained && CameraTrfm.position.y < _cameraPivotPosition.y;

		#region Get inputs

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

		//Reset ViewPoint
/*        if (Input.GetKey(ResetSwitch) || Input.GetMouseButton(1))
        {
			ResetView();
		}*/

		// Align camera when moving forward or backwards
		if (AlignCameraWhenMoving && _alignCameraWithCharacter)
			AlignCameraWithCharacter();

		#endregion

		#region Smooth the inputs
		
		_mouseXSmooth = Mathf.SmoothDamp(_mouseXSmooth, _mouseX, ref _mouseXCurrentVelocity, MouseSmoothTime);
		_mouseYSmooth = Mathf.SmoothDamp(_mouseYSmooth, _mouseY, ref _mouseYCurrentVelocity, MouseSmoothTime);

		#endregion

		#region Compute the new camera position
		Vector3 newCameraPosition;
		// Compute the desired position
		_desiredPosition = GetCameraPosition(_mouseYSmooth, _mouseXSmooth, _desiredDistance);
		// Compute the closest possible camera distance by checking if there is something inside the view frustum
		closestDistance = (new RPGClipPlane(_desiredPosition, _cameraPivotPosition)).CheckViewFrustum();
		
		if (closestDistance > MinimumDistance) {
			// Set the camera distance to the closest distance because the camera is constricted
			closestDistance -= Camera.main.nearClipPlane;
			if (_distanceSmooth < closestDistance)
				// Smooth the distance if we move from a smaller constricted distance to a bigger constricted distance
				_distanceSmooth = Mathf.SmoothDamp(_distanceSmooth, closestDistance, ref _distanceCurrentVelocity, DistanceSmoothTime);
			else
				// Do not smooth if the new closest distance is smaller than the current distance
				_distanceSmooth = closestDistance;
		
		} else {
			// The camera is not constricted (anymore) so smooth the distance change
			_distanceSmooth = Mathf.SmoothDamp(_distanceSmooth, _desiredDistance, ref _distanceCurrentVelocity, DistanceSmoothTime);
		}

		// Compute the new camera position
		newCameraPosition = GetCameraPosition(_mouseYSmooth, _mouseXSmooth, _distanceSmooth);
		
		#endregion

		#region Update the camera transform

		CameraTrfm.position = newCameraPosition;
		// Check if we are in third or first person and adjust the camera rotation behavior
		if (_distanceSmooth > MinimumDistance)
			CameraTrfm.LookAt(_cameraPivotPosition);
		else
			CameraTrfm.eulerAngles = new Vector3(_mouseYSmooth, _mouseXSmooth, 0);
		
		// Let the character fade
		CharacterNeer();

		if (mouseYConstrained) {			
			lookUpDegrees = _desiredMouseY - _mouseY;
			CameraTrfm.Rotate(Vector3.right, lookUpDegrees);
		}

		#endregion
	}

	private Vector3 GetCameraPosition(float xAxisDegrees, float yAxisDegrees, float distance) {
		Vector3 offset = new Vector3(0, 0, -distance);
		Quaternion rotation = Quaternion.Euler(xAxisDegrees, yAxisDegrees, 0);

		return _cameraPivotPosition + rotation * offset;
	}

	public void ResetView() {
		_mouseX = PlayerTrfm.eulerAngles.y + StartCameraX;
		_mouseY = _desiredMouseY = StartCameraY;
		_desiredDistance = StartDistance;
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

	private void CharacterNeer() {
		// Get the actual distance
		
		cx = CameraTrfm.position.x;
		cz = CameraTrfm.position.z;

		actualDistance = Vector3.Distance(PlayerTrfm.position, CameraTrfm.position);

		while (actualDistance < RiseDistance) {
			CameraTrfm.position = new Vector3(cx, CameraTrfm.position.y + 0.001f, cz);
			actualDistance = Vector3.Distance(PlayerTrfm.position, CameraTrfm.position);
		}

		CameraTrfm.LookAt(_cameraPivotPosition);

		//CameraとPlayerの距離がMinimumDistanceより小さくなったら"Player"レイヤーを非表示にする
		horizontalDistance = Vector2.Distance (new Vector2 (PlayerTrfm.position.x, PlayerTrfm.position.z), new Vector2 (cx, cz));
		if (horizontalDistance <= MinimumDistance) {
			Camera.main.cullingMask &= ~(1<<PlayerLayerIdx);
		} else {
			Camera.main.cullingMask |= (1<<PlayerLayerIdx);
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(transform.position + CameraPivotLocalPosition, 0.1f);
	}
}
