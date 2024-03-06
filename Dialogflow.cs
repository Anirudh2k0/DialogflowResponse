using System.Collections; 

using UnityEngine; 

using UnityEngine.Networking; 

using UnityEngine.UI; 

using System.Text; 

using TMPro; 

  

public class AccessToken : MonoBehaviour 

{ 

    private string projectId = "Your ProjectID"; 

   private string accessToken = "Your Access Token that you generated with /gcloud auth application-default print-access-token 

 "; 

    private string dialogflowEndpoint = "https://dialogflow.googleapis.com/v2/projects/"; 

  

    // UI text input field reference 

    public TMP_InputField inputField; 

    public TextMeshProUGUI responseText; 

  

    [System.Serializable] 

    private class QueryInput 

    { 

        public TextInput text; 

        internal TextInput queryInput; 

    } 

  

    [System.Serializable] 

    private class QueryInputWrapper 

    { 

        public QueryInput queryInput; 

    } 

  

    [System.Serializable] 

    private class TextInput 

    { 

        public string text; 

        public string languageCode = "en-US"; 

    } 

  

    [System.Serializable] 

    private class ResponseBody 

    { 

        public QueryResult queryResult; 

    } 

  

    [System.Serializable] 

    private class QueryResult 

    { 

        public string fulfillmentText; 

    } 

  

    public string receivedMessage = ""; 

  

     

    void Start() 

    { 

        if (inputField != null) 

        { 

            inputField.onEndEdit.AddListener(OnEndEdit); 

        } 

    } 

  

    // Event handler for the input field's onEndEdit event 

    void OnEndEdit(string input) 

    { 

        receivedMessage = input; 

        Debug.Log("Received Message: " + receivedMessage); 

  

        // Call the method to send the text query to Dialogflow 

        StartCoroutine(SendTextQueryToDialogflow(receivedMessage)); 

  

        // Clear the input field after processing the input 

        if (inputField != null) 

        { 

            inputField.text = ""; 

        } 

    } 

  

    IEnumerator SendTextQueryToDialogflow(string textQuery) 

    { 

        string url = $"{dialogflowEndpoint}{projectId}/agent/sessions/default:detectIntent"; 

  

        QueryInput queryInput = new QueryInput 

        { 

            text = new TextInput 

            { 

                text = textQuery 

            } 

        }; 

  

        QueryInputWrapper queryInputWrapper = new QueryInputWrapper 

        { 

            queryInput = queryInput 

        }; 

  

        string jsonRequestBody = JsonUtility.ToJson(queryInputWrapper); 

        Debug.Log(jsonRequestBody); 

  

        using (UnityWebRequest www = new UnityWebRequest(url, "POST")) 

        { 

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody); 

            www.uploadHandler = new UploadHandlerRaw(bodyRaw); 

            www.downloadHandler = new DownloadHandlerBuffer(); 

            www.SetRequestHeader("Authorization", $"Bearer {accessToken}"); 

            www.SetRequestHeader("Content-Type", "application/json"); 

  

            yield return www.SendWebRequest(); 

  

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) 

            { 

                Debug.Log(www.result); 

                Debug.LogError($"Error: {www.error.ToString()}"); 

            } 

            else 

            { 

                string jsonResponse = www.downloadHandler.text; 

                Debug.Log($"Response: {jsonResponse}"); 

  

                ResponseBody response = JsonUtility.FromJson<ResponseBody>(jsonResponse); 

  

                string fulfillmentText = response.queryResult.fulfillmentText; 

                Debug.Log($"Fulfillment Text: {fulfillmentText}"); 

                if (responseText != null) 

                { 

                    responseText.text = fulfillmentText;  

                } 

                if (inputField != null) 

                { 

                    inputField.text = textQuery;  

                } 

  

            } 

        } 

    } 

} 
