using UnityEngine;
using System.Collections;

public class PieceBlink : BaseParticleAction<PieceBlink.Info> {
	public class Info : BaseInfo{
		public Vector3 position;
		public float blinkTimer;
		public float timer;
		public State state;
		public bool isStop;
		public void Init(){
			timer = 0.0f;
			blinkTimer = 0.0f;
			position = new Vector2(999.0f,999.0f);
			state = State.Wait;
			isStop = false;
		}
		public void UpdateBlink(){
			blinkTimer += 0.1f;
			color = ((int)blinkTimer % 2 == 0)?Color.white:Color.clear;
//			color = Color.white;
			timer += 1.0f / 60.0f;
		}
	}

	public enum State{
		Wait,
		Blink,
	}

	public void Start(){
		Emit();
	}

	public int Emit(Vector3 position){
		int index = 0;
		for(int i = 0;i < infos.Length;i++){
			if(infos[i].state == State.Wait){
				infos[i].state = State.Blink;
				infos[i].position = position;
				infos[i].isStop = false;
				index = i;
				break;
			}
		}
		return index;
	}

	public void DisableEmit(int index){
		infos[index].isStop = true;
	}

	protected override void SetEmitParticle (ref ParticleSystem.Particle particle, ref Info info){
		particle.lifetime = 9999f;
		particle.position = new Vector3(9999.0f,9999.0f,9999.0f);
		info.blinkTimer = 0.0f;
		info.color = Color.clear;
	}
	protected override void ProcessUpdate (ref ParticleSystem.Particle particle, ref Info info){
		if(info.state == State.Blink){
			info.UpdateBlink();
			particle.startColor = info.color;
			particle.position = info.position + Vector3.back;
			if(info.timer > 2.0f || info.isStop){
				info.Init();
				particle.position = info.position;
			}
		}
		if(particle.lifetime < 1.0f){particle.lifetime = 9999.0f;}
	}
}
