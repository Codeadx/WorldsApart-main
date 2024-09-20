using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationEasingFunctions
{
    public static Quaternion LinearEasing(float time, float duration, Quaternion startRotation, Quaternion endRotation) {
        float t = Mathf.Clamp01(time / duration); // time is the current time in transition, duration is total time
        Quaternion targetRotation = Quaternion.Slerp(startRotation, endRotation, t);
        return targetRotation;
    }

    public static Quaternion QuadraticEaseIn(float time,  float duration, Quaternion startRotation, Quaternion endRotation) {
        float t = Mathf.Clamp01(time / duration);
        float easedT = t * t;  // Quadratic ease-in
        Quaternion targetRotation = Quaternion.Slerp(startRotation, endRotation, easedT);
        return targetRotation;
    }

    public static Quaternion QuadraticEaseOut(float time,  float duration, Quaternion startRotation, Quaternion endRotation) {
        float t = Mathf.Clamp01(time / duration);
        float easedT = 1 - (1 - t) * (1 - t);  // Quadratic ease-out
        Quaternion targetRotation = Quaternion.Slerp(startRotation, endRotation, easedT);
        return targetRotation;
    }

    public static Quaternion QuadraticEaseInOut(float time,  float duration, Quaternion startRotation, Quaternion endRotation) {
        float t = Mathf.Clamp01(time / duration);
        float easedT = t * t * (3 - 2 * t);  // Smoothstep-like ease-in-out
        Quaternion targetRotation = Quaternion.Slerp(startRotation, endRotation, easedT);
        return targetRotation;
    }

    public static Quaternion CubicEaseInOut(float time,  float duration, Quaternion startRotation, Quaternion endRotation) {
        float t = Mathf.Clamp01(time / duration);
        float easedT = t * t * t * (t * (6f * t - 15f) + 10f);  // Cubic ease-in-out
        Quaternion targetRotation = Quaternion.Slerp(startRotation, endRotation, easedT);
        return targetRotation;
    }

    public static Quaternion ExponentialEaseInOut(float time,  float duration, Quaternion startRotation, Quaternion endRotation) {
        float EaseInOutExpo(float t) {
            return t == 0 ? 0 : t == 1 ? 1 : t < 0.5 ? Mathf.Pow(2, 20 * t - 10) / 2 : (2 - Mathf.Pow(2, -20 * t + 10)) / 2;
        }
        float t = Mathf.Clamp01(time / duration);
        Quaternion targetRotation = Quaternion.Slerp(startRotation, endRotation, EaseInOutExpo(t));
        return targetRotation;
    }

    public static Quaternion ElasticEaseOut(float time,  float duration, Quaternion startRotation, Quaternion endRotation) {
        float EaseOutElastic(float t) {
        float c4 = (2 * Mathf.PI) / 3;
            return t == 0 ? 0 : t == 1 ? 1 : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
        }
        float t = Mathf.Clamp01(time / duration);
        Quaternion targetRotation = Quaternion.Slerp(startRotation, endRotation, EaseOutElastic(t));
        return targetRotation;
    }
}
