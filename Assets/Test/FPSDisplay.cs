using UnityEngine;
using System.Collections;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
	public TextMeshProUGUI display_Text;
	string label = "";
	float count;

	IEnumerator Start()
	{
		while (true)
		{
			if (Time.timeScale == 1)
			{
				yield return new WaitForSeconds(0.1f);
				count = (1 / Time.deltaTime);
				display_Text.text = (Mathf.Round(count) + " FPS");
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}
