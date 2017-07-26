using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillParticle : BaseParticleAction<SkillParticle.Info> {
	public List<Material> materials;
	public class Info : BaseInfo{
		public float timer;
		public Vector3 position;
		public State state;
		public void Init(){
			position = new Vector3(999.0f,999.0f,999.0f);
			timer = 0.0f;
			state = State.Wait;
		}
	}

	public enum State{
		Wait,
		Play,
	}

	void Start(){
		Emit ();
	}

	public int Emit(Vector3 position){
		int index = 0;
		for(int i = 0;i < infos.Length;i++){
			if(infos[i].state == State.Wait){
				infos[i].state = State.Play;
				infos[i].position = position;
				index = i;
				break;
			}
		}
		return index;
	}

	public List<int> EmitMany(int skillId,List<GameObject> targets){
		this.MainParticle.GetComponent<Renderer>().material = materials[skillId];
		List<int> indexs = new List<int>();
		for(int i = 0;i < targets.Count;i++){
			indexs.Add(Emit(targets[i].transform.position));
		}
		return indexs;
	}

	#region implemented abstract members of BaseParticleAction
	protected override void SetEmitParticle (ref ParticleSystem.Particle particle, ref Info info){
		particle.lifetime = 9999f;
		particle.position = new Vector3(9999.0f,9999.0f,9999.0f);
		info.timer = 0.0f;
		info.color = Color.white;
	}

	protected override void ProcessUpdate (ref ParticleSystem.Particle particle, ref Info info){
		if(info.state == State.Play){
			particle.color = info.color;
			particle.position = info.position + Vector3.back;
			info.timer += Time.deltaTime;
			if(info.timer > 2.0f){
				info.Init();
				particle.position = info.position;
			}
		}
		if(particle.lifetime < 1.0f){particle.lifetime = 9999.0f;}
	}
	#endregion
}
