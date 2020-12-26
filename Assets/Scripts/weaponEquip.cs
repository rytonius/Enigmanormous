using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

namespace Com.Enigmanormous
{

    public class weaponEquip : MonoBehaviourPunCallbacks
    {
        #region Variables
        public weaponData[] loadout;
        [HideInInspector] public weaponData currentGunData;
        public Transform weaponParent;
        public GameObject bulletholePrefab;
        public GameObject bloodbulletholePrefab;
        public LayerMask canBeShot;
        public LineRenderer bulletTrail;


        public AudioSource fireSfx;
        
        public AudioSource reloadSfx;
        private int currentIndex;
        private GameObject currentWeapon;
        private float currentFireRate;
        private float currentRange;
        public bool currentlyAiming = false;
        public bool currentlySniping = false;
        private bool isReloading = false;
        private int cList;

        #endregion
        // Start is called before the first frame update
        void Start()
        {
            if (this.photonView)
            {
                foreach (weaponData a in loadout) a.Initialize();
                Equip(0);

                // this will need to be updated when inventory is figured out
                int length = loadout.Length;
                cList = length;

                Debug.Log("current weapon list: " + cList);
            }
                        
        }


        // Update is called once per frame
        void Update()
        {
            
            //equip that weapon
            selectEquipControls();
            FireGunBro();
            ReloadYourCurrentGun();
            //weapon position elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

        }

        #region private void

        //when you press left or right mouse you will aim or start firing like a ganster
        private void FireGunBro()
        {
            if (currentWeapon != null)
            {
                if (photonView.IsMine)
                {
                    Aim(Input.GetMouseButton(1));

                    if (loadout[currentIndex].Automatic)
                    {
                        if (Input.GetMouseButton(0) && currentFireRate <= 0)
                        {

                            if (loadout[currentIndex].FireBullet()) photonView.RPC("shoot", RpcTarget.All);
                            else
                            {
                                if (currentGunData.emptyGunSound)
                                {
                                    fireSfx.Stop();
                                    fireSfx.clip = currentGunData.emptyGunSound;

                                    fireSfx.Play();

                                }
                            }
                            //cooldown between bullets
                            currentFireRate += loadout[currentIndex].firerate;
                        }
                    }

                    else
                    {
                        if (Input.GetMouseButtonDown(0) && currentFireRate <= 0)
                        {

                            if (loadout[currentIndex].FireBullet()) photonView.RPC("shoot", RpcTarget.All);
                            else
                            {
                                if (currentGunData.emptyGunSound)
                                {
                                    fireSfx.Stop();
                                    fireSfx.clip = currentGunData.emptyGunSound;

                                    fireSfx.Play();

                                }
                            }
                            //cooldown between bullets
                            currentFireRate += loadout[currentIndex].firerate;
                        }
                    }
                    if (currentFireRate > 0) currentFireRate -= Time.deltaTime;

                }
            }
        }

        IEnumerator Reloading(float playerWait)
        {
            isReloading = true;
            
            reloadSfx.clip = currentGunData.reloadGunSound;

            reloadSfx.Play();
            currentWeapon.SetActive(false);
            yield return new WaitForSeconds(playerWait);
            loadout[currentIndex].Reload();
            currentWeapon.SetActive(true);
            isReloading = false;
            
        }

        private void ReloadYourCurrentGun()
        {
            if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reloading(loadout[currentIndex].reloadTime));
        }

        [PunRPC]
        void Equip(int weaponIndex)
        {


            if (currentWeapon != null)
            {
                if (isReloading) StopCoroutine("Reloading");
                Destroy(currentWeapon);
            }

            currentIndex = weaponIndex;

            GameObject newWeapon = Instantiate(loadout[weaponIndex].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            newWeapon.transform.localPosition = new Vector3(0, 0, 0);
            newWeapon.transform.localEulerAngles = new Vector3(0, 0, 0);
            newWeapon.GetComponent<WeaponSway>().enabled = photonView.IsMine;

            currentWeapon = newWeapon;
            currentFireRate = 0.5f;
            currentRange = loadout[weaponIndex].range;
            currentGunData = loadout[weaponIndex];
            
        }

        

        [PunRPC]
        private void TakeDamage(int playerDamage)
        {
            GetComponent<playerController>().TakeDamage(playerDamage);
        }

        private void SpawnBulletTrailer(Vector3 hitPoint)
        {
            Transform t_spawn_trail = currentWeapon.transform.Find("Anchor/Design/PewPew");
            
            GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, t_spawn_trail.position, Quaternion.identity);
            LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();
            lineR.SetPosition(0, t_spawn_trail.position);
            lineR.SetPosition(1, hitPoint);
            Destroy(bulletTrailEffect, 1f);
        }

        void selectEquipControls()
        {

            if (photonView.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0)) photonView.RPC("Equip", RpcTarget.AllBuffered, 0); 
                if (Input.GetKeyDown(KeyCode.Alpha1)) photonView.RPC("Equip", RpcTarget.AllBuffered, 1);
                if (Input.GetKeyDown(KeyCode.Alpha2)) photonView.RPC("Equip", RpcTarget.AllBuffered, 2);
                if (Input.GetKeyDown(KeyCode.Alpha3)) photonView.RPC("Equip", RpcTarget.AllBuffered, 3);
                if (Input.GetKeyDown(KeyCode.Alpha4)) photonView.RPC("Equip", RpcTarget.AllBuffered, 4);
                if (Input.GetKeyDown(KeyCode.Alpha5)) photonView.RPC("Equip", RpcTarget.AllBuffered, 5);
                //if (Input.GetKeyDown(KeyCode.Alpha6)) photonView.RPC("Equip", RpcTarget.AllBuffered, 6);
                float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
                //scroll up
                if (mouseWheel > 0)
                {
                   // Debug.Log(mouseWheel);
                    int wIndex = currentIndex;
                    //Debug.Log("current weapons index: " + wIndex);
                   // Debug.Log("clist :" + cList);
                    if (wIndex >= cList - 1) wIndex = 0; else wIndex++;
                        photonView.RPC("Equip", RpcTarget.AllBuffered, wIndex);
                        //Debug.Log("new weapon: " + wIndex);
                    
             
                }
                //scroll down
                if (mouseWheel < 0)
                {
                   // Debug.Log(mouseWheel);
                    int wIndex = currentIndex;
                   // Debug.Log("current weapons index: " + wIndex);
                   // Debug.Log("clist :" + cList);
                    if (wIndex <= 0) wIndex = cList - 1; else wIndex--;
                        photonView.RPC("Equip", RpcTarget.AllBuffered, wIndex);
                       // Debug.Log("new weapon: " + wIndex);
                    

                }
            }
            
        }
        #endregion

        #region Public Void
        public void Aim(bool playerAiming)
        {

            Transform aimWeaponAnchor = currentWeapon.transform.Find("Anchor");
            Transform aimWeaponADS = currentWeapon.transform.Find("States/ADS");
            Transform aimWeaponHip = currentWeapon.transform.Find("States/Hip");

            if (playerAiming)
            {
                //ADS sniping
                if (loadout[currentIndex].sniper)
                {

                    aimWeaponAnchor.position = Vector3.Lerp(aimWeaponAnchor.position, aimWeaponADS.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
                    currentlyAiming = true;
                    currentlySniping = true;
                }
                //ADS
                else
                {

                    aimWeaponAnchor.position = Vector3.Lerp(aimWeaponAnchor.position, aimWeaponADS.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
                    currentlyAiming = true;
                    currentlySniping = false;

                }


            }
            else
            {
                //Hip
                aimWeaponAnchor.position = Vector3.Lerp(aimWeaponAnchor.position, aimWeaponHip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
                currentlyAiming = false;
                currentlySniping = false;

            }
        }

        [PunRPC]
        public void shoot()
        {

            // this will have your bullets end up in the center
            Transform t_spawn = transform.Find("Cameras/PlayerCamera-PP");

            //bloom (accuracy)

            Vector3 t_bloom = t_spawn.position + t_spawn.forward * currentRange;
            t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
            t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
            t_bloom -= t_spawn.position;
            t_bloom.Normalize();

            
            //raycast

            RaycastHit t_hit = new RaycastHit();
            if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, currentRange, canBeShot))
            {
                int layer = t_hit.transform.gameObject.layer;
                Debug.Log("Touched Object: " + t_hit.transform.gameObject.name + " -- layer is: " + layer);

                if (layer != 29 || layer != 26)
                {

                    SpawnBulletTrailer(t_hit.point);
                    GameObject t_newHole = Instantiate(bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                    t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                    // this should put the decal on the object you hit
                    t_newHole.transform.parent = t_hit.transform;
                    Destroy(t_newHole, 60f);


                }
                //shooting other players
                if (photonView.IsMine)
                {
                    if (t_hit.collider.gameObject.layer == 26) // layer 26 is other players
                    {
                        SpawnBulletTrailer(t_hit.point);
                        //Instantiate(bloodbulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity);
                        GameObject t_newHole = Instantiate(bloodbulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                        t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                        // this should put the decal on the object you hit
                        t_newHole.transform.parent = t_hit.transform;
                        Destroy(t_newHole, 5f);

                        //RPC cull to damage player goes here
                        t_hit.collider.transform.root.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage);
                    }
                }
                //
                if (currentGunData.fireGunSound)
                {
                    fireSfx.Stop();
                    fireSfx.clip = currentGunData.fireGunSound;
                    fireSfx.pitch = 1 - currentGunData.pitchRandomization + Random.Range(-currentGunData.pitchRandomization, currentGunData.pitchRandomization);
                    fireSfx.Play();
                    
                }
                


            }
            else
            {
                SpawnBulletTrailer(t_bloom);
            }

            //gun fx
            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);

            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;

            //cooldown (fire rate)

        }
        public void RefreshClip(TextMeshProUGUI p_ctext)
        {
            int t_clip = loadout[currentIndex].GetClip();
            int t_stache = loadout[currentIndex].GetClipSize();

            p_ctext.text = "C: " + t_clip.ToString() + " / " + t_stache.ToString();
        }

        public void RefreshAmmo(TextMeshProUGUI p_atext)
        {
            int t_ammoleft = loadout[currentIndex].GetAmmoLeft();
            int t_ammoMax = loadout[currentIndex].GetAmmoMax();

            p_atext.text = "A: " + t_ammoleft.ToString() + " / " + t_ammoMax.ToString();
        }

        
        #endregion
    }
}
