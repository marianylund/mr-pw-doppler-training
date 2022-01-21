using System;
using System.Collections;
using UnityEngine;

namespace DopplerSim
{
    /// <summary>
    /// Expects to have the cover image that needs moving as a child
    /// Keeps track of the coroutine of moving the cover
    /// </summary>
    [RequireComponent(typeof(DopplerVisualiser))]
    public class GraphAnimator : MonoBehaviour
    {
        private float speed = 3.0f;
        private float height = 200f;
        private Transform cover;
        private float _timer = 0.0f;
        private Coroutine _currentCoroutine;

        public void Start()
        {
            cover = transform.GetChild(0)?.transform;
        }

        private void OnEnable()
        {
            GetComponent<DopplerVisualiser>().dopplerUpdate += Animate;
        }

        public void Animate()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            _currentCoroutine = StartCoroutine(MoveCover());
        }

        private void OnDisable()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
            }
            GetComponent<DopplerVisualiser>().dopplerUpdate -= Animate;
            cover.localPosition = -cover.up * height;
        }

        private IEnumerator MoveCover()
        {
            _timer = 0.0f;
            Vector3 start = new Vector3(0, 0, 0);
            Vector3 end = -cover.up * height; // not width and right because the images are rotated
            while (_timer < speed)
            {
                _timer += Time.deltaTime;
                cover.localPosition = Vector3.Lerp(start, end, _timer/speed); 
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
