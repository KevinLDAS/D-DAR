using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyProfile
{
    public class LoadingViewController : MonoBehaviour
    {
        [SerializeField]
        private Transform RotatorTarget = default;
        [SerializeField]
        private float Speed = default;

        void Update()
        {
            RotatorTarget.Rotate(new Vector3(0, 0, Speed * Time.deltaTime));
        }
    }
}


