using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MassiveProceduralTube;

public class DemoControl : MonoBehaviour {

    #region Properties
    public enum Demo {
        Material,
        TubeType,
        Dynamic,
        Performance,
        Performance2
    }
    public Demo demo = Demo.Material;

    [Space(5)]
    public ProceduralTubeManager tubeManager;
    public ProceduralTubeRenderer tubeRenderer;
    public Text label;
    private ProceduralTube[] tubes;
    #endregion

    #region Monobehaviour
    private void Awake() {
        if(tubeManager == null)
            tubeManager = FindObjectOfType<ProceduralTubeManager>();
        if(tubeRenderer == null)
            tubeRenderer = FindObjectOfType<ProceduralTubeRenderer>();
        if(label == null)
            label = GameObject.Find("Label_Text").GetComponent<Text>();
        tubes = FindObjectsOfType<ProceduralTube>();
    }

    private void Start() {
        switch(demo) {
            case Demo.Material:
                break;
            case Demo.TubeType:
                break;
            case Demo.Dynamic:
                DemoDynamicStart();
                break;
            case Demo.Performance:
                DemoPerformanceStart();
                break;
            case Demo.Performance2:
                DemoPerformance2Start();
                break;
        }
    }

    private void Update() {
        switch(demo) {
            case Demo.Material:
                DemoMaterial();
                break;
            case Demo.TubeType:
                DemoTaperType();
                break;
            case Demo.Dynamic:
                DemoDynamic();
                break;
            case Demo.Performance:
                DemoPerformance();
                break;
            case Demo.Performance2:
                DemoPerformance2();
                break;
        }
    }
    #endregion

    #region Demo_1_Material
    [Space(15), Header("Property-Demo1")]
    public Material[] mats;
    private void DemoMaterial() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            tubeRenderer.instanceMaterial = mats[0];
            label.text = mats[0].name;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            tubeRenderer.instanceMaterial = mats[1];
            label.text = mats[1].name;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            tubeRenderer.instanceMaterial = mats[2];
            label.text = mats[2].name;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)) {
            tubeRenderer.instanceMaterial = mats[3];
            label.text = mats[3].name;
        }
    }
    #endregion

    #region Demo_2_TaperType
    [Space(15), Header("Property-Demo2")]
    public ProceduralTube.TaperType taperType = ProceduralTube.TaperType.Linear;
    private void DemoTaperType() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            foreach(ProceduralTube tube in tubes) {
                taperType = ProceduralTube.TaperType.Linear;
                tube.taperType = taperType;
                label.text = taperType.ToString();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            foreach(ProceduralTube tube in tubes) {
                taperType = ProceduralTube.TaperType.Exponential;
                tube.taperType = taperType;
                label.text = taperType.ToString();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            foreach(ProceduralTube tube in tubes) {
                taperType = ProceduralTube.TaperType.InverseExponential;
                tube.taperType = taperType;
                label.text = taperType.ToString();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)) {
            foreach(ProceduralTube tube in tubes) {
                taperType = ProceduralTube.TaperType.Smooth;
                tube.taperType = taperType;
                label.text = taperType.ToString();
            }
        }
        if(Input.GetKeyDown(KeyCode.Alpha5)) {
            foreach(ProceduralTube tube in tubes) {
                taperType = ProceduralTube.TaperType.Symmetry;
                tube.taperType = taperType;
                label.text = taperType.ToString();
            }
        }
    }
    #endregion

    #region Demo_3_Dynamic
    [Space(15), Header("Property-Demo3")]
    public Transform sphere1;
    public Transform sphere2;
    private Vector3 sph1Center;
    private Vector3 sph2Center;
    private void DemoDynamicStart() {
        sph1Center = sphere1.position;
        sph2Center = sphere2.position;
        foreach(ProceduralTube tube in tubes) {
            float r = Random.Range(0.05f, 0.12f);
            tube.handle.radiusStart = r;
            tube.handle.radiusEnd = r;
        }
    }
    float sphr = 1f;
    private void DemoDynamic() {
        float time = Time.time*2;
        float x1 = Mathf.Cos(time)      * sphr;
        float z1 = Mathf.Sin(time)      * sphr;
        float y1 = Mathf.Cos(time*0.5f) * sphr;
        sphere1.position = sph1Center + new Vector3(x1, y1, z1);
        float y2 = Mathf.Cos(time+1.571f);
        y2 = Mathf.Clamp(y2*3, -1, 1) * sphr;
        sphere2.position = sph2Center + new Vector3( 0, y2, 0);
    }
    public void ToggleDynamic(bool b) {
        tubeManager.isSpringMode = b;
    }
    public void SliderIntensity(float s) {
        float i = s * 5f;  // 0~5
        tubeManager.intensity_spring = i;
    }
    public void SliderDrag(float s) {
        float i = s * 0.18f + 0.02f;  // 0.02~0.2
        tubeManager.drag_spring = i;
    }
    public void SliderGravity(float s) {
        float i = s * 15f;  // 0~15
        tubeManager.gravity = i;
    }
    #endregion

    #region Demo_4_Performance
    [Space(15), Header("Property-Demo4")]
    public GameObject tubePrefab;
    private int NumTubes = 100;
    public List<ProceduralTube> listTubes = new List<ProceduralTube>();
    public Transform testSphere;
    private Vector3 testSphereCenter;
    public Transform testCube1;
    public Transform testCube2;
    public Text text_millis;
    private void DemoPerformanceStart() {
        testSphereCenter = testSphere.position;
        GenerateTubes(NumTubes);
    }
    private void DemoPerformance() {
        float time = Time.time * 2;
        float x = Mathf.Cos(time) * 1;
        float z = Mathf.Sin(time) * 1;
        testSphere.position = testSphereCenter + new Vector3(x, 0, z);
        text_millis.text = Mathf.Round(Time.unscaledDeltaTime * 1000).ToString() + " ms";
    }
    private void GenerateTubes(int n) {
        for(int i = 0; i < n; i++) {
            GameObject a = Instantiate(tubePrefab);
            a.transform.parent = this.transform;
            listTubes.Add(a.GetComponent<ProceduralTube>());
            Transform cube = (i % 2 == 0) ? cube = testCube1 : cube = testCube2;
            Vector3 pointStart = cube.position + new Vector3(Random.Range(-0.8f, 0.8f), 0, Random.Range(-0.8f, 0.8f));
            Vector3 pointEnd = testSphere.position;
            int id = listTubes.Count - 1;
            listTubes[id].transform.hasChanged = false;
            listTubes[id].handle.p0 = listTubes[id].transform.InverseTransformPoint(pointStart);
            listTubes[id].handle.p3 = listTubes[id].transform.InverseTransformPoint(pointEnd);
            listTubes[id].handle.p1 = listTubes[id].transform.InverseTransformPoint(pointStart) + cube.up * 1f;
            listTubes[id].handle.p2 = listTubes[id].transform.InverseTransformPoint(pointEnd) + testSphere.up*-1f;
            listTubes[id].handle.radiusStart = 0.02f;
            listTubes[id].handle.radiusEnd = 0.005f;
            listTubes[id].handle.taperType = 1;
            ProceduralTube_ConstraintHandles constraint = a.GetComponent<ProceduralTube_ConstraintHandles>();
            constraint.EndPoint = testSphere.transform;
        }
    }
    private void RemoveTubes(int n) {
        for(int i = 0; i < n; i++) {
            int id = listTubes.Count - 1;
            //print(id);
            if(id >= 0) {
                Destroy(listTubes[listTubes.Count - 1].transform.gameObject);
                listTubes.RemoveAt(listTubes.Count - 1);
            }
        }
    }
    public void IncreaseButton() {
        switch(NumTubes) {
            case 10:
                NumTubes = 100;
                GenerateTubes(90);
                break;
            case 5:
                NumTubes = 10;
                GenerateTubes(5);
                break;
            case 3:
                NumTubes = 5;
                GenerateTubes(2);
                break;
            case 1:
                NumTubes = 3;
                GenerateTubes(2);
                break;
            default:
                NumTubes += 100;
                GenerateTubes(100);
                break;
        }
        label.text = NumTubes.ToString();

    }
    public void DecreaseButton() {
        switch(NumTubes) {
            case 100:
                NumTubes = 10;
                RemoveTubes(90);
                break;
            case 10:
                NumTubes = 5;
                RemoveTubes(5);
                break;
            case 5:
                NumTubes = 3;
                RemoveTubes(2);
                break;
            case 3:
                NumTubes = 1;
                RemoveTubes(2);
                break;
            case 1:
                NumTubes = 1;
                break;
            default:
                NumTubes -= 100;
                RemoveTubes(100);
                break;
        }
        label.text = NumTubes.ToString();
    }
    #endregion

    #region Demo_5_Performance2
    [Space(15), Header("Property-Demo5")]
    public GameObject tubePrefab_static;
    public List<ProceduralTube> listStaticTubes = new List<ProceduralTube>();
    private void DemoPerformance2Start() {
        GenerateTubesStatic(NumTubes);
    }
    private void DemoPerformance2() {
        text_millis.text = Mathf.Round(Time.unscaledDeltaTime * 1000).ToString() + " ms";
    }
    private void GenerateTubesStatic(int n) {
        for(int i = 0; i < n; i++) {
            GameObject a = Instantiate(tubePrefab_static);
            a.transform.parent = this.transform;
            listTubes.Add(a.GetComponent<ProceduralTube>());
            Transform cube = (i % 2 == 0) ? cube = testCube1 : cube = testCube2;
            int id = listTubes.Count - 1;
            listTubes[id].transform.hasChanged = false;
            float y = Random.Range(-0.0f, 3.3f);
            float z = Random.Range(-1.8f, 1.8f);
            Vector3 startPoint = new Vector3(-4, y + Random.Range(-0.2f, 0.2f), z + Random.Range(-0.2f, 0.2f));
            Vector3 endPoint   = new Vector3( 4, y + Random.Range(-0.2f, 0.2f), z + Random.Range(-0.2f, 0.2f));
            listTubes[id].handle.p0 = listTubes[id].transform.InverseTransformPoint(startPoint);
            listTubes[id].handle.p3 = listTubes[id].transform.InverseTransformPoint(endPoint);
            listTubes[id].handle.p1 = listTubes[id].transform.InverseTransformPoint(startPoint + new Vector3( 1,-0.5f, 0));
            listTubes[id].handle.p2 = listTubes[id].transform.InverseTransformPoint(  endPoint + new Vector3(-1,-0.5f, 0));
            float r = Random.Range(0.005f, 0.02f);
            listTubes[id].handle.radiusStart = r;
            listTubes[id].handle.radiusEnd = r;
            listTubes[id].handle.taperType = 1;
            listTubes[id].isStatic = true;
        }
    }
    private void RemoveTubesStatic(int n) {
        for(int i = 0; i < n; i++) {
            int id = listTubes.Count - 1;
            //print(id);
            if(id >= 0) {
                Destroy(listTubes[listTubes.Count - 1].transform.gameObject);
                listTubes.RemoveAt(listTubes.Count - 1);
            }
        }
    }
    public void IncreaseStaticButton() {
        switch(NumTubes) {
            case 10:
                NumTubes = 100;
                GenerateTubesStatic(90);
                break;
            case 5:
                NumTubes = 10;
                GenerateTubesStatic(5);
                break;
            case 3:
                NumTubes = 5;
                GenerateTubesStatic(2);
                break;
            case 1:
                NumTubes = 3;
                GenerateTubesStatic(2);
                break;
            default:
                NumTubes += 100;
                GenerateTubesStatic(100);
                break;
        }
        label.text = NumTubes.ToString();

    }
    public void DecreaseStaticButton() {
        switch(NumTubes) {
            case 100:
                NumTubes = 10;
                RemoveTubesStatic(90);
                break;
            case 10:
                NumTubes = 5;
                RemoveTubesStatic(5);
                break;
            case 5:
                NumTubes = 3;
                RemoveTubesStatic(2);
                break;
            case 3:
                NumTubes = 1;
                RemoveTubesStatic(2);
                break;
            case 1:
                NumTubes = 1;
                break;
            default:
                NumTubes -= 100;
                RemoveTubesStatic(100);
                break;
        }
        label.text = NumTubes.ToString();
    }
    #endregion

}
