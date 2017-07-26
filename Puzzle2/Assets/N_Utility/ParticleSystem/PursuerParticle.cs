using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PursuerParticle : BaseParticleAction<PursuerParticle.PursuerInfo> {
	public Transform Target;
	
	public float StartDelay = 4.0f;
	public float Gravity = 19.6f;
	public float CollectDistance = 1f;
	float collectSqrDistance;
	
	public UnityEvent<ParticleSystem.Particle> HitCallback;

	public enum State{
		None,
		Pursuer,
		Getting,
	}
	public class PursuerInfo : BaseInfo{
		public State state;
	}

	protected override void Awake (){
		base.Awake ();
		collectSqrDistance = CollectDistance * CollectDistance;
	}

	void OnEnable(){
		MainParticle.Stop();
		Emit();
	}

	#region implemented abstract members of BaseParticleAction

	protected override void SetEmitParticle (ref ParticleSystem.Particle particle,ref PursuerInfo info){
		info.state = State.None;
	}

	protected override void ProcessUpdate (ref ParticleSystem.Particle particle, ref PursuerInfo info){
		bool collected = ((Vector2)particle.position - (Vector2)Target.position).sqrMagnitude < collectSqrDistance;
		if(collected && (info.state == State.Pursuer || info.state == State.None)) {
			//get coin do something
			particle.position = new Vector3(-100, -100, -100);
			info.state = State.Getting;
			//add event program -> ParticleHitCallback.Add(UnityAction);
			if(HitCallback != null){HitCallback.Invoke(particle);}
			//				HitCallback.Invoke();
		}else if(info.state == State.Pursuer){	// coin moveing
			particle.position += getVelocity(Target.position+(Vector3.up/2),particle.position,30);
		}else if (info.state == State.None){	// gravity moving
			particle.velocity += Vector3.up*-Gravity*Time.deltaTime;
			//state change  After a certain period of time has elapsed, the tracking start
			if(particle.lifetime > 0 && particle.lifetime < MainParticle.startLifetime - StartDelay){
				info.state = State.Pursuer;
			}
		}
	}

	#endregion

	void Update(){
		int currentCount = MainParticle.GetParticles(particles);
		if(currentCount <= 0){return;}
		for(int i = 0;i < currentCount;i++){
			ProcessUpdate(ref particles[i],ref infos[i]);
		}
		MainParticle.SetParticles(particles,currentCount);
		if(Input.GetKeyDown (KeyCode.A)){
			OnEnable();
		}
	}

	
	Vector3 getDistance(Vector3 p1,Vector3 p2){
		return p1 - p2;
	}
	
	Vector3 getVelocity(Vector3 target,Vector3 obj,float time){
		if(time < 1){return Vector3.zero;}
		var dist = getDistance(target,obj);
		return dist / time;
	}

}
