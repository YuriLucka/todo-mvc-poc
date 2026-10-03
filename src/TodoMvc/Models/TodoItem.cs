using System.ComponentModel.DataAnnotations;

namespace TodoMvc.Models;

public class TodoItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe um título.")]
    [StringLength(200, ErrorMessage = "Máximo de 200 caracteres.")]
    [Display(Name = "Título")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Concluída")]
    public bool IsDone { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
