using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;

namespace PdfEmailService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail(IFormFile file, [FromForm] string email)
        {
            if (file == null || string.IsNullOrEmpty(email))
            {
                return BadRequest("Faltan archivos o email");
            }

            // Configura el cliente de SMTP
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("gonzalo", "gonzalolimari@gmail.com"));
            message.To.Add(new MailboxAddress("gonzalo.limari.celis@cftmail.cl", email));
            message.Subject = "Archivo PDF";
            message.Body = new TextPart("plain")
            {
                Text = "Aquí está el archivo PDF que solicitaste."
            };

            // Agrega el archivo adjunto
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                var attachment = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(stream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = file.FileName
                };
                var multipart = new Multipart("mixed") { message.Body, attachment };
                message.Body = multipart;
            }

            // Configura el transporte SMTP
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("gonzalolimari@gmail.com", "zjha elnj slvw hrby");

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                catch (System.Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Error al enviar el correo: {ex.Message}");
                }
            }

            return Ok("Correo enviado con éxito");
        }
    }
}
