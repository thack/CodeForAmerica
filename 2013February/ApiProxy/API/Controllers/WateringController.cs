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
        public IEnumerable<bool> Get()
        {
            // default values if not defined by query string
            NameValueCollection queryParams = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            string url = queryParams["url"] ?? "http://www.snwa.com/apps/watering_group/get_wg_data.cfml";
            string address = queryParams["address"] ?? "317 S 6th Street";
            string city = queryParams["city"] ?? "Las Vegas";
            string zip = queryParams["zip"] ?? "89101";

            NameValueCollection postVars = new NameValueCollection();
            postVars["WGstreetaddress"] = address;
            postVars["WGcity"] = city;
            postVars["WGzip"] = zip;

            // send http request with post variables
            WebClient client = new WebClient();
            byte[] bytes = client.UploadValues(url, postVars);
            string page = Encoding.UTF8.GetString(bytes);

            try
            {
                // look for content containing watering days
                int startIndex = page.IndexOf("<h4>Watering Days");
                int endIndex = page.IndexOf("<h4>Water Provider");
                page = page.Substring(startIndex, endIndex - startIndex);
            }
            catch
            {
                // mobile client doesn't have logic to deal with error conditions, return default
                return new bool[] { false, false, false, true, false, false, false };
            }

            bool[] days = new bool[] { false, false, false, false, false, false, false };

            if (page.Contains("Sun"))
                days[0] = true;
            if (page.Contains("Mon"))
                days[1] = true;
            if (page.Contains("Tue"))
                days[2] = true;
            if (page.Contains("Wed"))
                days[3] = true;
            if (page.Contains("Thu"))
                days[4] = true;
            if (page.Contains("Fri"))
                days[5] = true;
            if (page.Contains("Sat"))
                days[6] = true;

            return days;
        }

        // GET api/values/5
        public bool Get(int id)
        {
            return false;
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