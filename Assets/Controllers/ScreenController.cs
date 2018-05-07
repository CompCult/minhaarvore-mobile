using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour 
{
	protected static string previousView, nextView;

	public virtual void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
			LoadPreviousView();
	}

	public void LoadNextView()
	{
		LoadView(nextView);
	}

	public void LoadPreviousView()
	{
		LoadView(previousView);
	}

	public void LoadView(string Scene) 
	{
		if (Scene != null) 
			SceneManager.LoadScene(Scene);
		else
			Application.Quit();
	}

	public void ReloadView()
	{
        SceneManager.LoadScene(GetViewName());
	}

	public string GetViewName()
	{
		Scene scene = SceneManager.GetActiveScene();
		return scene.name;
	}
}


