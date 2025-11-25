using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public TMP_Text distanceText, webText;

	public Transform playerTransform;
	float distanceTravelled = 0f;
	bool firstFall = true;

	public AudioClip[] fallSounds;
	AudioSource audioSource;
	bool gameOver = false;
	private void Start()
	{
		WebController.OnWebAttached += UpdateWebs;
		WebController.OnFall += HandlePlayerFall;
		audioSource = GetComponent<AudioSource>();
	}

	private void OnDestroy()
	{
		WebController.OnWebAttached -= UpdateWebs;
		WebController.OnFall -= HandlePlayerFall;
	}

	private void Update()
	{
		if(playerTransform.position.x > distanceTravelled)
		{
			distanceTravelled = playerTransform.position.x;
			UpdateDistance(distanceTravelled);
		}
	}

	public void UpdateDistance(float distance)
    {
        distanceText.text = "Distance: " + distance.ToString("F2") + " m";
	}

    public void UpdateWebs(int webs)
    {
        webText.text = "Webs: " + webs.ToString();
	}

	void HandlePlayerFall()
	{
		StartCoroutine(HandlePlayerFallRoutine());
	}

	IEnumerator HandlePlayerFallRoutine()
	{
		if(gameOver)
			yield break;
		if (firstFall)
		{
			firstFall = false;
			yield break;
		}
		gameOver = true;
		int randomIndex = Random.Range(0, fallSounds.Length);
		audioSource.PlayOneShot(fallSounds[randomIndex]);
		playerTransform.GetComponent<WebController>().SetCanAttachWeb(false);
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
