using System;
using System.Collections;
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

        public bool ShowMaxValues = true;

        [SerializeField] private RectTransform labelTemplateY;
        [SerializeField] private RectTransform tickTemplateY;
        [SerializeField] private RectTransform tickTemplateX;
        [SerializeField] private RectTransform xAxis;
        [SerializeField] private RectTransform loadingLine;

        // "Max PRF: 22\tMax Velocity: ??"
        [SerializeField] private Text maxValues;
        
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
            loadingLine.gameObject.SetActive(false);
            CreateAxis();
            UpdateMaxValues();
        }

        private void UpdateMaxValues()
        {
            if (ShowMaxValues)
            {
                string velocityColour = Simulator.IsVelocityOverMax ? "red" : "green";
                var roundedMaxVelocity = Mathf.Round(Simulator.MaxVelocity * 10) / 10;
                maxValues.text = $"Max PRF: {Mathf.RoundToInt(Simulator.MaxPRF)} kHz                      " +
                                 $"Max Velocity: <color={velocityColour}>{roundedMaxVelocity}</color> cm/s";
            }
            else
            {
                maxValues.text = "";
            }
        }

        private void CreateAxis()
        {
            const float gapY = 10f;
            const int velocityStepY = 30;
            const int timeStepX = velocityStepY;
            Transform parent = transform.parent;
            
            for (int tick = -4; tick < 6; tick++)
            {
                RectTransform tickY = Instantiate(tickTemplateY, parent);
                tickY.anchoredPosition = new Vector2(tickTemplateY.anchoredPosition.x, gapY * tick + xAxis.anchoredPosition.y);
                tickY.gameObject.SetActive(true);
                
                if(tick <= 0)
                    continue;
                RectTransform labelY = Instantiate(labelTemplateY, parent);
                labelY.anchoredPosition = new Vector2(labelTemplateY.anchoredPosition.x, gapY * tick + xAxis.anchoredPosition.y);
                labelY.gameObject.SetActive(true);
                labelY.GetComponent<Text>().text = (tick* 20f).ToString();
            }

            for (int tick = 1; tick < 7; tick++)
            {
                RectTransform tickX = Instantiate(tickTemplateX, parent);
                tickX.anchoredPosition = new Vector2( tickTemplateX.anchoredPosition.x - timeStepX * tick, tickTemplateX.anchoredPosition.y);
                tickX.gameObject.SetActive(true);
            }

        }

        public void UpdateDoppler()
        {
            UpdateMaxValues();
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
            loadingLine.gameObject.SetActive(true);
            for (int t = Simulator.n_timepoints - 1; t >= 0; t--) // have to go in opposite direction to go from left to right
            {
                Simulator.UpdatePlot(t);
                loadingLine.anchoredPosition = new Vector2(Simulator.n_timepoints - t, 0);
                yield return new WaitForUpdate();
            }
            
            loadingLine.gameObject.SetActive(false);
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