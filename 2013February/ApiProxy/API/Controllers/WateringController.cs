using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Text;

namespace API.Controllers
{
    public class WateringController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            NameValueCollection queryParams = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            NameValueCollection postVars = new NameValueCollection();

            string url = queryParams["url"] ?? "http://www.snwa.com/apps/watering_group/get_wg_data.cfml";
            string address = queryParams["address"] ?? "317 S 6th Street";
            string city = queryParams["city"] ?? "Las Vegas";
            string zip = queryParams["zip"] ?? "89101";

            postVars["WGstreetaddress"] = HttpUtility.UrlEncode(address);
            postVars["WGcity"] = HttpUtility.UrlEncode(city);
            postVars["WGzip"] = HttpUtility.UrlEncode(zip);

            WebClient client = new WebClient();
            byte[] bytes = client.UploadValues(url, postVars);
            string page = Encoding.UTF8.GetString(bytes);

            try
            {
                int startIndex = page.IndexOf("<h4>Watering Days");
                int endIndex = page.IndexOf("<h4>Water Provider");
                page = page.Substring(startIndex, endIndex - startIndex);
            }
            catch
            {
                return new string[] { "false", "false", "false", "true", "false", "false", "false" };
            }

            //string[] days = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            string[] days = new string[] { "true", "true", "true", "true", "true", "true", "true" };

            string falseValue = "false";
            if (!page.Contains("Sun"))
                days[0] = falseValue;
            if (!page.Contains("Mon"))
                days[1] = falseValue;
            if (!page.Contains("Tue"))
                days[2] = falseValue;
            if (!page.Contains("Wed"))
                days[3] = falseValue;
            if (!page.Contains("Thu"))
                days[4] = falseValue;
            if (!page.Contains("Fri"))
                days[5] = falseValue;
            if (!page.Contains("Sat"))
                days[6] = falseValue;

            return days;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}