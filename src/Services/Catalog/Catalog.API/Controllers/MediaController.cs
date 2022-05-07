using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using System.Collections.Generic;

namespace Microsoft.eShopOnDapr.Services.Catalog.API.Controllers
{
    public class MediaController : Controller
    {
        IConfiguration Config { get; }
        protected IOptionsMonitor<MvcOptions> OptionsMonitor { get; }
        public MediaController(IConfiguration config)
        {
            Config = config;
        }

        /// <summary>
        /// 上传疵点图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/v1/catalog/upload")]
        public async Task<ActionResult> Upload([Required] IFormFile file)
        {
            if (file != null && file.Length < 1)
            {
            }
            string endpoint = Config["Minio:Endpoint"];
            string accessKey = Config["Minio:AccessKey"];
            string secretKey = Config["Minio:SecretKey"];
            string bucketName = Config["Minio:BucketName"];
            string domainHost = Config["Host"];
            MinioClient minio = new MinioClient()
                                   .WithEndpoint(endpoint)
                                   .WithCredentials(accessKey, secretKey)
                                   .Build();
            using (Stream stream = file.OpenReadStream())
            {
                Dictionary<string, string> metaData = new Dictionary<string, string>
                    {
                        { "Name", $"{file.Name}" }
                    };
                PutObjectArgs args = new PutObjectArgs()
                                                .WithBucket(bucketName)
                                                .WithObject(file.FileName)
                                                .WithStreamData(stream)
                                                .WithObjectSize(stream.Length)
                                                .WithContentType("application/octet-stream")
                                                .WithHeaders(metaData);
                await minio.PutObjectAsync(args);
                return Json(new { });
            }
        }
        /// <summary>
        /// http://192.168.1.5:5101/api/v1/catalog/img/10.jpeg
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        [HttpGet]//
        [Route("/api/v1/catalog/img/{objectName}")]
        public async Task<IActionResult> GetFileAsync(string objectName = "hello.jpg")
        {
            string endpoint = Config["Minio:Endpoint"];
            string accessKey = Config["Minio:AccessKey"];
            string secretKey = Config["Minio:SecretKey"];
            var bucketName = Config["Minio:BucketName"];
            string fileName = Path.Combine(Directory.GetCurrentDirectory(), objectName);

            MinioClient minio = new MinioClient()
                                     .WithEndpoint(endpoint)
                                     .WithCredentials(accessKey, secretKey)
                                     .Build();
            GetObjectArgs getObjectArgs = new GetObjectArgs()
                                                      .WithBucket(bucketName)
                                                      .WithObject(objectName)
                                                        .WithFile(fileName);
            ObjectStat stat = await minio.GetObjectAsync(getObjectArgs);
            return new PhysicalFileResult(fileName, stat.ContentType);
        }
    }
}
