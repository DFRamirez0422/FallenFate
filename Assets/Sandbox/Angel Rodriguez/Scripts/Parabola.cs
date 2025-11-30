using UnityEngine;

public class Parabola : MonoBehaviour
{
public float speed;

public GameObject targetObject;
public Vector3 target;
public float arcHeight;

Vector3 _startPosition;
float _stepScale;
float _progress;

void Start() {
    _startPosition = transform.position;

    float distance = Vector3.Distance(_startPosition, target);
      target = targetObject.transform.position;
    // This is one divided by the total flight duration, to help convert it to 0-1 progress.
    _stepScale = speed / distance;
}

void Update() {
    // Increment our progress from 0 at the start, to 1 when we arrive.
    _progress = Mathf.Min(_progress + Time.deltaTime * _stepScale, 1.0f);

    // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.
    float parabola = 1.0f - 4.0f * (_progress - 0.5f) * (_progress - 0.5f);

    // Travel in a straight line from our start position to the target.        
    Vector3 nextPos = Vector3.Lerp(_startPosition, target, _progress);

    // Then add a vertical arc in excess of this.
    arcHeight = arcHeight * Vector3.Distance(_startPosition, target);
    nextPos.y += parabola * arcHeight;

    // Continue as before.
    transform.LookAt(nextPos, transform.forward);
    transform.position = nextPos;
}

}
