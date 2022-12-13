using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MassiveProceduralTube;
using UnityEngine.UI;

public class TestCables :MonoBehaviour {

    public GameObject prefab;
    public List<ProceduralTube> tubes = new List<ProceduralTube>();


    public Text text_millis;
    public Text text_num;

    public Transform cube;
    public Transform sphere;

    private int num = 100;

    private void Generate(int n) {
        for(int i = 0; i < n; i++) {
            GameObject a = Instantiate(prefab);
            a.transform.parent = this.transform;
            tubes.Add(a.GetComponent<ProceduralTube>());
            Vector3 pointStart = cube.position + new Vector3(0, Random.Range(-5f, 5f), Random.Range(-5f, 5f));
            Vector3 pointEnd = sphere.position;
            int id = tubes.Count - 1;
            tubes[id].transform.hasChanged = false;
            tubes[id].handle.p0 = tubes[id].transform.InverseTransformPoint(pointStart);
            tubes[id].handle.p3 = tubes[id].transform.InverseTransformPoint(pointEnd);
            tubes[id].handle.p1 = tubes[id].transform.InverseTransformPoint(pointStart) + cube.right * 1f;
            tubes[id].handle.p2 = tubes[id].transform.InverseTransformPoint(pointEnd) + sphere.right * -1f;
            tubes[id].handle.radiusStart = 0.05f;
            tubes[id].handle.radiusEnd = 0.01f;
            tubes[id].handle.taperType = 1;
            ProceduralTube_ConstraintHandles constraint = a.GetComponent<ProceduralTube_ConstraintHandles>();
            constraint.EndPoint = sphere.transform;
        }
    }

    private void Remove(int n) {
        for(int i = 0; i < n; i++) {
            int id = tubes.Count - 1;
            //print(id);
            if(id >= 0) {
                Destroy(tubes[tubes.Count - 1].transform.gameObject);
                tubes.RemoveAt(tubes.Count - 1);
            }
        }
    }

    private void Start() {
        Generate(num);
    }

    private void Update() {
        text_millis.text = Mathf.Round(Time.unscaledDeltaTime * 1000).ToString() + " ms";
    }

    public void IncreaseButton() {
        switch(num) {
            case 10:
                num = 100;
                Generate(90);
                break;
            case 5:
                num = 10;
                Generate(5);
                break;
            case 3:
                num = 5;
                Generate(2);
                break;
            case 1:
                num = 3;
                Generate(2);
                break;
            default:
                num += 100;
                Generate(100);
                break;
        }
        text_num.text = num.ToString();

    }
    public void DecreaseButton() {
        switch(num) {
            case 100:
                num = 10;
                Remove(90);
                break;
            case 10:
                num = 5;
                Remove(5);
                break;
            case 5:
                num = 3;
                Remove(2);
                break;
            case 3:
                num = 1;
                Remove(2);
                break;
            case 1:
                num = 1;
                break;
            default:
                num -= 100;
                Remove(100);
                break;
        }
        text_num.text = num.ToString();
    }

    public void SliderX(float s) {
        float x = s * 8.5f - 3.5f;  // -3.5 ~ 5
        Vector3 p = sphere.position;
        sphere.position = new Vector3(x, p.y, p.z);
    }
    public void SliderY(float s) {
        float y = s * -6f + 3f;  // -3~3
        Vector3 p = sphere.position;
        sphere.position = new Vector3(p.x, y, p.z);
    }

    public ProceduralTubeManager manager;
    public void SliderInten(float s) {
        float i = s * 5f;  // 0~5
        manager.intensity_spring = i;
    }
    public void SliderDrag(float s) {
        float i = s * 0.18f + 0.02f;  // 0.02~0.2
        manager.drag_spring = i;
    }
    public void SliderGravity(float s) {
        float i = s * 15f;  // 0~15
        manager.gravity = i;
    }

    public void ToggleSpring(bool b) {
        manager.isSpringMode = b;
    }


}
