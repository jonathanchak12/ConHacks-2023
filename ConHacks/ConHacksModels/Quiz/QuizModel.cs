using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ConHacksModels.Quiz;

public class Quiz
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public Guid Id { get; set; }
    [Required] public string Course { get; set; }
    [Required] public string Unit { get; set; }
    [Required] public string Question { get; set; }
    [Required] public string Solution { get; set; }
}

public class NewQuiz
{
    [Required] public string Course { get; set; }
    [Required] public string Unit { get; set; }
    [Required] public string Question { get; set; }
    [Required] public string Solution { get; set; }
}