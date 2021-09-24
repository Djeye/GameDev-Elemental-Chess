using UnityEngine;
using System.Collections;

public class SingleDemoEarthElemental : MonoBehaviour {

	public Transform T;
	public float speed = 10.0f;
	
	public int curmat;
	
	bool spin;
	Camera cam;
	
	public Animator anim;
	public float size = 1;
	public Material[] Mat;
	public Renderer mesh;
	public GameObject[] DieAnim;
	public GameObject Model;
	public GameObject instanceDieMesh;

	
	void Start () {
		cam = Camera.main;
		spin = false;
		transform.localScale = new Vector3(size,size,size);
		anim = GetComponentInChildren<Animator>();
	}
	
	public void Spin(){
		spin = !spin;
		T.eulerAngles = Vector3.zero;
	}
	public void ChangeMat(){
		if(curmat == 0){
			curmat = 1;

		}else{
			curmat = 0;
		}
		mesh.material = Mat[curmat];
	}
	
	/// animation
	public void Idle(){
		CheckStatus();
		anim.Play("idle");
	}
	
	public void IdleActivate(){
		
		CheckStatus();
		anim.Play("idleActivate");
	}
	
	public void Walk(){
		CheckStatus();
		anim.Play("walk");
	}
	public void Run(){
		CheckStatus();
		anim.Play("run");
	}
	public void Hit(){
		CheckStatus();
		anim.Play("hit");
	}
	public void Activate(){
		CheckStatus();
		anim.Play("activate");
	}
	
	public void Atk01(){
		CheckStatus();
		anim.Play("attack01");
	}
	public void Atk02(){
		CheckStatus();
		anim.Play("attack02");
	}
	
	public void die(){
		CheckStatus();
		anim.Play("die");
	}
	
	void Update(){
		if(Input.GetMouseButton(0)){
			T.eulerAngles = new Vector3(T.eulerAngles.x,T.eulerAngles.y + Input.GetAxis("Mouse X") * -2,T.eulerAngles.z);
		}
		
		if (spin == false){
			return;
		}
		T.eulerAngles = new Vector3(T.eulerAngles.x,T.eulerAngles.y + speed * Time.deltaTime,T.eulerAngles.z);
	}
	
	void CheckStatus(){
		
		if(Model.activeSelf == false){
			Model.SetActive(true);
			DestroyIstanceDieMesh();
		}
	}

	void DestroyIstanceDieMesh(){
		if(instanceDieMesh != null){
			Destroy(instanceDieMesh);
		}
	}

}
