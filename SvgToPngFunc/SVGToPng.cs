using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Svg;
using System.Drawing.Imaging;

namespace SvgToPng
{
    public static class SVGToPng
    {
        [FunctionName("SVGToPng")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if(string.IsNullOrEmpty(requestBody)) 
            {
                return new BadRequestObjectResult("No content, expected SVG content");
            }

            log.LogInformation($"Processing content with length: " + requestBody.Length);
            log.LogInformation(requestBody);

            var xd = new System.Xml.XmlDocument();
            try
            {
                xd.LoadXml(requestBody);
                log.LogInformation("XML Document parsed");
            }
            catch(Exception ex)
            {
                log.LogError(ex, "Error loading XML");
                return new BadRequestObjectResult($"The provided content was not a valid XML: {ex.Message}");
            }

            SvgDocument d = null;
            try
            {
                d = Svg.SvgDocument.Open(xd);
                log.LogInformation("SVG Document loaded");
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult($"Could not render bitmap: {ex.Message}");
            }

            try
            {
                if(d == null) 
                {
                    throw new Exception("Image was null?");
                }
                var b = d.Draw();
                log.LogInformation("Drawn to the canvas");
                using(var ms = new MemoryStream())
                {
                    log.LogInformation("Trying to write to memory");
                    b.Save(ms, ImageFormat.Png);
                    log.LogInformation("Written to memory");                
                }
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult($"Cannot render image: {ex.Message}");
            }
            // name = name ?? data?.name;
            return new OkObjectResult("Blaa");
            // return name != null
            //     ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //     : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
