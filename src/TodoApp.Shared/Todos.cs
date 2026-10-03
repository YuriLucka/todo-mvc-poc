using System.ComponentModel.DataAnnotations;

namespace TodoApp.Shared;

public record TodoDto(int Id, string Title, bool IsDone, DateTime CreatedAt);

public class TodoInput
{
    public const int TitleMaxLength = 200;

    [Required(ErrorMessage = "Informe um título.")]
    [StringLength(TitleMaxLength, ErrorMessage = "Máximo de 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    public bool IsDone { get; set; }
}
