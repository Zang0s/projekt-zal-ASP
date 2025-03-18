using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models.Movies;

namespace WebApp.Controllers
{
    public class ActorsController : Controller
    {
        private readonly MoviesDbContext _context;

        public ActorsController(MoviesDbContext context)
        {
            _context = context;
        }

        // GET: Actors
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var actorIds = await _context.MovieCasts
                .Select(mc => mc.PersonId)
                .Distinct()
                .ToListAsync();
            
            var totalActors = await _context.People
                .Where(a => actorIds.Contains(a.PersonId))
                .CountAsync();
            
            var actors = await _context.People
                .Where(a => actorIds.Contains(a.PersonId)) 
                .OrderBy(a => a.PersonName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalActors / pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;
            
            var actorViewModels = actors.Select(a => new ActorViewModel
            {
                PersonId = a.PersonId,
                PersonName = a.PersonName,
                MovieCount = _context.MovieCasts.Count(mc => mc.PersonId == a.PersonId),
                Movies = _context.MovieCasts
                    .Where(mc => mc.PersonId == a.PersonId)
                    .OrderByDescending(mc => mc.Movie.Popularity)
                    .Select(mc => new MovieRoleViewModel
                    {
                        Title = mc.Movie.Title,
                        CharacterName = mc.CharacterName
                    })
                    .ToList()
                
            }).ToList();

            return View(actorViewModels);
        }
        
        public async Task<IActionResult> Movies(int id)
        {
            var actor = await _context.People.FirstOrDefaultAsync(p => p.PersonId == id);
            if (actor == null)
            {
                return NotFound();
            }

            ViewBag.ActorId = id;
            ViewBag.ActorName = actor.PersonName;
            
            var movies = await _context.MovieCasts
                .Where(mc => mc.PersonId == id)
                .OrderByDescending(mc => mc.Movie.Popularity)
                .Select(mc => new MovieViewModel
                {
                    Title = mc.Movie.Title,
                    Budget = mc.Movie.Budget,
                    Popularity = mc.Movie.Popularity,
                    Homepage = mc.Movie.Homepage
                })
                .ToListAsync();

            return View(movies);
        }

        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddMovie(int id)
        {
            var actor = await _context.People.FirstOrDefaultAsync(p => p.PersonId == id);

            if (actor == null)
            {
                return NotFound();
            }

            ViewBag.ActorId = id;
            ViewBag.ActorName = actor.PersonName;

            var movies = await _context.Movies.ToListAsync();
            ViewBag.Movies = new SelectList(movies, "MovieId", "Title");

            return View();
        }



        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovie(int actorId, int movieId, string characterName)
        {
            if (actorId == 0 || movieId == 0 || string.IsNullOrEmpty(characterName))
            {
                return BadRequest("Invalid data provided.");
            }
            
            var actorExists = await _context.People.AnyAsync(p => p.PersonId == actorId);
            if (!actorExists)
            {
                return NotFound("Actor not found.");
            }
            
            var movieExists = await _context.Movies.AnyAsync(m => m.MovieId == movieId);
            if (!movieExists)
            {
                return NotFound("Movie not found.");
            }
            
            var sql = "INSERT INTO movie_cast (person_id, movie_id, character_name) VALUES (@PersonId, @MovieId, @CharacterName)";
            await _context.Database.ExecuteSqlRawAsync(sql,
                new[]
                {
                    new Microsoft.Data.Sqlite.SqliteParameter("@PersonId", actorId),
                    new Microsoft.Data.Sqlite.SqliteParameter("@MovieId", movieId),
                    new Microsoft.Data.Sqlite.SqliteParameter("@CharacterName", characterName)
                });

            return RedirectToAction("Movies", new { id = actorId });
        }





    }

    public class ActorViewModel
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int MovieCount { get; set; }
        public List<MovieRoleViewModel> Movies { get; set; }
    }

    public class MovieRoleViewModel
    {
        public string Title { get; set; }
        public string CharacterName { get; set; }
    }
    
    public class MovieViewModel
    {
        public string Title { get; set; }
        public int? Budget { get; set; }
        public double? Popularity { get; set; }
        public string Homepage { get; set; }
    }

}
