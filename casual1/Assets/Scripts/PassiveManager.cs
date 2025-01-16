using UnityEngine;

public class PassiveManager : MonoBehaviour
{
	public static PassiveManager instance = null;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}
}
