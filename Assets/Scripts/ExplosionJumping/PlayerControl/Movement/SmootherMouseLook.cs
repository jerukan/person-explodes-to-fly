using UnityEngine;

/// <summary>
/// Ripped straight from https://forum.unity.com/threads/a-free-simple-smooth-mouselook.73117/
/// </summary>
[AddComponentMenu("Camera/Simple Smooth Mouse Look")]
public class SmootherMouseLook : MonoBehaviour {

    [Tooltip("Clamps each value from [-value, value]. X represents yaw and Y represents pitch.")]
    public Vector2 clampInDegrees = new Vector2(360, 179);
    [Tooltip("Determines whether to actively lock the cursor (keep true if you aren't modifying the cursor lock somewhere else).")]
    public bool lockCursor = true;
    [Tooltip("X and Y sensetivity of the mouse.")]
    public Vector2 sensitivity = new Vector2(2, 2);
    [Tooltip("How much to smooth the camera rotation by. Set to 1 to effectively have no smoothing.")]
    public Vector2 smoothing = new Vector2(1, 1);
    [HideInInspector] public Vector2 targetDirection;
    [HideInInspector] public Vector2 targetCharacterDirection;

    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    [Tooltip("Assign this if there's a parent object controlling motion, such as a Character Controller. " +
             "Yaw rotation will affect this object instead of the camera if set.")]
    public GameObject characterBody;
    [Tooltip("Determines whether the parent object body should be rotated. " +
             "Rotation will only affect the camera if the character body exists and this is false.")]
    public bool shouldRotateCharacterBody;

    private Vector2 _mouseAbsolute;
    private Vector2 _smoothMouse;
    private bool cursorIsLocked = true;

    void Start() {
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        if (characterBody) {
            targetCharacterDirection = characterBody.transform.localRotation.eulerAngles;
        }
    }

    void Update() {
        // Don't update anything if the game is paused.
        if (System.Math.Abs(Time.timeScale) < float.Epsilon) return;

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        if (characterBody && shouldRotateCharacterBody) {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
            characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
        }
        else {
            var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
            transform.localRotation *= yRotation;
        }

        UpdateCursorLock();
    }

    public void UpdateCursorLock() {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor) {
            InternalLockUpdate();
        }
    }

    private void InternalLockUpdate() {
        if(Input.GetKeyUp(KeyCode.Escape)) {
            cursorIsLocked = !cursorIsLocked;
        }
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            cursorIsLocked = true;
        }
        if (cursorIsLocked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!cursorIsLocked) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}