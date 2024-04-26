//Extension methods for HttpRequestData to help with parsing, converting and returning results
public static class HttpRequestDataExtensions
{
    /// <summary>
    /// Will extract the body of the request and parse it as a JToken
    /// </summary>
    /// <param name="req"></param>
    /// <returns>JToken with the content of the body</returns>
    public static async Task<JToken> ParseAsJson(this HttpRequestData req)
    {
        StreamReader reader = new StreamReader(req.Body);
        string text = await reader.ReadToEndAsync();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }
        JToken jsonData = JToken.Parse(text);
        return jsonData;
    }

    /// <summary>
    /// Attempts to convert the input to a certain type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="req"></param>
    /// <returns></returns>
    public static async Task<T> Convert<T>(this HttpRequestData req)
    {
        var json = await req.ParseAsJson();
        if(json != null)
        {
            return JsonConvert.DeserializeObject<T>(json.ToString());
        }
        return default(T);
    }

    public static async Task<string> GetBody(this HttpRequestData req)
    {
        StreamReader reader = new StreamReader(req.Body);
        string text = await reader.ReadToEndAsync();
        return text;
    }

    public static async Task<HttpResponseData> CreateJsonResponse(this HttpRequestData req, object dataObject)
    {
        var response = req.CreateResponse();
        response.StatusCode = System.Net.HttpStatusCode.OK;
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        string jsonContent = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
        await response.WriteStringAsync(jsonContent); // Use WriteStringAsync instead of WriteString
        return response;
    }

    public static async Task<HttpResponseData> CreateHtmlResponse(this HttpRequestData req, string htmlString)
    {
        var response = req.CreateResponse();
        response.StatusCode = System.Net.HttpStatusCode.OK;
        response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        await response.WriteStringAsync(htmlString);
        return response;
    }

    public static async Task<HttpResponseData> CreateMimeResponse(this HttpRequestData req, byte[] content, string mimetype, string filename)
    {
        var response = req.CreateResponse();
        response.StatusCode = System.Net.HttpStatusCode.OK;
        response.Headers.Add("Content-Disposition", $"attachment; filename=\"{filename}\"");
        response.Headers.Add("Content-Type", mimetype);
        await response.Body.WriteAsync(content, 0, content.Length);
        return response;
    }

    public static async Task<HttpResponseData> CreateExceptionResponse(this HttpRequestData req, Exception ex)
    {
        var response = req.CreateResponse();
        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(JsonConvert.SerializeObject(ex));
        return response;
    }

    public static async Task<HttpResponseData> CreateJsonResponse(this HttpRequestData req, string dataJson)
    {
        var response = req.CreateResponse();
        response.StatusCode = System.Net.HttpStatusCode.OK;
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        await response.WriteStringAsync(dataJson);
        return response;
    }

    public static async Task<HttpResponseData> CreateBadResponse(this HttpRequestData req, string errorMessage)
    {
        var response = req.CreateResponse();
        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        //create an error object
        dynamic error = new ExpandoObject();
        error.error = errorMessage;
        string jsonContent = JsonConvert.SerializeObject(error, Formatting.Indented);
        await response.WriteStringAsync(jsonContent);
        return response;
    }
}
