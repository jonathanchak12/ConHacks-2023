using ConHacks.Models;
using ConHacksModels.Quiz;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ConHacks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuizController : BaseController
{
    private readonly ApplicationDbContext _context;
    
    public QuizController(IConfiguration configuration, ApplicationDbContext context) : base(configuration)
    {
        _context = context;
    }

    [HttpPost]
    [Route("quiz/create")]
    public async Task<IActionResult> CreateQuiz([FromBody] NewQuiz quiz)
    {
        if (!quiz.Course.IsNullOrEmpty() && !quiz.Unit.IsNullOrEmpty() && !quiz.Question.IsNullOrEmpty() &&
            !quiz.Solution.IsNullOrEmpty())
        {
            var newQuiz = new Quiz();
            newQuiz.Course = quiz.Course;
            newQuiz.Unit = quiz.Unit;
            newQuiz.Question = quiz.Question;
            newQuiz.Solution = quiz.Solution;

            await _context.Quizzes.AddAsync(newQuiz);
            await _context.SaveChangesAsync();

            return Ok(newQuiz);
        }
        
        return BadRequest();
    }

    [HttpPost]
    [Route("quiz/fetchShuffled")]
    public IActionResult FetchShuffledQuizzes([FromHeader] string course, [FromHeader] string unit)
    {
        if (!course.IsNullOrEmpty() && !unit.IsNullOrEmpty())
        {
            // Fetch all quizzes that match the criteria
            var quizzes = _context.Quizzes.Where(x => x.Course == course && x.Unit == unit).ToList();

            // Check if there are any quizzes
            if (!quizzes.Any())
            {
                return NotFound("No quizzes found for the specified course and unit.");
            }

            // Shuffle the quizzes
            Random rnd = new Random();
            var shuffledQuizzes = quizzes.OrderBy(x => rnd.Next()).ToList();

            return Ok(shuffledQuizzes);
        }

        return BadRequest("Course or unit is null or empty.");
    }
}