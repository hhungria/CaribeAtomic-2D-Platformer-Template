using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CaribeAtomic
{
    public class Restarter : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                SceneManager.LoadScene("01", LoadSceneMode.Single);
            }
        }
    }
}
