﻿using System;
using System.Threading.Tasks;
using ePlatform.Api;
using Microsoft.AspNetCore.Mvc;

namespace ePlatform.Api.SampleNetCoreApp
{

    [Route("api/[controller]")]
    [ApiController]
    public class OutboxInvoiceController : ControllerBase
    {
        private readonly IOutboxInvoiceClient _outboxInvoiceClient;

        public OutboxInvoiceController(
            IOutboxInvoiceClient outboxInvoiceClient)
        {
            _outboxInvoiceClient = outboxInvoiceClient;
        }

        [HttpGet("pdf/{id}")] //1d9a9825-d0bb-4303-9d87-f3ae435cedc4
        public async Task<ActionResult> GetPdf(Guid id)
        {
            var data = await _outboxInvoiceClient.GetPdf(id);
            return File(data, "application/pdf", $"{id}.zip");
        }

        [HttpPost("post-invoice")]
        public async Task<IActionResult> Post()
        {
            var filledModel = FillUblModel.fillUblModel(); // burada fatura numarası sistemden alınacak şekilde yapıldı.
            var data = await _outboxInvoiceClient.Post(filledModel);
            return Ok(data);
        }

        [HttpPut("update/{id}")] //94b3611a-2f8a-4745-83f6-d411f7afc3df
        public async Task<IActionResult> Update(Guid id)
        {
            /// Önemli: Sadece taslak durumundaki faturalarda değişiklik yapılabilir.

            var updateModel = FillUblModel.fillUblModel();
            updateModel.InvoiceId = id;
            updateModel.GeneralInfoModel.Ettn = id;
            updateModel.GeneralInfoModel.InvoiceNumber = "Gönderilen ilk fatura numarası"; // Burak Düzeltecek.

            var data = await _outboxInvoiceClient.Update(id, updateModel);
            return Ok(data);
        }


        //[HttpPost("post-ubl")]
        //public async Task<IActionResult> PostUBL()
        //{
        //    //Önceden oluşturulmuş Fatura UBL'i okuyarak, servisin istediği önceden ziplenmiş base64 formata çeviriyorum..
        //    var ubl = new Api.Invoice.Models.Invoice21.InvoiceType();
        //    string path = Path.Combine(Environment.CurrentDirectory, "Static");
        //    var invoiceXml = await System.IO.File.ReadAllTextAsync(Path.Combine(path, "Invoice.xml"), Encoding.UTF8);
        //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(invoiceXml);
        //    string base64 = string.Empty;

        //    var helper = new Helper();

        //    using (var stream = new MemoryStream(plainTextBytes))
        //    {
        //        var serializer = new XmlSerializer(typeof(Api.Invoice.Models.Invoice21.InvoiceType));
        //        ubl = (Api.Invoice.Models.Invoice21.InvoiceType)(serializer.Deserialize(stream) as Api.Invoice.Models.Invoice21.InvoiceType);
        //    }

        //    using (var stream = new MemoryStream())
        //    {
        //        var writer = new XmlTextWriter(stream, Encoding.UTF8);
        //        var xmlSerializer = new XmlSerializer(typeof(Api.Invoice.Models.Invoice21.InvoiceType));
        //        xmlSerializer.Serialize(writer, ubl, XmlNamespaceHelper.InvoiceNamespaces);
        //        writer.Flush();
        //        stream.Seek((long)0, SeekOrigin.Begin);

        //        var zipFile = helper.ZipFileToByte(stream, ubl.UUID.Value.ToString() + ".xml");
        //        base64 = Convert.ToBase64String(zipFile);
        //    }

        //    var byteFile = Convert.FromBase64String(base64);
        //    var unZipFile = helper.UnzipFile(byteFile);

        //    CreateInvoiceModel model = new CreateInvoiceModel()
        //    {
        //        InvoiceZip = base64,
        //        // Status = (int)InvoiceStatus.Draft,  //Taslak fatura
        //        Status = (int)InvoiceStatus.Queued,  //Direkt gönderilmesi için
        //        CheckLocalReferenceId = false,
        //        Prefix = "",
        //        UseManualInvoiceId = true,
        //        TargetAlias = "urn:mail:defaulttest3pk@medyasoft.com.tr",
        //        // AppType = 1 // e-Fatura
        //        AppType = 2 // e-Arşiv
        //    };

        //    var data = await _outboxInvoiceClient.PostUBL(model);
        //    return Ok(data);
        //}

        // [HttpGet("getwithenvelopes/{id}")] //B980FFE7-A6F0-4072-AACE-119CCB40A483
        // public async Task<ActionResult<OutboxInvoiceModel>> GetWithEnvelopes(Guid id)
        // {
        //     return await _outboxInvoiceClient.GetWithEnvelopes(id);
        // }
    }
}
