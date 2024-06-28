using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween PunchScale")]
public class TweenPunchScale : UITweener
{
	public Vector3 from = Vector3.one;

	public Vector3 to = Vector3.one;

	private Transform mTrans;

	[HideInInspector]
	public Vector3 vecGap = Vector3.zero;

	[HideInInspector]
	public Vector3 vecAmplitude = Vector3.zero;

	private float m_fPeriod = 0.3f;

	private float m_fSpace;

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	public Vector3 value
	{
		get
		{
			return cachedTransform.localScale;
		}
		set
		{
			cachedTransform.localScale = value;
		}
	}

	[Obsolete("Use 'value' instead")]
	public Vector3 scale
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	private void Awake()
	{
		mTrans = base.transform;
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		UpdatePunchScaleTargets(factor);
	}

	private void UpdatePunchScaleTargets(float factor)
	{
		vecGap = to - from;
		if (to.x > 0f)
		{
			vecAmplitude.x = punch(vecGap.x, factor);
		}
		else if (to.x < 0f)
		{
			vecAmplitude.x = 0f - punch(Mathf.Abs(vecGap.x), factor);
		}
		if (to.y > 0f)
		{
			vecAmplitude.y = punch(vecGap.y, factor);
		}
		else if (to.y < 0f)
		{
			vecAmplitude.y = 0f - punch(Mathf.Abs(vecGap.y), factor);
		}
		if (to.z > 0f)
		{
			vecAmplitude.z = punch(vecGap.z, factor);
		}
		else if (to.z < 0f)
		{
			vecAmplitude.z = 0f - punch(Mathf.Abs(vecGap.z), factor);
		}
		value = from + vecAmplitude;
	}

	private float punch(float amplitude, float factor)
	{
		if (factor == 0f)
		{
			return 0f;
		}
		if (factor == 1f)
		{
			return 0f;
		}
		m_fSpace = m_fPeriod / 6.28318548f * Mathf.Asin(0f);
		return amplitude * Mathf.Pow(2f, -10f * factor) * Mathf.Sin((factor * 1f - m_fSpace) * 6.28318548f / m_fPeriod);
	}

	public static TweenPunchScale Begin(GameObject go, float duration, Vector3 scale)
	{
		TweenPunchScale tweenPunchScale = UITweener.Begin<TweenPunchScale>(go, duration);
		tweenPunchScale.from = tweenPunchScale.value;
		tweenPunchScale.to = scale;
		tweenPunchScale.vecGap = tweenPunchScale.to - tweenPunchScale.from;
		tweenPunchScale.vecAmplitude = Vector3.zero;
		if (duration <= 0f)
		{
			tweenPunchScale.Sample(1f, isFinished: true);
			tweenPunchScale.enabled = false;
		}
		return tweenPunchScale;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		from = value;
	}

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		to = value;
	}

	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		value = from;
	}

	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		value = to;
	}
}