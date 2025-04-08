using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  MusicTogether.Level
{
    [DisallowMultipleComponent]
    public class BasicCameraFollower : MonoBehaviour
    {
        public Transform target;
        public Transform rotator;
        public Camera TargetCamera { get; set; }
        
        public Vector3 followSpeed = new(1.2f, 3f, 6f);
        private Transform _origin;
        private Vector3 Translation => target.position - transform.position;

        public bool follow = true,followBeforeStart = true;
        public bool smooth = true;
        // Start is called before the first frame update
        void Awake()
        {
            TargetCamera = GetComponentInChildren<Camera>();
            _origin = new GameObject("CameraMovementOrigin")
            {
                transform =
                {
                    position = Vector3.zero,
                    rotation = Quaternion.Euler(Vector3.zero),//ԭ����0,45,0
                    localScale = Vector3.one
                }
            }.transform;
        }

        // Update is called once per frame
        void Update()
        {
            var translation = new Vector3(Translation.x * Time.smoothDeltaTime * followSpeed.x,
                Translation.y * Time.smoothDeltaTime * followSpeed.y,
                Translation.z * Time.smoothDeltaTime * followSpeed.z);
            if (followBeforeStart || follow)
                transform.Translate(smooth ? translation : Translation, _origin);

        }
    }

}
