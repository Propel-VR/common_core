using Autohand;
using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor window used to gemnerate a link cable from a link prefab
/// </summary>
public class LinkCableGenerator : EditorWindow
{

    #region private fields
    Rigidbody _fromPoint;
    ConfigurableJoint _toPoint;

    float _linkSize;
    int _numLinks;
    
    GameObject _linkPrefab;

    List<GameObject> _linkSegments = new List<GameObject>();
    #endregion


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

    /// <summary>
    /// Called on button press to generate 
    /// </summary>
    public void GenerateCable()
    {
        Rigidbody lastPoint = _fromPoint;

        /*setupFirstLink*/
        GameObject firstLink = GameObject.Instantiate(_linkPrefab, lastPoint.transform.position+lastPoint.transform.up.normalized* (2 * _linkSize), Quaternion.identity);

        SetupAsFirstSegment(firstLink, lastPoint);

        lastPoint = firstLink.GetComponent<Rigidbody>();

        _linkSegments.Add(firstLink);

        /*set up middle links*/
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

        /*set up connected link point*/
        if (_toPoint)
        {
            _toPoint.connectedBody = lastPoint;
            _toPoint.GetComponent<LinkSegment>().SetUpVisuals(lastPoint.transform);
        }
       
    }

    /// <summary>
    /// set up the required link and set up connections
    /// </summary>
    /// <param name="linkGO">game object of the link to be setup</param>
    /// <param name="connectedBody">previous rigidbody in the link cable</param>
    private void SetupSegment(GameObject linkGO, Rigidbody connectedBody)
    {
        linkGO.transform.localScale = new Vector3(linkGO.transform.localScale.x, _linkSize, linkGO.transform.localScale.z);

        ConfigurableJoint joint = linkGO.GetComponent<ConfigurableJoint>();

        if (connectedBody)
        {
            joint.connectedBody = connectedBody;


        }
    }

    /// <summary>
    /// set up the FIRST link in the cable and set up its connections
    /// </summary>
    /// <param name="linkGO">game object of the link to be setup</param>
    /// <param name="connectedBody">previous rigidbody in the link cable</param>
    private void SetupAsFirstSegment(GameObject linkGO, Rigidbody connectedBody)
    {
        linkGO.transform.localScale = new Vector3(linkGO.transform.localScale.x, _linkSize, linkGO.transform.localScale.z);

        ConfigurableJoint joint = linkGO.GetComponent<ConfigurableJoint>();

        if (connectedBody)
        {
            joint.connectedBody = connectedBody;
            joint.connectedAnchor = Vector3.zero;
            
        }
    }


}
