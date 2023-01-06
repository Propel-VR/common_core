using MassiveProceduralTube;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCable : MonoBehaviour
{

    [SerializeField]
    private GameObject _cablePrefab;
    

    private List<ProceduralTube> _tubes = new List<ProceduralTube>();

    [SerializeField]
    private Transform _tubeFrom, _tubeTo;
    [SerializeField]
    private Transform _fromCurve, _toCurve;

    [SerializeField]
    private int _numCables = 1;


    public void SetSettings(Transform from, Transform fromC, Transform to, Transform toC)
    {
        _tubeFrom= from;
        _tubeTo= to;
        _fromCurve= fromC;
        _toCurve= toC;
    }


    private void Generate(int n)
    {
        for (int i = 0; i < n; i++)
        {
            GameObject a = Instantiate(_cablePrefab);
            a.transform.parent = this.transform;
            _tubes.Add(a.GetComponent<ProceduralTube>());
            Vector3 pointStart = _tubeFrom.position;
            Vector3 pointEnd = _tubeTo.position;
            int id = _tubes.Count - 1;
            _tubes[id].transform.hasChanged = false;
            _tubes[id].handle.p0 = _tubes[id].transform.InverseTransformPoint(pointStart);
            _tubes[id].handle.p3 = _tubes[id].transform.InverseTransformPoint(pointEnd);
            //_tubes[id].handle.p1 = _tubes[id].transform.InverseTransformPoint(pointStart) + _tubeFrom.forward * 0.05f;
            //_tubes[id].handle.p2 = _tubes[id].transform.InverseTransformPoint(pointEnd) + _tubeTo.forward * 0.05f;
            _tubes[id].handle.p1 = _tubes[id].transform.InverseTransformPoint(_fromCurve.position);
            _tubes[id].handle.p2 = _tubes[id].transform.InverseTransformPoint(_toCurve.position);
            _tubes[id].handle.radiusStart = 0.015f;
            _tubes[id].handle.radiusEnd = 0.015f;
            _tubes[id].handle.taperType = 1;
            ProceduralTube_ConstraintHandles constraint = a.GetComponent<ProceduralTube_ConstraintHandles>();
            constraint.EndPoint = _tubeTo.transform;
            constraint.StartPoint= _tubeFrom.transform;

        }
    }

    private void Update()
    {
        foreach(ProceduralTube tube in _tubes) 
        {

            tube.handle.p0 = tube.transform.InverseTransformPoint(_tubeFrom.position);
            tube.handle.p3 = tube.transform.InverseTransformPoint(_tubeTo.position);
            //tube.handle.p1 = tube.transform.InverseTransformPoint(_tubeFrom.position) + _tubeFrom.forward * 0.2f;
            //tube.handle.p2 = tube.transform.InverseTransformPoint(_tubeTo.position) + _tubeTo.forward * 0.2f;
            tube.handle.p1 = tube.transform.InverseTransformPoint(_fromCurve.position);
            tube.handle.p2 = tube.transform.InverseTransformPoint(_toCurve.position);
        }
    }

    private void Remove(int n)
    {
        for (int i = 0; i < n; i++)
        {
            int id = _tubes.Count - 1;
            //print(id);
            if (id >= 0)
            {
                Destroy(_tubes[_tubes.Count - 1].transform.gameObject);
                _tubes.RemoveAt(_tubes.Count - 1);
            }
        }
    }

    private void Start()
    {
        Generate(_numCables);
    }

}
