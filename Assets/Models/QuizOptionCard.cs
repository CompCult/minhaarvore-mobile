using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizOptionCard : MonoBehaviour
{
	#pragma warning disable 0108 0472

	public Text option;
	private string alternative;

	public void UpdateOption (string answer, string alternative)
	{
		this.alternative = alternative;
		option.text = answer;
	}

	public void SendResponse ()
	{
		StartCoroutine(_SendResponse());
	}

	private IEnumerator _SendResponse ()
	{
		AlertsService.makeLoadingAlert("Enviando");

		Quiz currentQuiz = QuizzesService.quiz;
		User currentUser = UserService.user;
		string answer = this.alternative;

		WWW responseRequest = QuizzesService.SendResponse(currentQuiz._id, currentUser._id, answer);

		while (!responseRequest.isDone)
			yield return new WaitForSeconds(0.1f);

		Debug.Log("Header: " + responseRequest.responseHeaders["STATUS"]);
		Debug.Log("Text: " + responseRequest.text);
		AlertsService.removeLoadingAlert();

		if (responseRequest.responseHeaders["STATUS"] == HTML.HTTP_200)
		{
			if (currentQuiz.HasCorrectAnswer())
				AlertsService.makeAlert("Resposta enviada", "Se você marcou a alternativa correta, será recompensado(a) com folhas em breve!", "");
			else
				AlertsService.makeAlert("Resposta enviada", "Um gestor analisará sua resposta e o(a) recompensará com folhas se a resposta for válida.", "");

			yield return new WaitForSeconds(4f);
			SceneManager.LoadScene("Quizzes");
			yield return null;
		}
		else 
		{
			AlertsService.makeAlert("Falha na conexão", "Tente novamente mais tarde.", "Entendi");
			SceneManager.LoadScene("Home");
		}

		yield return null;
	}
}
