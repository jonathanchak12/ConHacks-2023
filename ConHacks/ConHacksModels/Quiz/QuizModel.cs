using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ConHacksModels.Quiz;

public class NewQuiz
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] Guid id { get; set; }
    [Required] public Guid UserId { get; set; }
    [ForeignKey("UserId")] public IdentityUser? User;
    [Required] public Guid CourseId { get; set; }
    [Required] public Guid UnitId { get; set; }
    [Required] public string Question { get; set; }
    [Required] public string Solution { get; set; }
}