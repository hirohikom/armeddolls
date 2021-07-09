using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]  

public class PlayerController_EnemyTapVer : MonoBehaviour {

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

	public Image ForceFieldMeter;
	public float ForceFieldMaxStrength = 100.0f;
	private float ForceFieldNowStrength;
	private Image ForceFieldSprite;

	public float EnergyMaxAmount = 100.0f;

	public float animatorSpeed = 1.0f;

	private Ray ray;
	private RaycastHit rht;
	private Vector3 fwd;
	private Vector2 Sw2fStartPos;
	public static bool onTouchMoveControll = false;

	private Transform PlayerTrfm;
	private Transform CameraTrfm;
	private Vector3 rootDirection;
	private Vector3 CameraDirection;
	private Quaternion referentialShift;
	private Vector3 moveDirection;
	private Vector3 axis;

	public static bool isDown = false;

	public delegate void PlayerDownHandler();
	public static event PlayerDownHandler OnPlayerDown;


	void Awake()
	{
		ForceFieldSprite = ForceFieldMeter;//.gameObject.GetComponent<UISprite>();
//		ForceFieldSprite.fillAmount = 1.0f;
		ForceFieldNowStrength = ForceFieldMaxStrength;

		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
		animator.speed = animatorSpeed;
	}

	void Start()
	{
		PlayerTrfm = this.gameObject.transform;
		CameraTrfm = Camera.main.transform;
	}

	//EasyTouch Operations
	
	// Subscribe to events
	void OnEnable(){
		EasyTouch.On_DoubleTap += On_DoubleTap;
		EasyTouch.On_TouchDown += On_TouchDown;
		//EasyTouch.On_LongTap += On_LongTap;
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
		//EasyTouch.On_LongTap -= On_LongTap;
	}

	private void On_DoubleTap(Gesture gesture){
        onTouchMoveControll = true;
        //ダブルタップした座標をゲーム内座標に変換する
        ray = Camera.main.ScreenPointToRay(gesture.position);
        if (Physics.Raycast(ray, out rht, 100))
        {
            //移動ベクトルを算出
            moveVec = (rht.point - transform.position) * TouchMoveSensitivility;
        }
        else if (!RPGCameraEx.onMoveView)
        {
            //何もないところ（空）をダブルタップすると停止
            speed = 0.0f;
            direction = 0.0f;
            moveVec = Vector3.zero;
            onTouchMoveControll = false;
        }
	}

	private void On_TouchDown(Gesture gesture){
		if (!RPGCameraEx.onMoveView) {
			ray = Camera.main.ScreenPointToRay (gesture.position);
			if (Physics.Raycast (ray, out rht, 100)) {
                //playerにタッチすると停止
                if (rht.collider.tag == "Player")
                {
					speed = 0.0f;
					direction = 0.0f;
					moveVec = Vector3.zero;
					onTouchMoveControll = false;
				}
                //敵にタッチするとその方向を向いて停止
                if (rht.collider.tag == "Enemy")
                {
                    Vector3 fwd = this.transform.forward;
                    Vector3 vec = rht.point - transform.position;
                    axis = Vector3.Cross(fwd, vec);
                    direction = Vector3.Angle(fwd, vec) / 180.0f * (axis.y < 0 ? -1 : 1);
                    speed = 0.0f;
                    moveVec = Vector3.zero;
                    onTouchMoveControll = false;
                    locomotion.Do(speed * 6, direction * 180);
                }
			}
		}
	}
	
/*	private void On_LongTap(Gesture gesture)
	{
		ray = Camera.main.ScreenPointToRay (gesture.position);
		if (Physics.Raycast (ray, out rht, 100))
		{
			Vector3 rhtpos = rht.transform.position;
			Vector3 targetpos = new Vector3(rhtpos.x, this.transform.position.y, rhtpos.z);
			this.transform.LookAt (targetpos);
		}
	}
*/
	// Update is called once per frame
	void Update () {

/*		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)/* ||
//	inputDevice.Direction.X != 0 || inputDevice.Direction.Y != 0*///) 
/*		{
			moveVec = Vector3.zero;
			onTouchMoveControll = false;
		}*/

		//Playerの移動
		if (animator && Camera.main) {
			if (onTouchMoveControll){
				TouchDo (PlayerTrfm, CameraTrfm, ref speed, ref direction);
			} else {
				Do (PlayerTrfm, CameraTrfm, ref speed, ref direction);
			}
			locomotion.Do (speed * 6, direction * 180);
		}

		//Jump
//		if (/*inputDevice.Action1 || */Input.GetKey(KeyCode.Space)) {
/*			animator.SetBool ("Jump", true);
		}
		state = animator.GetCurrentAnimatorStateInfo (0);
		if (state.IsName ("Locomotion.Jump") || state.IsName ("Locomotion.Idle")) {
			animator.SetBool ("Jump", false);
		}*/

		//Turn
/*		if (Input.GetKey (KeyCode.Space) || RepeatButton_Turn.RepeatBtn) 
		{
			Turn();
		}*/
	}

	void TouchDo(Transform root, Transform camera, ref float speed, ref float direction){
		
		rootDirection = root.forward;

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
//		}*/
		moveDirection = referentialShift * stickDirection;
		speed = Mathf.Clamp (speedVec.magnitude, 0, 1);      
		
		if (speed > 0.01f){ // dead zone
			axis = Vector3.Cross(rootDirection, moveDirection);
			direction = Vector3.Angle(rootDirection, moveDirection) / 180.0f * (axis.y < 0 ? -1 : 1);
		} else {
			direction = 0.0f;
		}
	}

	public void Turn()
	{
		if (moveVec == Vector3.zero || speed == 0)
			direction = 180.0f;
		else
			moveVec *= -1.0f;
		locomotion.Do (speed * 6, direction * 180);
	}

	//ダメージ処理
	//_params[0]:Vector3　発射地点
	//_params[1]:Vector3　着弾地点
	//_params[2]:float　ダメージ量
	void OnDamage(object[] _params)
	{
		float _damage = (float)_params [2];
		ForceFieldNowStrength -= _damage;
		if (ForceFieldNowStrength <= 0)
			ForceFieldNowStrength = 0;
		ForceFieldSprite.fillAmount = ForceFieldNowStrength / ForceFieldMaxStrength;
		int FFRemain = (int)(100.0f * ForceFieldNowStrength / ForceFieldMaxStrength);
		string ffr = FFRemain.ToString();

		if (ForceFieldNowStrength <= 0) 
		{
			PlayerDown();
		}
	}

	//Player戦闘不能ルーチン
	void PlayerDown()
	{
		isDown = true;
		OnPlayerDown ();
		Debug.Log ("Player Down!");
	}



}