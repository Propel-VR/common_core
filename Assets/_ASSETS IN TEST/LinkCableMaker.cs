using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkCableMaker : MonoBehaviour
{

    [SerializeField]
    Rigidbody _fromPoint;
    [SerializeField]
    ConfigurableJoint _toPoint;

    [SerializeField]
    float _linkSize;

    [SerializeField]
    int _numLinks;

    [SerializeField]
    GameObject _linkPrefab;

    List<GameObject> _linkSegments=new List<GameObject>();


    private void Start()
    {
        StartCoroutine(GenerateCable());
    }

    public IEnumerator GenerateCable()
    {
        Rigidbody lastPoint=_fromPoint;

        //setupFirstLink
        GameObject firstLink = GameObject.Instantiate(_linkPrefab, Vector3.zero, Quaternion.identity);
        //GameObject firstLink = GameObject.Instantiate(_linkPrefab, lastPoint.transform.position+lastPoint.transform.up.normalized*_linkSize, Quaternion.identity);

        //firstLink.GetComponent<LinkSegment>().SetupAsFirstSegment(_linkSize, lastPoint);

        lastPoint = firstLink.GetComponent<Rigidbody>();

        _linkSegments.Add(firstLink);

        //yield return new WaitForSeconds(0.15f);

        for (int i=1; i<_numLinks; i++)
        {

            //instantiate a link
            GameObject go =GameObject.Instantiate(_linkPrefab, Vector3.zero, Quaternion.identity);
            //GameObject go =GameObject.Instantiate(_linkPrefab, lastPoint.transform.position+lastPoint.transform.up.normalized * _linkSize, Quaternion.identity);

            //go.GetComponent<LinkSegment>().SetupSegment(_linkSize, lastPoint);

            lastPoint=go.GetComponent<Rigidbody>();

            _linkSegments.Add(go);

            //yield return new WaitForSeconds(0.15f);

        }

        if (_toPoint)
            _toPoint.connectedBody = lastPoint;

        yield return new WaitForSeconds(0.5f);
        /*
        foreach(GameObject lsGO in _linkSegments)
        {
            ConfigurableJoint cnfjnt=lsGO.GetComponent<ConfigurableJoint>();

            cnfjnt.massScale= 1.0f;
            cnfjnt.connectedMassScale= 1.0f;

        }
        */
    }
}
