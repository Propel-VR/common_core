using UnityEngine;
using System;
using System.Collections.Generic;


public class ParticleSpawner:MonoBehaviour{
    	//Used to sort particle system list
    //Visible properties
    public GameObject[] particles;			//Particle systems to add a button for each
    public int maxButtons = 10;			//Maximum buttons per page	
    public bool showInfo;
    public string removeTextFromButton;
    
    //Hidden properties
    int page = 0;			//Current page
    int pages;				//Number of pages
    string currentPSInfo;	//Current particle info
    GameObject currentPS;
    
    public void Start(){
    	//Calculate number of pages
    	pages = (int)Mathf.Ceil((float)((particles.Length -1 )/ maxButtons));
    }
    
    public void OnGUI() {
    	//Time Scale Vertical Slider
    	Time.timeScale = GUI.VerticalSlider (new Rect (185.0f, 50.0f, 20.0f, 150.0f), Time.timeScale, 2.0f, 0.0f);
    	//Field of view Vertical Slider
    		//Camera.mainCamera.fieldOfView = GUI.VerticalSlider (Rect (225, 50, 20, 150), Camera.mainCamera.fieldOfView, 20.0, 100.0);
    	//Check if there are more particle systems than max buttons (true adds "next" and "prev" buttons)
    	if(particles.Length > maxButtons){
    		//Prev button
    		if(GUI.Button(new Rect(20.0f,(float)((maxButtons+1)*18),75.0f,18.0f),"Prev"))if(page > 0)page--;else page=pages;
    		//Next button
    		if(GUI.Button(new Rect(95.0f,(float)((maxButtons+1)*18),75.0f,18.0f),"Next"))if(page < pages)page++;else page=0;
    		//Page text
    		GUI.Label (new Rect(60.0f,(float)((maxButtons+2)*18),150.0f,22.0f), "Page" + (page+1) + " / " + (pages+1));
    		
    	}
    	//Toggle button for info
    	showInfo = GUI.Toggle (new Rect(185.0f, 20.0f,75.0f,25.0f), showInfo, "Info");
    	
    	//System info
    	if(showInfo)GUI.Label (new Rect(250.0f, 20.0f,500.0f,500.0f), currentPSInfo);
    	
    	//Calculate how many buttons on current page (last page might have less)
    	int pageButtonCount = particles.Length - (page*maxButtons);
    	//Debug.Log(pageButtonCount);
    	if(pageButtonCount > maxButtons)pageButtonCount = maxButtons;
    	
    	//Adds buttons based on how many particle systems on page
    	for(int i=0;i < pageButtonCount;i++){
    		string buttonText = particles[i+(page*maxButtons)].transform.name;
    		buttonText = buttonText.Replace(removeTextFromButton, "");
    		if(GUI.Button(new Rect(20.0f,(float)(i*18+18),150.0f,18.0f),buttonText)){
    			if(currentPS != null) Destroy(currentPS);
    			GameObject go = (GameObject)Instantiate(particles[i+page*maxButtons]);
    			currentPS = go;
    			PlayPS(go.GetComponent<ParticleSystem>(), i + (page * maxButtons) +1);
    			InfoPS(go.GetComponent<ParticleSystem>(), i + (page * maxButtons) +1);
    		}
    	}
    }
    //Play particle system (resets time scale)
    public void PlayPS(ParticleSystem _ps,int _nr){
    		Time.timeScale = 1.0f;
    		_ps.Play();
    }
    
    public void InfoPS(ParticleSystem _ps,int _nr){
    		//Change current particle info text
    		currentPSInfo = "System" + ": " + _nr + "/" + particles.Length +"\n"+
    		"Name: " + _ps.gameObject.name +"\n\n" +
    		"Main PS Sub Particles: " + _ps.transform.childCount  +"\n" +
    		"Main PS Materials: " + _ps.GetComponent<Renderer>().materials.Length +"\n" +
    		"Main PS Shader: " + _ps.GetComponent<Renderer>().material.shader.name;
    		//If plasma(two materials)
    		if(_ps.GetComponent<Renderer>().materials.Length >= 2)currentPSInfo = currentPSInfo + "\n\n *Plasma not mobile optimized*";
    		//Usage Info
    		currentPSInfo = currentPSInfo + "\n\n Use mouse wheel to zoom, click and hold to rotate";
    		currentPSInfo = currentPSInfo.Replace("(Clone)", "");
    }
}