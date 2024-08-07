using UnityEngine;

public static class AudioSourceTools
{
	public static void PlaySFX(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool loop, float delay, float volume, float pitch)
	{
		SoundManager.PlaySFX(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name), loop, delay, volume, pitch);
	}

	public static void PlaySFX(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool loop, float delay, float volume)
	{
		SoundManager.PlaySFX(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name), loop, delay, volume);
	}

	public static void PlaySFX(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool loop, float delay)
	{
		SoundManager.PlaySFX(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name), loop, delay);
	}

	public static void PlaySFX(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool loop)
	{
		SoundManager.PlaySFX(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name), loop);
	}

	public static void PlaySFX(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name)
	{
		SoundManager.PlaySFX(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name));
	}

	public static void PlaySFX(ref AudioSource theAudioSource, AudioClip clip, bool loop, float delay, float volume, float pitch)
	{
		SoundManager.PlaySFX(theAudioSource, clip, loop, delay, volume, pitch);
	}

	public static void PlaySFX(ref AudioSource theAudioSource, AudioClip clip, bool loop, float delay, float volume)
	{
		SoundManager.PlaySFX(theAudioSource, clip, loop, delay, volume);
	}

	public static void PlaySFX(ref AudioSource theAudioSource, AudioClip clip, bool loop, float delay)
	{
		SoundManager.PlaySFX(theAudioSource, clip, loop, delay);
	}

	public static void PlaySFX(ref AudioSource theAudioSource, AudioClip clip, bool loop)
	{
		SoundManager.PlaySFX(theAudioSource, clip, loop);
	}

	public static void PlaySFX(ref AudioSource theAudioSource, AudioClip clip)
	{
		SoundManager.PlaySFX(theAudioSource, clip);
	}

	public static void StopSFX(ref AudioSource theAudioSource)
	{
		SoundManager.StopSFXObject(theAudioSource);
	}

	public static void PlaySFXLoop(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool tillDestroy, float volume, float pitch, float maxDuration)
	{
		SoundManager.PlaySFXLoop(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name), tillDestroy, volume, pitch, maxDuration);
	}

	public static void PlaySFXLoop(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool tillDestroy, float volume, float pitch)
	{
		SoundManager.PlaySFXLoop(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name), tillDestroy, volume, pitch);
	}

	public static void PlaySFXLoop(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool tillDestroy, float volume)
	{
		SoundManager.PlaySFXLoop(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name), tillDestroy, volume);
	}

	public static void PlaySFXLoop(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name, bool tillDestroy)
	{
		SoundManager.PlaySFXLoop(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name), tillDestroy);
	}

	public static void PlaySFXLoop(ref AudioSource theAudioSource, bool fromGroup, string clipOrGroup_Name)
	{
		SoundManager.PlaySFXLoop(theAudioSource, (!fromGroup) ? SoundManager.Load(clipOrGroup_Name) : SoundManager.LoadFromGroup(clipOrGroup_Name));
	}
}
