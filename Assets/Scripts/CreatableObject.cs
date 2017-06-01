using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CreatableObject : MonoBehaviour {

	[Header("Creatable Object", order = 0)]

	public string id;
	public string matKey;

	public bool isGrabbed = false;
	public float throwVelocity = 10.0f;

	public bool isCustomizable = false;
	public bool isLarge = false;
	public bool isFreezable = true;
	public bool isFood = false;
	public float minScale;
	public float maxScale;
	public float minDistance;
	public float maxDistance;
	public AudioClip collisionClip;
	public AudioClip consumeClip;

	[Header("Materials Dictionary", order = 1)]
	public List<string> _mKeys = new List<string>();
	public List<Material> _mValues = new List<Material>();
	protected Dictionary<string, Material> mats;

	protected bool defaultPrecision;
	public bool isFrozen = false;

	protected Vector3 initialScale;

	private GameObject grabbingObject;
	private bool originalKinematicState;

	#region InitAndLifecycle
	//------------------------------------------------------------------------------------------------------------------
	// Initialization and Lifecycle
	//------------------------------------------------------------------------------------------------------------------

	// Use this for initialization
	protected virtual void Start () 
	{
		initialScale = transform.localScale;
	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		if (transform.position.y < -50)
		{
			Destroy(gameObject);
		}
	}

	protected virtual void InitializeMaterials()
	{
		mats = new Dictionary<string, Material>();

		for (var i = 0; i != Math.Min(_mKeys.Count, _mValues.Count); i++)
		{
			mats.Add(_mKeys[i], _mValues[i]);
		}
	}
	#endregion

	#region Modifying Object
	//------------------------------------------------------------------------------------------------------------------
	// Modifying Object
	//------------------------------------------------------------------------------------------------------------------
	public virtual void ApplyMaterial(string matKey)
	{ 
		if (mats == null)
		{
			InitializeMaterials();
		}

		if (mats.ContainsKey(matKey))
		{
			Material mat = mats[matKey];
			GetComponent<Renderer>().material = mat;
		} else
		{
			Debug.Log("Invlid Material for Object");
		}
	}

	public virtual void ApplyScale(string scale)
	{
		float currentMass = GetComponent<Rigidbody>().mass;
		if (scale == "big")
		{
			if (maxScale > 0)
			{
				transform.localScale = new Vector3(maxScale, maxScale, maxScale);
				GetComponent<Rigidbody>().mass = currentMass * maxScale * 10;
			} else
			{
				transform.localScale = new Vector3(transform.localScale.x * 2, transform.localScale.y * 2, transform.localScale.z * 2);
				GetComponent<Rigidbody>().mass = currentMass * 20;
			}
		} else if (scale == "small")
		{
			if (minScale > 0)
			{
				transform.localScale = new Vector3(minScale, minScale, minScale);
				GetComponent<Rigidbody>().mass = currentMass / (minScale * 10);
			} else
			{
				transform.localScale = new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, transform.localScale.z / 2);
				GetComponent<Rigidbody>().mass = currentMass / 20;
			}
		}
	}
	#endregion

	#region Touching
	//------------------------------------------------------------------------------------------------------------------
	// Touching Object
	//------------------------------------------------------------------------------------------------------------------
	public virtual void Touch(GameObject touchingObject) {

	}

	public virtual void StopTouch(GameObject touchingObject) {

	}

	#endregion

	#region Grabbing
	//------------------------------------------------------------------------------------------------------------------
	// Grabbing Object
	//------------------------------------------------------------------------------------------------------------------

	public virtual void Grab(GameObject grabbingObject) {
		this.grabbingObject = grabbingObject;
		gameObject.transform.parent = grabbingObject.transform;
		Rigidbody rb = GetComponent<Rigidbody> ();
		originalKinematicState = rb.isKinematic;
		rb.isKinematic = true;
		//grabbingObject.GetComponent<ControllerListener> ().isHolding = true;
		isGrabbed = true;
	}

	public virtual void Release(GameObject grabbingObject) {
		this.grabbingObject = null;
		gameObject.transform.parent = null;
		GetComponent<Rigidbody> ().isKinematic = originalKinematicState;
		//grabbingObject.GetComponent<ControllerListener> ().isHolding = false;
		isGrabbed = false;
		//ThrowReleasedObject ();
	}

	private void ThrowReleasedObject()
	{
		Transform origin = gameObject.transform;
		Vector3 angularVelocity = GvrController.Gyro;
		Rigidbody rigidBody = GetComponent<Rigidbody>();

		Debug.Log ("GYRO: " + angularVelocity);

		if (origin != null)
		{
			rigidBody.velocity = new Vector3 (throwVelocity, throwVelocity, throwVelocity);
			rigidBody.angularVelocity = origin.TransformDirection(angularVelocity);
		}
		else
		{
			rigidBody.velocity = new Vector3 (throwVelocity, throwVelocity, throwVelocity);
			rigidBody.angularVelocity = angularVelocity;
		}
	}

	#endregion
}
