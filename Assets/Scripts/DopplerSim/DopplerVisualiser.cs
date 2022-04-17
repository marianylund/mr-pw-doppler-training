using System;
using System.Collections;
using System.Threading;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace DopplerSim
{
    [RequireComponent(typeof(RawImage))]
    public class DopplerVisualiser : MonoBehaviour
    {
        public delegate void OnDopplerVisualiser();
        public OnDopplerVisualiser dopplerUpdate;

        private RawImage _rawImage;
        public DopplerSimulator Simulator;

        private Coroutine _currentCoroutine;
        private Coroutine _secondCoroutine;

        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
            Simulator = new DopplerSimulator();
            _rawImage.texture = Simulator.CreatePlot();
            _rawImage.SetNativeSize();

        }

        public void UpdateDoppler()
        {
            if (_currentCoroutine == null)
            {
                _currentCoroutine = StartCoroutine(UpdateDopplerGraphRoutine(() => _currentCoroutine = null));
                dopplerUpdate?.Invoke();
            }
            else if (_secondCoroutine == null)
            {
                _secondCoroutine = StartCoroutine(UpdateDopplerGraphRoutine(() => _secondCoroutine = null));
                dopplerUpdate?.Invoke();
            }
        }

        private IEnumerator UpdateDopplerGraphRoutine(Action onFinish)
        {
            for (int t = Simulator.n_timepoints - 1; t >= 0; t--) // have to go in opposite direction to go from left to right
            {
                Simulator.UpdatePlot(t);
                yield return new WaitForUpdate();
            }

            onFinish();
        }

        private void OnDisable()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
            if (_secondCoroutine != null)
            {
                StopCoroutine(_secondCoroutine);
                _secondCoroutine = null;
            }
        }
    }
}