#if (UNITY_EDITOR) 
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class CloudVolumeController : MonoBehaviour
    {
        [SerializeField] private bool drawSimulationSpace = false;
        [SerializeField] private bool drawActivationRadius = false;

        SphereCollider col; 

        public void ToggleGizmos() 
        {
            if(!drawActivationRadius && !drawActivationRadius) 
            {
                drawSimulationSpace = !drawSimulationSpace;
                drawActivationRadius = !drawActivationRadius;
            }
            else
            {
                drawSimulationSpace = false;
                drawActivationRadius = false;
            }
        }

        public void DisableGizmos() 
        {
            drawSimulationSpace = false;
            drawActivationRadius = false;
        }

        public void EnableGizmos() 
        {
            drawSimulationSpace = true;
            drawActivationRadius = true;
        }

        void Start() 
        {
            col = this.GetComponent<SphereCollider>();
        }

        void OnDrawGizmos()
        {
            if(drawSimulationSpace) 
            {
                // Note: Does not update with scale lol
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawWireCube(transform.position + Vector3.Scale(new Vector3(20.0f, 20.0f, 20.0f), transform.lossyScale), Vector3.Scale(new Vector3(40.0f, 40.0f, 40.0f), transform.lossyScale));
            }
            if(drawActivationRadius) 
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawWireSphere(transform.position + Vector3.Scale(col.center, transform.lossyScale), col.radius * transform.lossyScale.x);
            }
        }
    }
#endif