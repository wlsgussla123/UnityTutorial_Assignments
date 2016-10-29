using UnityEngine;
using System.Collections;

public class RaycastShoot : MonoBehaviour {
    public int gunDamage = 1; // how much damage is applied to an object
    public float fireRate = .25f; // control how often the player can fire their weapon.
    public float weaponRange = 50f; // determine how far our ray will be cast into the scene.
    public float hitForce = 100f; // If our raycast intersects an object with a rigidbody component attached we will apply force to it
    public Transform gunEnd;  // mark the position at which our laser line will begin. (gunEnd will be the Transform component of an empty GameObject)  

    private Camera fpsCam; // hold a reference to our first person camera, We will use this to determine the position the player is aming from.
    private WaitForSeconds shotDuration = new WaitForSeconds(.07f); // determine how long we want the laser to remain visible in the game view once the player has fired. 
    private AudioSource gunAudio; // we will use to play our shooting sound effect when the player fires.
    private LineRenderer laserLine; // LineRenderer takes an array of 2 or more points in 3D space and draws a straight line between them in the game view.
    private float nextFire; // hold the time at which the player will be allowed to fire again after firing.
        
    // Use this for initialization
    void Start () {
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        fpsCam = GetComponentInParent<Camera>();
	}
	
	// we will handle user input, check if the player can fire and ultimately apply physics force to any GameObject with a Rigidbody hit by the ray.
	void Update () {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate; // set shooting delay
            StartCoroutine(ShotEffect());

            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;

            laserLine.SetPosition(0, gunEnd.position); // gunEnd is GameObject that we will attach to the end of our gun in the scene.
            
            // it will only be executed if our raycast hit something.
            if(Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                laserLine.SetPosition(1, hit.point);
                // check if there is a ShootableBox component attached to the GameObject that our raycast hit.
                ShootableBox health = hit.collider.GetComponent<ShootableBox>();

                if(health != null)
                {
                    health.Damage(gunDamage);
                    
                    if(hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * hitForce);
                    }
                }
            }
            else
            {
                laserLine.SetPosition(1, fpsCam.transform.forward * weaponRange);
            }
        }
    }

    // turn on and off our laser effect. 
    private IEnumerator ShotEffect()
    {
        gunAudio.Play(); // play the AudioClip assigned to gunAudio. This is a shooting sound effect that has already been assiend in the editor.
        laserLine.enabled = true;

        yield return shotDuration; // WaitforSenconds

        laserLine.enabled = false;
    }
}
