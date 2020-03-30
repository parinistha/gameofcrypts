using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour
{
	public Camera firstPersonCamera;
	private Anchor anchor;
	private DetectedPlane detectedPlane;
	private float yOffset;
	
	void CreateAnchor()
{
    // Create the position of the anchor by raycasting a point towards
    // the top of the screen.
    Vector2 pos = new Vector2 (Screen.width * .5f, Screen.height * .90f);
    Ray ray = firstPersonCamera.ScreenPointToRay (pos);
    Vector3 anchorPosition = ray.GetPoint (5f);

    // Create the anchor at that point.
    if (anchor != null) {
      DestroyObject (anchor);
    }
    anchor = detectedPlane.CreateAnchor (
        new Pose (anchorPosition, Quaternion.identity));

    // Attach the scoreboard to the anchor.
    transform.position = anchorPosition;
    transform.SetParent (anchor.transform);

    // Record the y offset from the plane.
    yOffset = transform.position.y - detectedPlane.CenterPose.position.y;

    // Finally, enable the renderers.
    foreach (Renderer r in GetComponentsInChildren<Renderer>())
    {
        r.enabled = true;
    }
}

	public void SetSelectedPlane(DetectedPlane detectedPlane)
	{
    	this.detectedPlane = detectedPlane;
    	CreateAnchor();
	}
    // Start is called before the first frame update
    void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
    	{
        	r.enabled = false;
    	}
    }

    // Update is called once per frame
    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
		{
    		return;
		}
		if (detectedPlane == null)
		{
    		return;
		}

		while (detectedPlane.SubsumedBy != null)
		{
    		detectedPlane = detectedPlane.SubsumedBy;
		}
		
		transform.LookAt (firstPersonCamera.transform); 
		// Move the position to stay consistent with the plane.
		transform.position = new Vector3(transform.position.x,
            detectedPlane.CenterPose.position.y + yOffset, transform.position.z);
    }
}
