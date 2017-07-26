using UnityEngine;
using System.Collections;

abstract public class BaseParticleAction<TInfo> : MonoBehaviour where TInfo : BaseParticleAction<TInfo>.BaseInfo ,new() {
	public ParticleSystem MainParticle;
	public int EmitterCount;
	protected ParticleSystem.Particle[] particles;
	protected TInfo[] infos;

	public class BaseInfo{
		public Color color;
		public float size;
		public BaseInfo(){}
	}

	protected virtual void Awake(){
		particles = new ParticleSystem.Particle[MainParticle.maxParticles];
		infos = new TInfo[MainParticle.maxParticles];
		for(int i = 0;i < infos.Length;i++){
			if(infos[i] == null){infos[i] = new TInfo();}
		}
	}

	protected void Emit(){
		MainParticle.Emit(EmitterCount);
		int currentCount = MainParticle.GetParticles(particles);
		for(int i = 0;i < currentCount;i++){
			SetEmitParticle(ref particles[i],ref infos[i]);
		}
		MainParticle.SetParticles(particles,currentCount);
	}

	void Update(){
		int currentCount = MainParticle.GetParticles(particles);
		if(currentCount <= 0){return;}
		for(int i = 0;i < currentCount;i++){
			ProcessUpdate(ref particles[i],ref infos[i]);
		}
		MainParticle.SetParticles(particles,currentCount);
	}

	abstract protected void SetEmitParticle(ref ParticleSystem.Particle particle,ref TInfo info);
	abstract protected void ProcessUpdate(ref ParticleSystem.Particle particle,ref TInfo info);
}
