using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;

    [SerializeField, Range(10, 100)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    Transform[] points;

    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    float duration;


    void Awake()
    {
        //var scale = Vector3.one / 5f;
        //for (int i = 0; i < 10; i++)
        //{
        //    Transform point = Instantiate(pointPrefab);
        //    point.localPosition = Vector3.right * ((i + 0.5f) / 5f - 1f);
        //    point.localScale = scale;
        //}

        points = new Transform[resolution * resolution];

        float step = 2f / resolution;
        var scale = Vector3.one * step;

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);

            point.localScale = scale;
            point.SetParent(transform, false);

        }
    }
    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        } else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }
        if (transitioning)
        {
            UpdateFunctionTransition();
        }else
        UpdateFunction();
    }
    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
    private void UpdateFunction()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);

        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;

        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;

            }
            float u = (x + 0.5f) * step - 1f;

            points[i].localPosition = f(u, v, time);

        }
    }
    private void UpdateFunctionTransition()
    {
        FunctionLibrary.Function from = FunctionLibrary.GetFunction(transitionFunction),
                                 to = FunctionLibrary.GetFunction(function);

        float progress = duration / transitionDuration;
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;

        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;

            }
            float u = (x + 0.5f) * step - 1f;

            points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);

        }
    }
}