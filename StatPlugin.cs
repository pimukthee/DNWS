using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.Redis;

namespace DNWS
{
  class StatPlugin : IPlugin
  {
    protected static RedisManagerPool manager = null;
    protected static Dictionary<String, int> statDictionary = null;
    public StatPlugin()
    {
      manager =  new RedisManagerPool("redis:6379");
    }

    public void PreProcessing(HTTPRequest request)
    {
      using (var client = manager.GetClient()) 
       {
           client.IncrementValue(request.Url);
       }
    }
    public virtual HTTPResponse GetResponse(HTTPRequest request)
    {
      HTTPResponse response = null;
      StringBuilder sb = new StringBuilder();
      sb.Append("<html><body><h1>Stat:</h1>");
      using (var client = manager.GetClient()) {
        List<String> entrys = client.GetAllKeys();
        foreach (String entry in entrys)
        {
          sb.Append(entry + ": " + client.GetValue(entry) + "<br />");
        }
      }
      sb.Append("</body></html>");
      response = new HTTPResponse(200);
      response.Body = Encoding.UTF8.GetBytes(sb.ToString());
      return response;
    }

    public HTTPResponse PostProcessing(HTTPResponse response)
    {
      throw new NotImplementedException();
    }
  }
}