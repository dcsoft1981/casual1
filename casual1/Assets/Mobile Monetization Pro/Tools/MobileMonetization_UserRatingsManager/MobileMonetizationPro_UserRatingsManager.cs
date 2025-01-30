using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileMonetizationPro
{
    public class MobileMonetizationPro_UserRatingsManager : MonoBehaviour
    {
        public static MobileMonetizationPro_UserRatingsManager instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                PlayerPrefs.SetInt("IsAppOpened", 0);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // If an instance already exists, destroy this duplicate
                Destroy(gameObject);
            }
        }
    }
}