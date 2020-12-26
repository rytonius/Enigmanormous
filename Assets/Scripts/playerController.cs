using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;




namespace Com.Enigmanormous
{

    public class playerController : MonoBehaviourPunCallbacks, IPunObservable //!
    {
        #region Variables

        [Header("Stats")]
        [Tooltip("Level = Current level for your Manly Hero")]
        public int XPLevel = 1;
        [Tooltip("XP = get this to 0, you gain a level, each level would be something like (100 x level * 1.1) = next level")]
        public int XPoints = 100;
        [Tooltip("Xenies = Currency of the world to buy and sell drugs")]
        public int Currency = 0;
        [Tooltip("player health")]
        public int maxHealthPoints = 100;
        public Color HealthBar;
        private int currentHealth;
        [Tooltip("Energy")]
        public int energyPoints = 100;
        [Tooltip("offensive stat = Accuracy related?")]
        public int Dexterity = 10;
        [Tooltip("defensive stat = Endurance related?")]
        public int Endurance = 10;
        [Tooltip("Agility stat = speed run faster related?")]
        public int needForSpeed = 10;



        [Header("Movement")]
        [Tooltip("speed of how fast character moves nukka")]
        public float speed = 400f;
        [Tooltip("SPRINT MODIFIER GO bRRRRRR")]
        public float sprintXthis = 2f;
        [Tooltip("Never say a white man can't jump")]
        public float jumpForce = 200f;
        [Tooltip("Crouch speed reduced")]
        public float crouchModifier = 0.8f;        
        [Tooltip("slide time, how far you slide")]
        public float lengthOfSlide = 5f;
        public float slideModifier = 5f;
        [Tooltip("put in their rigid body (attached to the player most likely)")]
        private Rigidbody rig;
        //^^ that rig body is what moves when you like move

        [Header("CameraStuff")]
        [Tooltip("drop the player model here")]
        public Transform player;
        [Tooltip("drop the camera for that player here, same camera both sluts")]
        public GameObject cameraParent;
        public Transform cams;
        public Camera camsFov;
        [Tooltip("the object that holds weapons on the player object goes here")]
        public Transform weaponArm;

        [Tooltip("the dudes arm on the player object goes here")]
        public Transform playerArm;
        //public Transform FlashLightAttach;
        [Tooltip("base FOV for the player")]
        public float baseFOV = 60f;
        [Tooltip("X Sensitivity so like how fast you turn")]
        public float xSensitivity = 100f;
        [Tooltip("Y Sensitivity so like how fast you turn")]
        public float ySensitivity = 100f;
        [Tooltip("max angle that clamps the Vertical Looking")]
        public float maxAngle = 60f;
        [Tooltip("lowers the camera when crouched")]
        public float crouchAmount = 0.5f;
        [Tooltip("lowers the camera when slide")]
        public float slideAmount = 0.5f;


        [Header("OtherStuff")]
        public LayerMask ground;
        public Transform groundDetector;
        
        public Gradient gradient;
        public GameObject standingCollider;
        public GameObject crouchingCollider;
        
        private static bool cursorLocked = true; //this is for escape menu, hit escape to access

        private Slider ui_healthbar;
        private Image Ui_healthbarcolor;
        private TextMeshProUGUI ui_healthtxt;
        private TextMeshProUGUI ui_ammotxt;
        private TextMeshProUGUI ui_cliptxt;

        // can't remmber what this does... OH it's the center of your camera, the second private Camera normCam is just to grab the camera for FOV shit
        private Quaternion camCenter;


        // to get weapon location origin
        private Vector3 weaponParentOrigin;
        private Vector3 weaponParentCurrentPOS;
        private Vector3 targetBobPosition;

        // this is the base FOV, this is ingame so that you can do cool affects like raise or lower fov when sprinting

        private float sprintFOVModifier = 1.25f;
        private Vector3 origin;

        //target camera for sliding 
        private Vector3 normalCamTarget;
        private Vector3 weaponCamTarget;

        [Tooltip("this will zoom in when using ADS on a non sniper weapon")]
        public float aimFOVModifier = 0.5f;
        public float sniperFOVModifier = 0.1f;

        // movementCounter and idle counter
        private float movementCounter;
        private float idleCounter;

        //this section makes all those private voids talk to each other
        private bool jump;
        private bool sprint;
        private bool crouch;
        private bool crouched;
        private bool sliding;
        private Vector3 slideDirection;
        private float slideTime;
        //private bool isJumping;        
        private bool isSprinting = false;
        private bool isCurrentlyAiming = false;
        private bool isCurrentlySniping = false;
        float xMove;
        float zMove;

        // lets you use other scripts methods
        private weaponEquip weaponEquip;
        private Manager manager;
        //private Flashlight_PRO Flashlight;
        private int receivedDamage;
        

        // Ammo System
        [Header("Ammo system")]
        public int stashAmmoBullets = 90;
        public int stashSniperBullets = 15;
        private float aimAngle;



        #endregion

        #region Monobehavior Callbacks
        //start loads things for the script i guess
        private void Awake()
        {
            //this will grab a script easily if it's same object
            weaponEquip = gameObject.GetComponent<weaponEquip>();
            //this will find the script if it's not connected to your body
            manager = GameObject.Find("=Manager=").GetComponent<Manager>();

            //Flashlight = GameObject.Find("Misc/Flashlight").GetComponent<Flashlight_PRO>();


        }

        private void Start()
        {

            cameraParent.SetActive(photonView.IsMine);

            if (!photonView.IsMine)
            {

                gameObject.layer = 26;
            }

            if (photonView.IsMine)
            {
                currentHealth = maxHealthPoints;
                ui_healthbar = GameObject.Find("HUD/Health/HealthBar").GetComponent<Slider>();
                Ui_healthbarcolor = GameObject.Find("HUD/Health/HealthBar/fill").GetComponent<Image>();
                ui_healthtxt = GameObject.Find("HUD/Health/HealthTxt").GetComponent<TextMeshProUGUI>();
                ui_cliptxt = GameObject.Find("HUD/Ammo/ClipTxt").GetComponent<TextMeshProUGUI>();
                ui_ammotxt = GameObject.Find("HUD/Ammo/AmmoTxt").GetComponent<TextMeshProUGUI>();
                RefreshHealthBar();

            }


            baseFOV = camsFov.fieldOfView;
            origin = cams.transform.localPosition;
            if (Camera.main) Camera.main.enabled = false;
            rig = GetComponent<Rigidbody>();
            camCenter = cams.localRotation; // set rotation orgin for cameras to camCenter hahaha makes sense
            weaponParentOrigin = weaponArm.localPosition;
            weaponParentCurrentPOS = weaponParentOrigin;


        }

        //fixed update for when you do things, right now just player movement 
        void FixedUpdate()
        {
            if (!photonView.IsMine) return;
            Movement();
            // headbob
            HeadBobConditions();

        }

        private void Update()
        {

            if (!photonView.IsMine)
            {
                RefreshMultiplayerState();
                return;
            }
                
        
            if (cursorLocked)
            {
                MyInput();
                SetY();
                SetX();

            }

            //if (Input.GetKeyDown(KeyCode.F)) Flashlight.Switch();
            KillYourself();
            UpdateCusorLock();
            RefreshHealthBar();
            weaponEquip.RefreshAmmo(ui_ammotxt);
            weaponEquip.RefreshClip(ui_cliptxt);
            
            
        }
        #endregion

        #region Private Methods
        //Set Y is for vertical camera movement
        private void MyInput()
        {
            /*
             * START OF CONTROLS
             */
            // horizontal is A and D, and vertical is W and S
            xMove = Input.GetAxisRaw("Horizontal");
            zMove = Input.GetAxisRaw("Vertical");

            //this part is to get input if your holding shift so you can sprint  || means OR so left OR right Shift
            sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            crouch = Input.GetKeyDown(KeyCode.C);

            // Jump bitch
            jump = Input.GetKey(KeyCode.Space);
            /*
             * END OF CONTROLS
             */

        }

        private void Movement()
        {

            
            // These are considered States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.5f, ground);
            // !isJumping so you can't sprint while jumping cause that'd look weird... or would it.... dun DUH dun
            bool isJumping = jump && isGrounded;
            isCurrentlyAiming = weaponEquip.currentlyAiming;
            isCurrentlySniping = weaponEquip.currentlySniping;
            //if your sprinting, ok so the && zMove > 0 means you can only sprint if your only going forward
            bool isSprinting = sprint && zMove > 0 && !isJumping && isGrounded && !isCurrentlyAiming;
            bool isCrouching = crouch && !isSprinting && !isJumping && isGrounded;
            bool isSliding = isSprinting && crouch && !sliding;

            

            //if your moving then this will like, apply that to a variable which direction your moving.  
            Vector3 playerDirection = Vector3.zero;
            float playerAdjustedSpeed = speed * 10;

            if (isCrouching)
            {
                photonView.RPC("SetCrouch", RpcTarget.All, !crouched);
            }

            if (!sliding)
            {
                
                playerDirection = new Vector3(xMove, 0, zMove);
                playerDirection.Normalize();
                playerDirection = transform.TransformDirection(playerDirection);
                //but if you hold sprint, then it multiplies with sprintXthis variable and your camera goes WOOOOOO
                if (isSprinting)
                {
                    if (crouched) photonView.RPC("SetCrouch", RpcTarget.All, false);
                    playerAdjustedSpeed *= sprintXthis;
                }
                else if (crouched)
                {
                    playerAdjustedSpeed *= crouchModifier;
                }

            }
            else
            {
                
                playerDirection = slideDirection;
                // slide speed
                playerAdjustedSpeed *= slideModifier;
                slideTime -= Time.fixedDeltaTime;
                if (slideTime <= 0)
                {
                    sliding = false;
                    weaponParentCurrentPOS -= Vector3.down * (slideAmount - crouchAmount);
                }

            }
            Vector3 playerTargetVelocity = (playerDirection) * playerAdjustedSpeed * Time.fixedDeltaTime;
            playerTargetVelocity.y = rig.velocity.y;
            rig.velocity = playerTargetVelocity;

            if (isCurrentlyAiming)
            {
                if (!isCurrentlySniping)
                {
                    camsFov.fieldOfView = Mathf.Lerp(camsFov.fieldOfView, baseFOV * aimFOVModifier, Time.fixedDeltaTime * 6f);
                    
                }
                else
                {
                    camsFov.fieldOfView = Mathf.Lerp(camsFov.fieldOfView, baseFOV * sniperFOVModifier, Time.fixedDeltaTime * 10f);
                    
                }

            }

            if (isJumping)
            {
                if (crouched) photonView.RPC("SetCrouch", RpcTarget.All, false);
                rig.AddForce(Vector3.up * jumpForce);

            }

            

            if(isSliding)
            {
                sliding = true;
                slideDirection = playerDirection;
                slideTime = lengthOfSlide;
                // adjust camera
                weaponParentCurrentPOS += Vector3.down * (slideAmount - crouchAmount);
                if (!crouched) photonView.RPC("SetCrouch", RpcTarget.All, true);

            }
            // camera stuff
            if (sliding)
            {
                camsFov.fieldOfView = Mathf.Lerp(camsFov.fieldOfView, baseFOV * 1.25f, Time.fixedDeltaTime * 8f);
                cams.transform.localPosition = Vector3.Lerp(cams.transform.localPosition, origin + Vector3.down * slideAmount, Time.deltaTime * 6f);

                
            }
            // camera stuff when sprinting an not sprinting
            else
            {
                if (isSprinting)
                {
                    // this makes it smooooooooooth when you sprint you zoom out slowly matfh.lerppppp
                    camsFov.fieldOfView = Mathf.Lerp(camsFov.fieldOfView, baseFOV * sprintFOVModifier, Time.fixedDeltaTime * 4f);
                } else { camsFov.fieldOfView = Mathf.Lerp(camsFov.fieldOfView, baseFOV, Time.fixedDeltaTime * 8f); }
                if (crouched)
                {                    
                    cams.transform.localPosition = Vector3.Lerp(cams.transform.localPosition, origin + Vector3.down * crouchAmount, Time.deltaTime * 6f);
                } else { cams.transform.localPosition = Vector3.Lerp(cams.transform.localPosition, origin, Time.deltaTime * 6f); }
            }

            
        }

        void SetY()
        {
            float playerYInput = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            Quaternion playerAdjust = Quaternion.AngleAxis(playerYInput, -Vector3.right);
            Quaternion playerDelta = cams.localRotation * playerAdjust;

            if (Quaternion.Angle(camCenter, playerDelta) < maxAngle)
            {
                cams.localRotation = playerDelta;

            }

            // moves the gun and arm with the rotation (is this still needed?)
            weaponArm.localRotation = cams.localRotation;
            //FlashLightAttach.localRotation = cams.localRotation;
            //playerArm.localRotation = weaponArm.localRotation;

        }

        // Set X is for rotation of player body
        void SetX()
        {
            float playerXInput = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            Quaternion playerAdjust = Quaternion.AngleAxis(playerXInput, Vector3.up);
            Quaternion playerDelta = player.localRotation * playerAdjust;
            player.localRotation = playerDelta;
        }

        //this shit is for locking the cursor when you hit escape (so when i make the escape menu motherfucker)
        void UpdateCusorLock()
        {
            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                // see this shit, when you press that key, it's not an input and does what its says it'll do
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = false;
                }

            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                // motherfucker did you just hit escape again, well guess what, now you've uhh unlocked (or locked? fuck if i know)

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLocked = true;
                }
            }

        }


        // head bobbing code

        void HeadBob(float playerBobZ, float playerBobIntensityX, float PlayerBobIntensityY)
        {
            float t_aim_adjust = 1f;
            if (weaponEquip.currentlyAiming) t_aim_adjust = 0.1f;
            targetBobPosition = weaponParentCurrentPOS + new Vector3(Mathf.Cos(playerBobZ) * playerBobIntensityX * t_aim_adjust, Mathf.Sin(playerBobZ * 2) * PlayerBobIntensityY * t_aim_adjust, 0);
        }
        // this uhh is the conditions so you bob that head I SAID BOB THAT HEAD

        private void HeadBobConditions()
        {
            if (sliding) //sliding
            {
                HeadBob(movementCounter, 0.15f, 0.12f);
                
                weaponArm.localPosition = Vector3.Lerp(weaponArm.localPosition, targetBobPosition, Time.fixedDeltaTime * 6f);
            }
            else if (xMove == 0 && zMove == 0 && isCurrentlyAiming && isCurrentlySniping) //sniping
            {
                HeadBob(idleCounter, 0.002f, 0.002f);
                idleCounter += Time.fixedDeltaTime;
                weaponArm.localPosition = Vector3.Lerp(weaponArm.localPosition, targetBobPosition, Time.fixedDeltaTime * 2f);
            }

            else if (xMove == 0 && zMove == 0 && isCurrentlyAiming && !isCurrentlySniping) //aiming
            {

                HeadBob(idleCounter, 0.01f, 0.01f);
                idleCounter += Time.fixedDeltaTime;
                weaponArm.localPosition = Vector3.Lerp(weaponArm.localPosition, targetBobPosition, Time.fixedDeltaTime * 3f);

            }

            else if (xMove == 0 && zMove == 0) //idle
            {

                HeadBob(idleCounter, 0.02f, 0.02f);
                idleCounter += Time.fixedDeltaTime;
                weaponArm.localPosition = Vector3.Lerp(weaponArm.localPosition, targetBobPosition, Time.fixedDeltaTime * 3f);

            }

            else if (!isSprinting && !crouched) //walking
            {
                HeadBob(movementCounter, 0.07f, 0.07f);
                movementCounter += Time.fixedDeltaTime * 3f;
                weaponArm.localPosition = Vector3.Lerp(weaponArm.localPosition, targetBobPosition, Time.fixedDeltaTime * 6f);
            }
            
            else if (crouched) //crouched
            {
                HeadBob(movementCounter, 0.03f, 0.03f);
                movementCounter += Time.fixedDeltaTime * 2f;
                weaponArm.localPosition = Vector3.Lerp(weaponArm.localPosition, targetBobPosition, Time.fixedDeltaTime * 4f);
            }

            else //sprinting
            {
                HeadBob(movementCounter, 0.12f, 0.15f);
                movementCounter += Time.fixedDeltaTime * 6f;
                weaponArm.localPosition = Vector3.Lerp(weaponArm.localPosition, targetBobPosition, Time.fixedDeltaTime * 10f);
            }

        }

        private void RefreshHealthBar()
        {
            float t_health_ratio = (float)currentHealth / (float)maxHealthPoints;
            ui_healthbar.value = t_health_ratio;
            Ui_healthbarcolor.color = gradient.Evaluate(ui_healthbar.normalizedValue);


            ui_healthtxt.text = ("HP: " + currentHealth.ToString() + "   -" + receivedDamage);

        }

        private void KillYourself()
        {
            if (Input.GetKeyDown(KeyCode.K)) TakeDamage(20);
        }

        #endregion

        #region Public Methods


        public void TakeDamage(int playerDamage)
        {
            if (photonView.IsMine)
            {
                currentHealth -= playerDamage;
                receivedDamage = playerDamage;
                RefreshHealthBar();

                if (currentHealth <= 0)
                {
                    manager.Spawn();
                    PhotonNetwork.Destroy(gameObject);
                }

            }

        }

        [PunRPC]
        void SetCrouch(bool p_state)
        {
            if (crouched == p_state) return;

            crouched = p_state;

            if (crouched)
            {
                standingCollider.SetActive(false);
                crouchingCollider.SetActive(true);
                weaponParentCurrentPOS += Vector3.down * crouchAmount;
            }

            else
            {
                standingCollider.SetActive(true);
                crouchingCollider.SetActive(false);
                weaponParentCurrentPOS -= Vector3.down * crouchAmount;
            }
        }
        #endregion

        #region Photon Callbacks
        public void OnPhotonSerializeView(PhotonStream p_stream, PhotonMessageInfo p_message)
        {
            if (p_stream.IsWriting)
            {
                p_stream.SendNext((int)(weaponArm.transform.localEulerAngles.x * 100f));
            }
            else
            {
                aimAngle = (int)p_stream.ReceiveNext() * 0.001f;
            }
        }

        void RefreshMultiplayerState()
        {
            float cacheEulY = weaponArm.localEulerAngles.y;

            Quaternion targetRotation = Quaternion.identity * Quaternion.AngleAxis(aimAngle, Vector3.right);
            weaponArm.rotation = Quaternion.Slerp(weaponArm.rotation, targetRotation, Time.deltaTime * 8f);

            Vector3 finalRotation = weaponArm.localEulerAngles;
            finalRotation.y = cacheEulY;

            weaponArm.localEulerAngles = finalRotation;
        }
    }


    #endregion
}

