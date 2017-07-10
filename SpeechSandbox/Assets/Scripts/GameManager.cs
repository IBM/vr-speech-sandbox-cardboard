using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.IO;

public class GameManager : MonoBehaviour {

	[Header("Objects Dictionary", order = 0)]
	public List<string> _oKeys = new List<string>();
	public List<GameObject> _oValues = new List<GameObject>();
	protected Dictionary<string, GameObject> objects;
	
	[Header("AudioSources", order = 1)]
	public GvrAudioRoom musicSource;
	public GvrAudioSource voiceSource;
	public GvrAudioSource errorSource;
        public AudioClip welcomeSource;

	[Header("Objects", order = 2)]
	public GameObject mainCamera;
	
    [Header("Pointer", order = 1)]
    public GvrReticlePointer pointer;

    
	protected GameObject pointerTarget;
    private GameObject currentObject;
    private  GameObject[] gameObjects;

    #region InitAndLifecycle
    //------------------------------------------------------------------------------------------------------------------
    // Initialization and Lifecycle
    //------------------------------------------------------------------------------------------------------------------

    protected virtual void Awake()
	{
        InitializeObjects();
        PlayClip(welcomeSource);
    }
		
	protected virtual void Start () {

	}

	protected virtual void Update () {

	}

	void InitializeObjects()
	{
		Debug.Log ("Initializing Objects");
		objects = new Dictionary<string, GameObject>();

		for (var i = 0; i != Math.Min(_oKeys.Count, _oValues.Count); i++)
		{
			objects.Add(_oKeys[i], _oValues[i]);
		}
			
		_oValues.Clear();
	}

	#endregion

	#region Public
	//------------------------------------------------------------------------------------------------------------------
	// Public Interface Functions
	//------------------------------------------------------------------------------------------------------------------

	public void PlayClip(AudioClip clip)
	{
		voiceSource.clip = clip;
		voiceSource.Play();
	}

	public void PlayError(AudioClip clip)
	{
		if (!errorSource.isPlaying && !voiceSource.isPlaying)
		{
			errorSource.clip = clip;
			errorSource.Play();
		}
	}

    private GameObject FindClosestObject()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Creatable");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = pointer.reticlePointerImpl.GetLineEndPoint();

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance && curDistance < 1.0)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
    public void DestroyAtPointer() 
	{
        pointerTarget = null;
        pointerTarget = FindClosestObject();
        if (pointerTarget) {
			Destroy (pointerTarget);
		}
	}

    public void MoveObject(string direction)
    {
        GameObject pointerTarget = null;
        pointerTarget = FindClosestObject();
        Vector3 origin = new Vector3(0, 0, 0);

        if (pointerTarget){
            Vector3 currentLocation = pointerTarget.transform.position;
            if (direction == "up")
            {
                pointerTarget.transform.position = new Vector3(currentLocation.x, currentLocation.y + 5, currentLocation.z);
            }
            else if (direction == "right")
            {
                pointerTarget.transform.RotateAround(origin, Vector3.up, 20);
            }
            else if (direction == "left")
            {
                pointerTarget.transform.RotateAround(origin, Vector3.up, -20);

            }
            else if (direction == "closer")
            {
                transform.position = Vector3.MoveTowards(currentLocation, origin, 2);
            }
            else if (direction == "farther")
            {
                transform.position = Vector3.MoveTowards(currentLocation, origin, -2);
            }
            

        }
    }

	public virtual void CreateObject(string key, string matKey, string scale)
	{
		GameObject newObject = objects[key];

        // Check Pointer
        Vector3? tempDestination = pointer.reticlePointerImpl.GetLineEndPoint();
        
     	if (newObject != null && tempDestination != null)
		{
			Vector3 destination = (Vector3)tempDestination;
			Vector3 location = new Vector3(destination.x, destination.y + newObject.transform.position.y, destination.z);
       
			if (matKey != null && newObject.GetComponent<CreatableObject>().isCustomizable)
			{
				newObject.GetComponent<CreatableObject>().ApplyMaterial(matKey);
				newObject.GetComponent<CreatableObject>().matKey = matKey;
			}
            
			GameObject objectInstance = (GameObject) Instantiate(newObject, location, newObject.transform.rotation);

			if (scale != null)
			{
				objectInstance.GetComponent<CreatableObject>().ApplyScale(scale);
			}
		}
	}

    #endregion
}
