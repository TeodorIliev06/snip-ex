namespace SnipEx.Web.ViewModels.DTOs
{
    using Microsoft.AspNetCore.Http;

    public class FileUploadRequestDto
    {
        public IFormFile File { get; set; }
    }

}
