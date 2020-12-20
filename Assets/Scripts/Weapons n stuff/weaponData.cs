using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Enigmanormous
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Scriptables/weapon data", order = 2)]

    public class weaponData : ScriptableObject
    {
        [Header("WeaponStats")]
        [Tooltip("weapon name")]
        public string gunName = "namethis";
        [Tooltip("fire rate: 1 would be 1 per second, lower number shoots faster")]
        public float firerate = 1;


        [Tooltip("blood = accuracy for raycast")]
        public float bloom = 1;
        public float recoil = 1;
        public float kickback = 1;
        public int damage = 1;
        public float range = 1000f;
        [Tooltip("How Fast you aim if RightClickAim is true")]
        public float aimSpeed = 10;
        public float reloadTime = 2f;

        
        [Header("ammo system temporary")]
        public int ammoMax = 90; //total ammo
        private int ammoLeft; //curent ammo        
        public int clipSize = 30;//clip size
        private int clip; //left in clip
        


        [Header("True/False things to categorize guns")]
        public bool Automatic = true;
        [Tooltip("If true, you will hold the gun up to your head like cod style and zoom in")]
        public bool RightClickAim = true;
        public bool sniper = false;        
        public bool DualWielded = false;
        public bool Kinetic = true;

        [Header("Audio")]
        public AudioClip fireGunSound;
        public float pitchRandomization = 0.1f;
        public AudioClip emptyGunSound;
        public AudioClip reloadGunSound;

        [Header("Gun Prefab")]
        public GameObject prefab;

        public void Initialize()
        {
            ammoLeft = ammoMax;
            clip = clipSize;
        }

          
        public bool FireBullet()
        {
            if (clip > 0)
            {
                clip -= 1;
                return true;
            }
            else return false;
        }

        public void Reload()
        {
            ammoLeft += clip; // add what's left in your clip into stash
            clip = Mathf.Min(clipSize, ammoLeft); //grab however much you have left over for clip
            ammoLeft -= clip;// need to fill up clip, remove from stash
        }

        public int GetAmmoLeft() { return ammoLeft; }
        public int GetAmmoMax() { return ammoMax; }
        public int GetClip() { return clip; }
        public int GetClipSize() { return clipSize; }



    }
}

  