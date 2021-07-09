using UnityEngine;
using System;
using System.Collections;
//using InControl;

[RequireComponent(typeof(Animator))]  

public class PlayerController4Test : MonoBehaviour {

	protected Animator animator;
	private AnimatorStateInfo state;

	private float speed = 0;
	private float direction = 0;
	private Locomotion locomotion = null;
	private Vector3 stickDirection = new Vector3(0, 0, 0);
	private Vector2 speedVec = new Vector2(0, 0);
	private Vector3 moveVec = new Vector3(0, 0, 0);

	private float horizontal;
	private float vertical;
	public float KeySensitivility = 1.0f;
	public float TouchMoveSensitivility = 1.8f;
	public float animatorSpeed = 1.0f;

//	private InputDevice inputDevice;

	private Ray ray;
	private RaycastHit rht;
	private Vector3 fwd;
	private Vector2 Sw2fStartPos;
	public static bool onTouchMoveControll = false;

	private Vector3 rootDirection;
	private Vector3 CameraDirection;
	private Quaternion referentialShift;
	private Vector3 moveDirection;
	private Vector3 axis;


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
		animator.speed = animatorSpeed;
	}
	
	//EasyTouch Operations
	
	// Subscribe to events
	void OnEnable(){
		EasyTouch.On_DoubleTap += On_DoubleTap;
		EasyTouch.On_TouchDown += On_TouchDown;
	}
	
	void OnDisable(){
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		EasyTouch.On_DoubleTap -= On_DoubleTap;
		EasyTouch.On_TouchDown -= On_TouchDown;
	}

	private void On_DoubleTap(Gesture gesture){
		onTouchMoveControll = true;
		//ダブルタップした座標をゲーム内座標に変換する
		ray = Camera.main.ScreenPointToRay(gesture.position);
		if (Physics.Raycast(ray, out rht, 100)){
			//移動ベクトルを算出
			moveVec = (rht.point - transform.position) * TouchMoveSensitivility;
		} else if (!RPGCameraEx.onMoveView) {
			//何もないところ（空）をダブルタップすると停止
			speed = 0.0f;
			direction = 0.0f;
			onTouchMoveControll = false;
		}
	}

	private void On_TouchDown(Gesture gesture){
		//playerにタッチすると停止
		if (!RPGCameraEx.onMoveView) {
			ray = Camera.main.ScreenPointToRay (gesture.position);
			if (Physics.Raycast (ray, out rht, 100)) {
				if (rht.collider.tag == "Player") {
					speed = 0.0f;
					direction = 0.0f;
					onTouchMoveControll = false;
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {

//		inputDevice = InputManager.ActiveDevice;

		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)/* ||
			inputDevice.Direction.X != 0 || inputDevice.Direction.Y != 0*/)
			onTouchMoveControll = false;

		//Plyaerの移動
		if (animator && Camera.main) {
			if (onTouchMoveControll){
				TouchDo (transform, Camera.main.transform, ref speed, ref direction);
			} else {
				Do (transform, Camera.main.transform, ref speed, ref direction);
			}
			locomotion.Do (speed * 6, direction * 180);
		}

		//Jump
		if (/*inputDevice.Action1 || */Input.GetKey(KeyCode.Space)) {
			animator.SetBool ("Jump", true);
		}
		state = animator.GetCurrentAnimatorStateInfo (0);
		if (state.IsName ("Locomotion.Jump") || state.IsName ("Locomotion.Idle")) {
			animator.SetBool ("Jump", false);
		}
	}


	void TouchDo(Transform root, Transform camera, ref float speed, ref float direction){
		
		rootDirection = root.forward;

		// Get camera rotation.    
		CameraDirection = camera.forward;
		CameraDirection.y = 0.0f; // kill Y
		referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);
		
		// Get camera rotation.    
		CameraDirection = camera.forward;
		CameraDirection.y = 0.0f; // kill Y
		referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);

		speed = Mathf.Clamp (moveVec.magnitude, 0, 1);      
		if (speed > 0.01f){ // dead zone
			axis = Vector3.Cross(rootDirection, moveVec);
			direction = Vector3.Angle(rootDirection, moveVec) / 180.0f * (axis.y < 0 ? -1 : 1);
		} else {
			direction = 0.0f;
		}
	}

	void Do(Transform root, Transform camera, ref float speed, ref float direction){
		
		rootDirection = root.forward;
		if (Input.GetKey(KeyCode.D))
			horizontal = KeySensitivility;
		if (Input.GetKey(KeyCode.A))
			horizontal = KeySensitivility * -1.0f;
		if (Input.GetKey(KeyCode.W))
			vertical = KeySensitivility;
		if (Input.GetKey(KeyCode.S))
			vertical = KeySensitivility * -1.0f;
		if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A) ||
		    Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)) {
			horizontal = 0;
			vertical = 0;
		}
		
		// Get camera rotation.    
		CameraDirection = camera.forward;
		CameraDirection.y = 0.0f; // kill Y
		referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);
		
		// Convert joystick input in Worldspace coordinates
/*		if (inputDevice.Direction.X != 0 || inputDevice.Direction.Y != 0) {
			stickDirection = new Vector3 (inputDevice.Direction.X, 0, inputDevice.Direction.Y);
			speedVec = inputDevice.LeftStick.Vector;
		} else {*/
			stickDirection = new Vector3 (horizontal, 0, vertical);
			speedVec = new Vector2 (horizontal, vertical);
//		}
		moveDirection = referentialShift * stickDirection;
		speed = Mathf.Clamp (speedVec.magnitude, 0, 1);      
		
		if (speed > 0.01f){ // dead zone
			axis = Vector3.Cross(rootDirection, moveDirection);
			direction = Vector3.Angle(rootDirection, moveDirection) / 180.0f * (axis.y < 0 ? -1 : 1);
		} else {
			direction = 0.0f;
		}
	}
}