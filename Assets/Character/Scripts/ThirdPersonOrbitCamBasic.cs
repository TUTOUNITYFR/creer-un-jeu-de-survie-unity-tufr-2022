using UnityEngine;

// This class corresponds to the 3rd person camera features.
public class ThirdPersonOrbitCamBasic : MonoBehaviour 
{
	public Transform player;                                           // Player's reference.
	public Vector3 pivotOffset = new Vector3(0.0f, 1.7f,  0.0f);       // Offset to repoint the camera.
	public Vector3 camOffset   = new Vector3(0.0f, 0.0f, -3.0f);       // Offset to relocate the camera related to the player position.
	public float smooth = 10f;                                         // Speed of camera responsiveness.
	public float horizontalAimingSpeed = 6f;                           // Horizontal turn speed.
	public float verticalAimingSpeed = 6f;                             // Vertical turn speed.
	public float maxVerticalAngle = 30f;                               // Camera max clamp angle. 
	public float minVerticalAngle = -60f;                              // Camera min clamp angle.
	public string XAxis = "Analog X";                                  // The default horizontal axis input name.
	public string YAxis = "Analog Y";                                  // The default vertical axis input name.

	private float angleH = 0;                                          // Float to store camera horizontal angle related to mouse movement.
	private float angleV = 0;                                          // Float to store camera vertical angle related to mouse movement.
	private Transform cam;                                             // This transform.
	private Vector3 smoothPivotOffset;                                 // Camera current pivot offset on interpolation.
	private Vector3 smoothCamOffset;                                   // Camera current offset on interpolation.
	private Vector3 targetPivotOffset;                                 // Camera pivot offset target to iterpolate.
	private Vector3 targetCamOffset;                                   // Camera offset target to interpolate.
	private float defaultFOV;                                          // Default camera Field of View.
	private float targetFOV;                                           // Target camera Field of View.
	private float targetMaxVerticalAngle;                              // Custom camera max vertical clamp angle.
	private bool isCustomOffset;                                       // Boolean to determine whether or not a custom camera offset is being used.

	// Get the camera horizontal angle.
	public float GetH { get { return angleH; } }

	void Awake()
	{
		// Reference to the camera transform.
		cam = transform;

		// Set camera default position.
		cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
		cam.rotation = Quaternion.identity;

		// Set up references and default values.
		smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;
		defaultFOV = cam.GetComponent<Camera>().fieldOfView;
		angleH = player.eulerAngles.y;

		ResetTargetOffsets ();
		ResetFOV ();
		ResetMaxVerticalAngle();

		// Check for no vertical offset.
		if (camOffset.y > 0)
			Debug.LogWarning("Vertical Cam Offset (Y) will be ignored during collisions!\n" +
				"It is recommended to set all vertical offset in Pivot Offset.");
	}

	void Update()
	{
		// Get mouse movement to orbit the camera.
		// Mouse:
		angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * horizontalAimingSpeed;
		angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * verticalAimingSpeed;
		// Joystick:
		angleH += Mathf.Clamp(Input.GetAxis(XAxis), -1, 1) * 60 * horizontalAimingSpeed * Time.deltaTime;
		angleV += Mathf.Clamp(Input.GetAxis(YAxis), -1, 1) * 60 * verticalAimingSpeed * Time.deltaTime;

		// Set vertical movement limit.
		angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);

		// Set camera orientation.
		Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
		Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
		cam.rotation = aimRotation;

		// Set FOV.
		cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp (cam.GetComponent<Camera>().fieldOfView, targetFOV,  Time.deltaTime);

		// Test for collision with the environment based on current camera position.
		Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
		Vector3 noCollisionOffset = targetCamOffset;
		while (noCollisionOffset.magnitude >= 0.2f)
		{
			if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset))
				break;
			noCollisionOffset -= noCollisionOffset.normalized * 0.2f;
		}
		if (noCollisionOffset.magnitude < 0.2f)
			noCollisionOffset = Vector3.zero;

		// No intermediate position for custom offsets, go to 1st person.
		bool customOffsetCollision = isCustomOffset && noCollisionOffset.sqrMagnitude < targetCamOffset.sqrMagnitude;

		// Repostition the camera.
		smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, customOffsetCollision ? pivotOffset : targetPivotOffset, smooth * Time.deltaTime);
		smoothCamOffset = Vector3.Lerp(smoothCamOffset, customOffsetCollision ? Vector3.zero : noCollisionOffset, smooth * Time.deltaTime);

		cam.position =  player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
	}

	// Set camera offsets to custom values.
	public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
	{
		targetPivotOffset = newPivotOffset;
		targetCamOffset = newCamOffset;
		isCustomOffset = true;
	}

	// Reset camera offsets to default values.
	public void ResetTargetOffsets()
	{
		targetPivotOffset = pivotOffset;
		targetCamOffset = camOffset;
		isCustomOffset = false;
	}

	// Reset the camera vertical offset.
	public void ResetYCamOffset()
	{
		targetCamOffset.y = camOffset.y;
	}

	// Set camera vertical offset.
	public void SetYCamOffset(float y)
	{
		targetCamOffset.y = y;
	}

	// Set camera horizontal offset.
	public void SetXCamOffset(float x)
	{
		targetCamOffset.x = x;
	}

	// Set custom Field of View.
	public void SetFOV(float customFOV)
	{
		this.targetFOV = customFOV;
	}

	// Reset Field of View to default value.
	public void ResetFOV()
	{
		this.targetFOV = defaultFOV;
	}

	// Set max vertical camera rotation angle.
	public void SetMaxVerticalAngle(float angle)
	{
		this.targetMaxVerticalAngle = angle;
	}

	// Reset max vertical camera rotation angle to default value.
	public void ResetMaxVerticalAngle()
	{
		this.targetMaxVerticalAngle = maxVerticalAngle;
	}

	// Double check for collisions: concave objects doesn't detect hit from outside, so cast in both directions.
	bool DoubleViewingPosCheck(Vector3 checkPos)
	{
		return ViewingPosCheck (checkPos) && ReverseViewingPosCheck (checkPos);
	}

	// Check for collision from camera to player.
	bool ViewingPosCheck (Vector3 checkPos)
	{
		// Cast target and direction.
		Vector3 target = player.position + pivotOffset;
		Vector3 direction = target - checkPos;
		// If a raycast from the check position to the player hits something...
		if (Physics.SphereCast(checkPos, 0.2f, direction, out RaycastHit hit, direction.magnitude))
		{
			// ... if it is not the player...
			if(hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				// This position isn't appropriate.
				return false;
			}
		}
		// If we haven't hit anything or we've hit the player, this is an appropriate position.
		return true;
	}

	// Check for collision from player to camera.
	bool ReverseViewingPosCheck(Vector3 checkPos)
	{
		// Cast origin and direction.
		Vector3 origin = player.position + pivotOffset;
		Vector3 direction = checkPos - origin;
		if (Physics.SphereCast(origin, 0.2f, direction, out RaycastHit hit, direction.magnitude))
		{
			if(hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
			{
				return false;
			}
		}
		return true;
	}

	// Get camera magnitude.
	public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
	{
		return Mathf.Abs ((finalPivotOffset - smoothPivotOffset).magnitude);
	}
}
