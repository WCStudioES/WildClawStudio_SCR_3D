using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using UnityEngine.Networking;

public class GitHubUpdater : MonoBehaviour
{
    private string repoUrl = "https://api.github.com/repos/SCR3DPublicServer/SCR3DPublicServer/contents/PublicServer.json";
    private string token = "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"; // Nunca lo incluyas directamente en producción.

    [Serializable]
    public class GitHubContent
    {
        public string message; // Mensaje del commit.
        public string content; // Contenido del archivo en base64.
        public string sha; // Identificador del archivo actual.
    }


    public void actualizarCodigoServidorPublico(string codigo)
    {
        // Actualiza el JSON
        string nuevoContenido = "{ \"joinCode\": \"" + codigo + "\" }"; // JSON de ejemplo.
        StartCoroutine(ActualizarArchivo(nuevoContenido, "Actualización desde Unity"));
    }

    private IEnumerator ActualizarArchivo(string nuevoContenido, string commitMessage)
    {
          // 1. Solicitud GET para obtener el SHA del archivo actual
        UnityWebRequest getRequest = UnityWebRequest.Get(repoUrl);
        getRequest.SetRequestHeader("Accept", "application/vnd.github+json");
        getRequest.SetRequestHeader("Authorization", "Bearer " + token); // Espacio corregido
        getRequest.SetRequestHeader("X-GitHub-Api-Version", "2022-11-28");

        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener el archivo: " + getRequest.error);
            yield break;
        }

        var jsonResponse = JsonUtility.FromJson<GitHubContent>(getRequest.downloadHandler.text);

        // 2. Codifica el nuevo contenido en base64
        string nuevoContenidoBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(nuevoContenido));

        // 3. Prepara el cuerpo del PUT para actualizar el archivo
        GitHubContent updateContent = new GitHubContent
        {
            message = commitMessage,
            content = nuevoContenidoBase64,
            sha = jsonResponse.sha
        };

        string jsonPayload = JsonUtility.ToJson(updateContent);

        // 4. Configura UnityWebRequest para PUT
        UnityWebRequest putRequest = new UnityWebRequest(repoUrl, "PUT");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        putRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        putRequest.downloadHandler = new DownloadHandlerBuffer();
        putRequest.SetRequestHeader("Accept", "application/vnd.github+json");
        putRequest.SetRequestHeader("Authorization", "Bearer " + token); // Espacio corregido
        putRequest.SetRequestHeader("X-GitHub-Api-Version", "2022-11-28");
        putRequest.SetRequestHeader("Content-Type", "application/json"); // Content-Type para JSON

        // 5. Envía la solicitud PUT
        yield return putRequest.SendWebRequest();

        if (putRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al actualizar el archivo: " + putRequest.error + "\nRespuesta: " + putRequest.downloadHandler.text);
        }
    }
}
