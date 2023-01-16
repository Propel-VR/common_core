using MassiveProceduralTube;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to generate a simple cable using proceduralk cables
/// </summary>
public class SimpleCable : MonoBehaviour
{

    #region private serialized fields
    [SerializeField]
    private GameObject _cablePrefab;
    
    [SerializeField]
    private Transform _tubeFrom, _tubeTo;
    [SerializeField]
    private Transform _fromCurve, _toCurve;

    [SerializeField]
    private int _numCables = 1;
    #endregion

    #region fields
    private List<ProceduralTube> _tubes = new List<ProceduralTube>();
    #endregion

    /// <summary>
    /// Called to set the specifications of the wire in code
    /// </summary>
    /// <param name="from">transform to start the wire from</param>
    /// <param name="fromC">second curve point</param>
    /// <param name="to">tramnsform to build the wire to</param>
    /// <param name="toC">third curve point</param>
    public void SetSettings(Transform from, Transform fromC, Transform to, Transform toC)
    {
        _tubeFrom= from;
        _tubeTo= to;
        _fromCurve= fromC;
        _toCurve= toC;
    }

    /// <summary>
    /// Generates the cable to render
    /// </summary>
    /// <param name="n">number of cables to generate</param>
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
            //update each cables position
            tube.handle.p0 = tube.transform.InverseTransformPoint(_tubeFrom.position);
            tube.handle.p3 = tube.transform.InverseTransformPoint(_tubeTo.position);
            tube.handle.p1 = tube.transform.InverseTransformPoint(_fromCurve.position);
            tube.handle.p2 = tube.transform.InverseTransformPoint(_toCurve.position);
        }
    }

   
    private void Start()
    {
        Generate(_numCables);
    }

}
