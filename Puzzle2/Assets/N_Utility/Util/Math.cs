using UnityEngine;
using System.Collections;

namespace N_Math{
	static public class Math2D{
		static public float GetSpeedXY(Vector2 p1,Vector2 p2,float time){
			var distance = Vector2.Distance(p1,p2);
			if(time == 0.0f){time = 1.0f;}
			return distance / time;
		}

		static public float GetSpeedYZ(Vector3 p1,Vector3 p2,float time){
			var distance = Vector3.Distance(p1,p2);
			if(time == 0.0f){time = 1.0f;}
			return distance / time;
		}

		static public float GetDegXY(Vector2 p1, Vector2 p2) {
			float dx = p2.x - p1.x;
			float dy = p2.y - p1.y;
			float rad = Mathf.Atan2(dy, dx);
			return rad * Mathf.Rad2Deg;
		}
		
		static public float GetDegYZ(Vector3 p1,Vector3 p2){
			float dy = p2.y - p1.y;
			float dz = p2.z - p1.z;
			float rad = Mathf.Atan2(dy, dz);
			return rad * Mathf.Rad2Deg;
		}
		
		static public Vector2 GetVelocityXY(float direction, float speed){
			Vector2 v = new Vector2();
			v.x = Mathf.Cos (Mathf.Deg2Rad * direction) * speed;
			v.y = Mathf.Sin (Mathf.Deg2Rad * direction) * speed;
			return v;
		}

		static public Vector2 GetVelocity(Vector2 dist,float time){
			return dist / time;
		}
		
		static public Vector3 GetVelocityZY(Vector3 xy,float directionYZ,float speed){
			xy.z = Mathf.Cos(Mathf.Deg2Rad * directionYZ)*speed;
			return xy;
		}

		static public Vector2 GetDistance(Vector2 p1,Vector2 p2){
			return p1 - p2;
		}
	}

	static public class Math3D{
		static public Vector3 GetDistance(Vector3 p1,Vector3 p2){
			return p1 - p2;
		}
		static public Vector3 GetVelocity(Vector3 target,Vector3 obj,float time){
			var dist = GetDistance(target,obj);
			return dist / time;
		}
	}
}