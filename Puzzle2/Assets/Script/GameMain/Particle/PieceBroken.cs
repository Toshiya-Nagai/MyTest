using UnityEngine;
using System.Collections;

public class PieceBroken : BaseParticleAction<PieceBroken.Info> {
	public class Info : BaseInfo{
		public State state;
		public Vector3 position;
		public float lifeTime;

		public void Init(){
			position = new Vector2(999.0f,999.0f);
			lifeTime = 0.0f;
			state = State.None;
		}

		public void Gravity(){
			lifeTime += Time.deltaTime;
		}
	}

	public float Gravity = 9.8f;
	public float LifeTime;
	public enum State{
		None,
		Setup,
		Gravity,
	}

	void Start(){
		Emit();
	}

	public int[] Emit(Vector3 position,Color color){
		int[] indexs = new int[8];
		int setCount = 0;
		for(int i = 0;i < infos.Length;i++){
			if(infos[i].state == State.None){
				infos[i].state = State.Setup;
				infos[i].position = position;
//				infos[i].position = new Vector3(position.x+((((i-2)%5)-2)*10),position.y+((((i-2)/5)-2)*10));
				infos[i].color = color;
				indexs[setCount] = i;
				setCount++;
				if(setCount >= indexs.Length){break;}
			}
		}
		return indexs;
	}
	
	protected override void SetEmitParticle (ref ParticleSystem.Particle particle, ref Info info){
		particle.lifetime = 9999f;
		particle.position = new Vector3(9999.0f,9999.0f,9999.0f);
		info.lifeTime = 0.0f;
		info.color = Color.clear;
	}
	protected override void ProcessUpdate (ref ParticleSystem.Particle particle, ref Info info){
		if(info.state == State.Setup){
			particle.position = info.position;
			particle.velocity = new Vector3(Random.Range(-4.0f,4.0f),Random.Range(-4.0f,4.0f),0.0f);
			info.state = State.Gravity;
		}else if(info.state == State.Gravity){
			particle.color = info.color;
			particle.velocity += Vector3.up * -Gravity * Time.deltaTime;
			info.lifeTime += Time.deltaTime;
			if(info.lifeTime > LifeTime){
				info.Init();
				particle.position = info.position;
				particle.velocity = Vector3.zero;
			}
		}
		if(particle.lifetime < 1.0f){particle.lifetime = 9999.0f;}

	}
}
