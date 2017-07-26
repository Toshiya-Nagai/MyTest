using UnityEngine;

public static class TransformExtension {

	public static void ResetPosition (this Transform transform) {
		transform.position = Vector3.zero;
	}

	public static void ResetLocalPosition (this Transform transform) {
		transform.localPosition = Vector3.zero;
	}


	public static void ResetRotation (this Transform transform) {
		transform.rotation = Quaternion.identity;
	}

	public static void ResetLocalRotation (this Transform transform) {
		transform.localRotation = Quaternion.identity;
	}

	public static void ResetScale (this Transform transform) {
		transform.localScale = Vector3.one;
	}


	// common
	public static void Reset (this Transform transform) {
		if (transform.parent == null) {
			transform.ResetPosition ();
			transform.ResetRotation ();
		} else {
			transform.ResetLocalPosition ();
			transform.ResetLocalRotation ();
		}
		transform.ResetScale ();
	}


	// parent
	public static Transform SearchParent (this Transform transform) {
		Transform parent = parentObject (transform);
		return parent;
	}

	static Transform parentObject (Transform obj) {
		if (obj.parent == null) {
			return obj;
		}
		return parentObject (obj.parent);
	}

	public static void SetChild (this Transform transform, Transform child) {
		Vector3	localPosition = child.localPosition;
		Vector3	localScale = child.localScale;
		Quaternion localRotation = child.localRotation;
		
		child.parent = transform;
		
		child.localScale = localScale;
		child.localPosition = localPosition;
		child.localRotation = localRotation;
	}

	public static void SetChildResetTransform (this Transform transform, Transform child) {
		child.parent = transform;
		child.Reset ();
	}

	public static void SetParentResetTransform (this Transform transform, GameObject parentObject) {
		parentObject.transform.SetChildResetTransform (transform);
	}
}