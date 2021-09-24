using UnityEngine;
using System.Collections;

public class AnimEventEarthElemental : MonoBehaviour {

//	public GameObject[] DieMesh;
	public GameObject EartElemental;
	public Transform Istancer;
	
	SingleDemoEarthElemental Demo;
	
	void Start(){
		Demo = GetComponentInParent<SingleDemoEarthElemental>();
	}
	
	public void DieEvent(){
		if(Demo.curmat == 0){
			GameObject G = (GameObject)Instantiate(Demo.DieAnim[0], Istancer.position, Istancer.rotation);
			Demo.instanceDieMesh = G;
			Demo.instanceDieMesh.transform.parent = Demo.T;
			G.SetActive(true);
		}else{
			GameObject G = (GameObject)Instantiate(Demo.DieAnim[1], Istancer.position, Istancer.rotation);
			Demo.instanceDieMesh = G;
			G.SetActive(true);
		}
		EartElemental.SetActive(false);
	}

}
