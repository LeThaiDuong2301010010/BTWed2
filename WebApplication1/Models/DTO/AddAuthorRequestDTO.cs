using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTO
{
    public class AddAuthorRequestDTO
    {
        [Required(ErrorMessage = "Tên tác giả là bắt buộc")]
        [MinLength(3, ErrorMessage = "Tên tác giả phải có ít nhất 3 ký tự")]
        public string FullName { get; set; }
    }
}