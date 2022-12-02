using UnityEngine;

public class FolderObject : MonoBehaviour {
    [SerializeField] bool keepAtOrigin = false;

    private void OnDrawGizmosSelected ()
    {
        if(keepAtOrigin)
            transform.position = Vector3.zero;
    }
}
