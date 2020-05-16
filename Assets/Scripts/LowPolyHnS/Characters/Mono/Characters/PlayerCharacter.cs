using LowPolyHnS.Attributes;
using LowPolyHnS.Core;
using LowPolyHnS.Core.Hooks;
using UnityEngine;

namespace LowPolyHnS.Characters
{
    [AddComponentMenu("LowPolyHnS/Characters/Player Character", 100)]
    public class PlayerCharacter : Character
    {
        public enum INPUT_TYPE
        {
            PointAndClick,
            Wsad,
            FollowPointer,
            FollowAndClickPointer
        }

        public enum MOUSE_BUTTON
        {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2
        }

        protected const string AXIS_H = "Horizontal";
        protected const string AXIS_V = "Vertical";

        protected static readonly Vector3 PLANE = new Vector3(1, 0, 1);

        protected const string PLAYER_ID = "player";
        public static OnLoadSceneData ON_LOAD_SCENE_DATA;

        // PROPERTIES: ----------------------------------------------------------------------------

        public INPUT_TYPE inputType = INPUT_TYPE.Wsad;
        public MOUSE_BUTTON mouseButtonMove = MOUSE_BUTTON.LeftClick;
        public LayerMask mouseLayerMask = ~0;
        public bool invertAxis;

        private float mouseTimer;
        private float mouseClickTime = 0.1f;

        public KeyCode jumpKey = KeyCode.Space;

        protected bool uiConstrained;
        protected Camera cacheCamera;

        private Vector3 direction = Vector3.zero;
        private Vector3 directionVelocity = Vector3.zero;

        public bool useAcceleration = true;
        public float acceleration = 4f;
        public float deceleration = 2f;

        public GameObject RippleClickEffect = null;

        public CharacterAttribute Strength;
        public CharacterAttribute Agility;
        public CharacterAttribute Intelligence;


        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            CharacterAwake();

            initSaveData = new SaveData
            {
                position = transform.position,
                rotation = transform.rotation
            };

            if (save)
            {
                SaveLoadManager.Instance.Initialize(
                    this, (int) SaveLoadManager.Priority.Normal, true
                );
            }

            HookPlayer hookPlayer = gameObject.GetComponent<HookPlayer>();
            if (hookPlayer == null) gameObject.AddComponent<HookPlayer>();

            if (ON_LOAD_SCENE_DATA != null && ON_LOAD_SCENE_DATA.active)
            {
                transform.position = ON_LOAD_SCENE_DATA.position;
                transform.rotation = ON_LOAD_SCENE_DATA.rotation;
                ON_LOAD_SCENE_DATA.Consume();
            }
        }

        protected virtual void Update()
        {
            if (!Application.isPlaying) return;

            switch (inputType)
            {
                case INPUT_TYPE.Wsad:
                    UpdateInputDirectional();
                    break;
                case INPUT_TYPE.PointAndClick:
                    UpdateInputPointClick();
                    break;
                case INPUT_TYPE.FollowPointer:
                    UpdateInputFollowPointer();
                    break;

                case INPUT_TYPE.FollowAndClickPointer:
                    UpdateInputFollowAndClick();
                    break;
            }

            if (IsControllable())
            {
                if (Input.GetKeyDown(jumpKey)) Jump();
            }
            else
            {
                direction = Vector3.zero;
                directionVelocity = Vector3.zero;
            }

            CharacterUpdate();
        }

        protected virtual void UpdateInputDirectional()
        {
            Vector3 targetDirection = Vector3.zero;
            if (!IsControllable()) return;


            targetDirection = new Vector3(
                Input.GetAxisRaw(AXIS_H),
                0.0f,
                Input.GetAxisRaw(AXIS_V));


            ComputeMovement(targetDirection);

            Camera maincam = GetMainCamera();
            if (maincam == null) return;

            Vector3 moveDirection = maincam.transform.TransformDirection(direction);
            moveDirection.Scale(PLANE);

            moveDirection.Normalize();
            moveDirection *= direction.magnitude;

            characterLocomotion.SetDirectionalDirection(moveDirection);
        }

        protected virtual void UpdateInputFollowAndClick()
        {
            if (!IsControllable()) return;
            UpdateUIConstraints();

            if (Input.GetMouseButtonDown((int) mouseButtonMove) && !uiConstrained)
            {
                mouseTimer = 0f;
            }


            if (Input.GetMouseButton((int) mouseButtonMove) && !uiConstrained)
            {
                mouseTimer += Time.deltaTime;

                if (mouseTimer > 0.2f)
                {
                    if (HookPlayer.Instance == null) return;

                    Camera maincam = GetMainCamera();
                    if (maincam == null) return;

                    Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);

                    Transform player = HookPlayer.Instance.transform;
                    Plane groundPlane = new Plane(Vector3.up, player.position);

                    if (groundPlane.Raycast(cameraRay, out var rayDistance))
                    {
                        Vector3 cursor = cameraRay.GetPoint(rayDistance);
                        if (Vector3.Distance(player.position, cursor) >= 0.05f)
                        {
                            Vector3 target = Vector3.MoveTowards(player.position, cursor, 1f);
                            characterLocomotion.SetTarget(target, null, 0f);
                        }
                    }
                }

                UpdateRippleClickEffect(false);
            }

            if (Input.GetMouseButtonUp((int) mouseButtonMove) && !uiConstrained)
            {
                if (mouseTimer > 0.2f)
                {
                    return;
                }

                Camera maincam = GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);
                characterLocomotion.SetTarget(cameraRay, mouseLayerMask, null, 0f);

                UpdateRippleClickEffect(true);
            }

            if (characterLocomotion.navmeshAgent.enabled == false)
            {
                UpdateRippleClickEffect(false);
            }
        }

        protected virtual void UpdateInputPointClick()
        {
            if (!IsControllable()) return;
            UpdateUIConstraints();

            if (Input.GetMouseButtonDown((int) mouseButtonMove) && !uiConstrained)
            {
                Camera maincam = GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);
                characterLocomotion.SetTarget(cameraRay, mouseLayerMask, null, 0f);

                UpdateRippleClickEffect(true);
            }

            if (characterLocomotion.navmeshAgent.enabled == false)
            {
                UpdateRippleClickEffect(false);
            }
        }

        protected virtual void UpdateInputFollowPointer()
        {
            if (!IsControllable()) return;
            UpdateUIConstraints();

            if (Input.GetMouseButton((int) mouseButtonMove) && !uiConstrained)
            {
                if (HookPlayer.Instance == null) return;

                Camera maincam = GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);

                Transform player = HookPlayer.Instance.transform;
                Plane groundPlane = new Plane(Vector3.up, player.position);

                if (groundPlane.Raycast(cameraRay, out float rayDistance))
                {
                    Vector3 cursor = cameraRay.GetPoint(rayDistance);
                    if (Vector3.Distance(player.position, cursor) >= 0.05f)
                    {
                        Vector3 target = Vector3.MoveTowards(player.position, cursor, 1f);
                        characterLocomotion.SetTarget(target, null, 0f);
                    }
                }
            }
        }

        #region OTHER METHODS

        protected Camera GetMainCamera()
        {
            if (HookCamera.Instance != null) return HookCamera.Instance.Get<Camera>();
            if (cacheCamera != null) return cacheCamera;

            cacheCamera = Camera.main;
            if (cacheCamera != null)
            {
                return cacheCamera;
            }

            cacheCamera = FindObjectOfType<Camera>();
            if (cacheCamera != null)
            {
                return cacheCamera;
            }

            Debug.LogError(ERR_NOCAM, gameObject);
            return null;
        }

        protected void UpdateUIConstraints()
        {
            EventSystemManager.Instance.Wakeup();
            uiConstrained = EventSystemManager.Instance.IsPointerOverUI();
        }

        protected void ComputeMovement(Vector3 target)
        {
            switch (useAcceleration)
            {
                case true:
                    float acceleration = Mathf.Approximately(target.sqrMagnitude, 0f)
                        ? deceleration
                        : this.acceleration;

                    direction = Vector3.SmoothDamp(
                        direction, target,
                        ref directionVelocity,
                        1f / acceleration,
                        acceleration
                    );

                    if (Mathf.Abs(target.sqrMagnitude) < 0.05f &&
                        Mathf.Abs(direction.sqrMagnitude) < 0.05f)
                    {
                        direction = Vector3.zero;
                    }

                    break;

                case false:
                    direction = target;
                    break;
            }
        }


        private void UpdateRippleClickEffect(bool effectEnable)
        {
            if (RippleClickEffect == null)
            {
                return;
            }

            if (effectEnable)
            {
                RippleClickEffect.transform.position =
                    characterLocomotion.navmeshAgent.destination + new Vector3(0f, 0.1f, 0f);
                RippleClickEffect.SetActive(true);
            }
            else
            {
                RippleClickEffect.SetActive(false);
            }
        }

        #endregion


        #region GAME SAVE

        protected override string GetUniqueCharacterID()
        {
            return PLAYER_ID;
        }

        #endregion
    }
}