using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]  

public class PlayerController : Photon.MonoBehaviour {

    public bool online = true;

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

	public float EnergyMaxAmount = 100.0f;
    private float EnergyNowAmount;

    public GameObject ForceField;

	public float animatorSpeed = 1.0f;

	private Ray ray;
	private RaycastHit rht;
	//private Vector3 fwd;
	private Vector2 Sw2fStartPos;
	public static bool onTouchMoveControll = false;

	private Transform PlayerTrfm;
	private Transform CameraTrfm;
	private Vector3 rootDirection;
	private Vector3 CameraDirection;
	private Quaternion referentialShift;
	private Vector3 moveDirection;
	private Vector3 axis;

    private float turnDirection;
    private Vector3 turnStartForward;
    private bool onLongTap = false;

	public static bool isDown = false;
    public static bool endBattle = false;

	public delegate void PlayerDownHandler();
	public static event PlayerDownHandler OnPlayerDown;

    public new PhotonView photonView;
    private PhotonTransformView photonTransformView;

    private EventTrigger eventTrigger;

    private LayerMask ignoreLayer;

    void Awake()
	{
        if (online)
        {
            photonView = GetComponent<PhotonView>();
            photonTransformView = GetComponent<PhotonTransformView>();
            if (!photonView.isMine)
            {
                Destroy(this);
            }
        }

        animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
		animator.speed = animatorSpeed;
	}

	void Start()
	{
		PlayerTrfm = this.gameObject.transform;
		CameraTrfm = Camera.main.transform;
        ignoreLayer = ~(1 << LayerMask.NameToLayer("Shield"));

        if (!online || photonView.isMine)
        {
            StartCoroutine(SetEventTriggers());
        }
    }

    IEnumerator SetEventTriggers()
    {
        while (!GameManager.UIReady)
        { }

        //Get EventTrigger and Make List of Event
        eventTrigger = GameObject.Find("JumpButton").GetComponent<EventTrigger>();
        eventTrigger.triggers = new List<EventTrigger.Entry>();

        //Add PointerDown Evevt
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        if (online)
            entry.callback.AddListener((eventData) => { photonView.RPC("Jump", PhotonTargets.All); });
        else
            entry.callback.AddListener((eventData) => { Jump(); });
        eventTrigger.triggers.Add(entry);

        //Add PointerUp Evevt
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerUp;
        if (online)
            entry2.callback.AddListener((eventData) => { photonView.RPC("StopJump", PhotonTargets.All); });
        else
            entry2.callback.AddListener((eventData) => { StopJump(); });
        eventTrigger.triggers.Add(entry2);

        yield return null;
    }



    //EasyTouch Operations

    // Subscribe to events
    void OnEnable(){
		EasyTouch.On_DoubleTap += On_DoubleTap;
		EasyTouch.On_TouchDown += On_TouchDown;
		EasyTouch.On_LongTap += On_LongTap;
        EasyTouch.On_TouchUp += On_TouchUp;
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
		EasyTouch.On_LongTap -= On_LongTap;
        EasyTouch.On_TouchUp -= On_TouchUp;
	}

	private void On_DoubleTap(Gesture gesture){
        onTouchMoveControll = true;
        //ダブルタップした座標をゲーム内座標に変換する
        ray = Camera.main.ScreenPointToRay(gesture.position);
        if (Physics.Raycast(ray, out rht, 1000, ignoreLayer))
        {
            //移動ベクトルを算出
            Vector3 fwd = transform.forward;
            moveVec = (rht.point - transform.position) * TouchMoveSensitivility;
            speed = Mathf.Clamp(moveVec.magnitude, 0, 1);
            axis = Vector3.Cross(fwd, moveVec);
            direction = Vector3.Angle(fwd, moveVec) / 180.0f *(axis.y < 0 ? -1 : 1);
            turnDirection = Math.Abs(direction);
            turnStartForward = fwd;
        }
        else if (!RPGCameraEx.onMoveView)
        {
            //何もないところ（空）をダブルタップすると停止
            speed = 0.0f;
            direction = 0.0f;
            moveVec = Vector3.zero;
//            onTouchMoveControll = false;
        }
	}

	private void On_TouchDown(Gesture gesture)
    {
		//onTouchMoveControll = true;
		if (!RPGCameraEx.onMoveView) {
			ray = Camera.main.ScreenPointToRay (gesture.position);
			if (Physics.Raycast (ray, out rht, 1000, ignoreLayer)) {
                //playerにタッチすると停止
                if (rht.collider.tag == "Player")
                {
					speed = 0.0f;
					direction = 0.0f;
					moveVec = Vector3.zero;
					onTouchMoveControll = false;
				}
                //player以外の物体にタッチするとその方向を向いて停止
/*                else
                {
                    Vector3 fwd = transform.forward;
                    //移動ベクトルを算出
                    moveVec = (rht.point - transform.position) * TouchMoveSensitivility;
                    axis = Vector3.Cross(fwd, moveVec);
                    direction = Vector3.Angle(fwd, moveVec) / 180.0f * (axis.y < 0 ? -1 : 1);
                    if (!RPGCameraEx.onMoveView)
                        locomotion.Do(speed * 6, direction * 180);
                        speed = 0.0f;
                    //                    onTouchMoveControll = false;
                }*/
			}
		}
	}

    private void On_LongTap(Gesture gesture)
    {
        onTouchMoveControll = true;
        ray = Camera.main.ScreenPointToRay(gesture.position);
       if (Physics.Raycast (ray, out rht, 1000, ignoreLayer))
       {
           onLongTap = true;
           Vector3 fwd = transform.forward;
           //移動ベクトルを算出
           moveVec = (rht.point - transform.position) * TouchMoveSensitivility;
           axis = Vector3.Cross(fwd, moveVec);
           direction = Vector3.Angle(fwd, moveVec) / 180.0f *(axis.y < 0 ? -1 : 1);
/*           if (!RPGCameraEx.onMoveView)
               locomotion.Do(speed * 6, direction * 180);
               speed = 0.0f;*/
           //    onTouchMoveControll = false;
           turnDirection = Math.Abs(direction);
           turnStartForward = fwd;
       }
    }

    private void On_TouchUp(Gesture gesture)
    {
        if (onLongTap)
        {
            onLongTap = false;
            speed = 0;
            onTouchMoveControll = false;
        }
    }


   
    // Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)/* ||
	inputDevice.Direction.X != 0 || inputDevice.Direction.Y != 0*/) 
		{
			onTouchMoveControll = false;
		}

		//Playerの移動
		if (animator && Camera.main) {
			if (onTouchMoveControll){
				TouchDo (PlayerTrfm, CameraTrfm, ref speed, ref direction);
                if (moveVec.magnitude > 1) moveVec = moveVec.normalized;
            } else {
				Do (PlayerTrfm, CameraTrfm, ref speed, ref direction);
            }
            if (Math.Abs(Vector3.Angle(transform.forward, turnStartForward) / 180.0f) >= turnDirection - 0.08f)
            {
//                direction = 0;
                turnStartForward = Vector3.zero;
                turnDirection = 0;
                if (onLongTap)
                {
                    speed = 0;
                }
            }
            locomotion.Do(speed * 6, direction * 180);
            //Synchronize PhotonTransform
            if (online)
                photonTransformView.SetSynchronizedValues(speed: moveVec * 6.0f, turnSpeed: 0);

        }

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

    void Do(Transform root, Transform camera, ref float speed, ref float direction)
    {

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
            Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
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
        stickDirection = new Vector3(horizontal, 0, vertical);
        moveVec = stickDirection;
        speedVec = new Vector2(horizontal, vertical);
        //		}
        moveDirection = referentialShift * stickDirection;
        speed = Mathf.Clamp(speedVec.magnitude, 0, 1);

        if (speed > 0.01f)
        { // dead zone
            axis = Vector3.Cross(rootDirection, moveDirection);
            direction = Vector3.Angle(rootDirection, moveDirection) / 180.0f * (axis.y < 0 ? -1 : 1);
        }
        else
        {
            direction = 0.0f;
        }
    }

    [PunRPC]
    public void Jump()
    {
        if (photonView.isMine)
        {
            animator.SetBool("Jump", true);
            state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Locomotion.Jump") || state.IsName("Locomotion.Idle"))
            {
                animator.SetBool("Jump", false);
            }
        }
    }

    [PunRPC]
    public void StopJump()
    {
        if (photonView.isMine)
        {
            animator.SetBool("Jump", false);
        }
    }

    //Player戦闘不能ルーチン
    public void PlayerDown()
	{
        isDown = true;
        Destroy(ForceField);
        animator.SetBool("Down", true);
        //        OnPlayerDown();
        Debug.Log ("Player Down!");
	}

}