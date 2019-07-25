using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SvgToPngApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SvgToBitmapController : ControllerBase
    {
        //Default function, so resolves to /api/SvgToBitmap/
        [HttpPost]
        public ActionResult Get()
        {
            string requestBody;
            using(StreamReader sr = new StreamReader(Request.Body))
            {
                requestBody = sr.ReadToEnd();
            }
            
            if(string.IsNullOrEmpty(requestBody)) 
            {
                return new BadRequestObjectResult("No content, expected SVG content");
            }

            LogInformation($"Processing content with length: " + requestBody.Length);
            LogInformation(requestBody);

            var xd = new System.Xml.XmlDocument();
            try
            {
                xd.LoadXml(requestBody);
                LogInformation("XML Document parsed");
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult($"The provided content was not a valid XML: {ex.Message}");
            }

            Svg.SvgDocument d = null;
            try
            {
                d = Svg.SvgDocument.Open(xd);
                LogInformation("SVG Document loaded");
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult($"Could not render bitmap: {ex.Message}");
            }

            byte[] res;
            try
            {
                if(d == null) 
                {
                    throw new Exception("Image was null?");
                }
                var b = d.Draw();
                
                LogInformation("Drawn to the canvas");
                using(var ms = new MemoryStream())
                {
                    LogInformation("Trying to write to memory");
                    b.Save(ms, ImageFormat.Png);
                    LogInformation("Written to memory");       
                    res = ms.ToArray();
                }
            }
            catch(Exception ex)
            {
               return new BadRequestObjectResult($"Cannot render image: {ex.Message}");
            }

            return File(res, "image/png"); 
        }

        private void LogInformation(object data)
        {
            Console.WriteLine(data);
            Trace.WriteLine(data);
            Debug.WriteLine(data);
        }

    }
}
