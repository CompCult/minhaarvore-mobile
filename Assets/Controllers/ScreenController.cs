using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour 
{
	protected static string backScene, nextScene;

	public virtual void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
			LoadPreviousView();
	}

	public void LoadNextView()
	{
		LoadView(nextScene);
	}

	public void LoadPreviousView()
	{
		LoadView(backScene);
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

	private string GetViewName()
	{
		Scene scene = SceneManager.GetActiveScene();
		return scene.name;
	}
}


