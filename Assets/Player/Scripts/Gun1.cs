using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun1 : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;
    
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isReloading = false;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public Animator animator;

    AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    
    void Start()
    {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        isReloading = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(isReloading)
        {
            return;
        }
        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
            audioSource.PlayOneShot(shootSound);
        }
    }

    IEnumerator Reload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            isReloading = true;
            animator.SetBool("Reloading", true);
            audioSource.PlayOneShot(reloadSound);
            yield return new WaitForSeconds(reloadTime);
            animator.SetBool("Reloading", false);
            currentAmmo = maxAmmo;
            isReloading = false;
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();

        currentAmmo--;

        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
    }
}
