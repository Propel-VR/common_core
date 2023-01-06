using Autohand;
using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class LinkCableGenerator : EditorWindow
{


    Rigidbody _fromPoint;
    ConfigurableJoint _toPoint;

    float _linkSize;
    int _numLinks;
    
    GameObject _linkPrefab;

    List<GameObject> _linkSegments = new List<GameObject>();


    [MenuItem("Tools/Link Cable Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(LinkCableGenerator));
    }


    private void OnGUI()
    {
        GUILayout.Label("Link Cable Generator", EditorStyles.boldLabel);

        GUILayout.Label("\n\n");

        _fromPoint = EditorGUILayout.ObjectField("From RigidBody", _fromPoint, typeof(Rigidbody)) as Rigidbody;
        _toPoint = EditorGUILayout.ObjectField("To Joint", _toPoint, typeof(ConfigurableJoint)) as ConfigurableJoint;

        _linkSize = EditorGUILayout.Slider("Link Size", _linkSize, 0.04f, 1.5f);
        _numLinks = EditorGUILayout.IntField("Number of Links", _numLinks);

        _linkPrefab = EditorGUILayout.ObjectField("Link Prefab", _linkPrefab, typeof(GameObject), false) as GameObject;
        GUILayout.Label("\n");

        if(GUILayout.Button("Generate Cable"))
        {
            GenerateCable();
        }
    }

    public void GenerateCable()
    {
        Rigidbody lastPoint = _fromPoint;

        //setupFirstLink
        //GameObject firstLink = GameObject.Instantiate(_linkPrefab, Vector3.zero, Quaternion.identity);
        GameObject firstLink = GameObject.Instantiate(_linkPrefab, lastPoint.transform.position+lastPoint.transform.up.normalized* (2 * _linkSize), Quaternion.identity);

        //firstLink.GetComponent<LinkSegment>().SetupAsFirstSegment(_linkSize, lastPoint);

        SetupAsFirstSegment(firstLink, lastPoint);

        lastPoint = firstLink.GetComponent<Rigidbody>();

        _linkSegments.Add(firstLink);


        for (int i = 1; i < _numLinks; i++)
        {

            //instantiate more links
            //GameObject go = GameObject.Instantiate(_linkPrefab, Vector3.zero, Quaternion.identity);
            GameObject go =GameObject.Instantiate(_linkPrefab, lastPoint.transform.position+lastPoint.transform.up.normalized *(2* _linkSize), Quaternion.identity);

            go.GetComponent<LinkSegment>().SetUpVisuals(lastPoint.transform);

            SetupSegment(go, lastPoint);


            lastPoint = go.GetComponent<Rigidbody>();

            _linkSegments.Add(go);


        }

        if (_toPoint)
        {
            _toPoint.connectedBody = lastPoint;
            _toPoint.GetComponent<LinkSegment>().SetUpVisuals(lastPoint.transform);
        }
       
    }

    private void SetupSegment(GameObject linkGO, Rigidbody connectedBody)
    {
        linkGO.transform.localScale = new Vector3(linkGO.transform.localScale.x, _linkSize, linkGO.transform.localScale.z);

        ConfigurableJoint joint = linkGO.GetComponent<ConfigurableJoint>();

        if (connectedBody)
        {
            joint.connectedBody = connectedBody;

            /*
            _joint.angularXMotion = ConfigurableJointMotion.Locked;
            _joint.angularYMotion = ConfigurableJointMotion.Locked;
            _joint.angularZMotion = ConfigurableJointMotion.Locked;

            */
        }
    }

    private void SetupAsFirstSegment(GameObject linkGO, Rigidbody connectedBody)
    {
        linkGO.transform.localScale = new Vector3(linkGO.transform.localScale.x, _linkSize, linkGO.transform.localScale.z);

        ConfigurableJoint joint = linkGO.GetComponent<ConfigurableJoint>();

        if (connectedBody)
        {
            joint.connectedBody = connectedBody;
            joint.connectedAnchor = Vector3.zero;
            /*
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
            */
            //connectedBody.transform.localRotation = Quaternion.Euler(Vector3.up * -90);
        }
    }

    private void ConnectJointedBodies()
    {

        foreach(GameObject go in _linkSegments)
        {
            Grabbable grabbable = go.GetComponent<Grabbable>();

            foreach (GameObject link in _linkSegments)
            { 
        
                if(link!=go)
                    grabbable.jointedBodies.Add(link.GetComponent<Rigidbody>());
                
            }

        }

    }


}
