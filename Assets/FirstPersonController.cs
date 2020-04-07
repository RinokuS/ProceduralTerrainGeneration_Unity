using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {

	[Range(1f, 5f)]
	public float MovementSpeed = 1f;

	[Range(1, 200f)]
	public float LookSensitivity = 10f;

	[Range(1, 100f)]
	public float JumpStrength = 2f;

	private CharacterController characterController;
	private Transform cameraTransform;

	private float cameraTilt = 0f;
	private float verticalSpeed = 0f;
	private float timeInAir = 0f;
	private bool jumpLocked = false;

	private float hitdistance;
	private bool isGrounded;

	void Start () {
		this.characterController = this.GetComponent<CharacterController>();
		this.cameraTransform = this.GetComponentInChildren<Camera>().transform;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Confined;
	}
	
	void Update () {
		onGroundMy();
		bool touchesGround = isGrounded;
		float runMultiplier = 1f + 2f * Input.GetAxis("Run");
		float y = this.transform.position.y;
		Vector3 movementVector = this.transform.forward * Input.GetAxis("Vertical") + this.transform.right * Input.GetAxis("Horizontal");
		if (movementVector.sqrMagnitude > 1) { // this check prevents partial joystick input from becoming 100% speed
			movementVector.Normalize();  // this prevents diagonal movement form being too fast
		}
		this.characterController.Move(movementVector * Time.deltaTime * this.MovementSpeed * runMultiplier);
		float verticalMovement = this.transform.position.y - y;
		if (verticalMovement < 0) {
			this.transform.position += Vector3.down * verticalMovement;
		}
		this.transform.rotation = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * Time.deltaTime * this.LookSensitivity, Vector3.up) * this.transform.rotation;
		this.cameraTilt = Mathf.Clamp(this.cameraTilt - Input.GetAxis("Mouse Y") * this.LookSensitivity * Time.deltaTime, -90f, 90f);
		this.cameraTransform.localRotation = Quaternion.AngleAxis(this.cameraTilt, Vector3.right);

		if (touchesGround) {
			this.timeInAir = 0;
		} else {
			this.timeInAir += Time.deltaTime;
		}

		if (touchesGround && this.verticalSpeed < 0) {
			this.verticalSpeed = 0;
		} else {
			this.verticalSpeed -= 9.18f * Time.deltaTime;
		}
		if (Input.GetAxisRaw("Jump") < 0.1f) {
			this.jumpLocked = false;
		}
		if (!this.jumpLocked && this.timeInAir < 0.5f && Input.GetAxisRaw("Jump") > 0.1f) {
			this.timeInAir = 0.5f;
			this.verticalSpeed = this.JumpStrength;
			this.jumpLocked = true;
		}
		
//		if (Input.GetAxisRaw("Jetpack") > 0.1f) {
//			this.verticalSpeed = 2f;
//		}
		this.characterController.Move(Vector3.up * Time.deltaTime * this.verticalSpeed);

//		if (Input.GetKeyDown(FlightController.OnOffKey)) {
//			var flyBehaviour = this.GetComponent<FlightController>();
//			if (flyBehaviour != null) {
//				this.GetComponent<FlightController>().enabled = true;
//			}
//			this.cameraTilt = 24;
//		}
	}

	public void Enable() {
		this.verticalSpeed = 0;
	}

	private bool onGround() {
		var ray = new Ray(this.transform.position, Vector3.down);
		return Physics.SphereCast(ray, this.characterController.radius, this.characterController.height / 2 - this.characterController.radius + 0.1f);
	}

	private void onGroundMy()
	{
		if (isGrounded)
			hitdistance = 0.35f;
		else
			hitdistance = 0.15f;

		if (Physics.Raycast(transform.position - new Vector3(0, 0.85f, 0), -transform.up, hitdistance))
			isGrounded = true;
		else
			isGrounded = false;
	}
}
