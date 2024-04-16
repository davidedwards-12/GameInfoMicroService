using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Micro
{
    [ApiController]
    [Route("[controller]")]
    public class MicroController : ControllerBase
    {
        private static readonly List<GameInfo> TheInfo = new List<GameInfo>
        {
            new GameInfo { 
                //Id = 1,
                Title = "Snake",
                //Content = "~/js/snake.js",
                Author = "Hillary clinton ",
                DateAdded = "01/01/1942",
                Description = "Look at me im a SNEEEEK",
                HowTo = "Just snek around",
                //Thumbnail = "/images/snake.jpg" //640x360 resolution
            },
            new GameInfo { 
                //Id = 2,
                Title = "Tetris",
                //Content = "~/js/tetris.js",
                Author = "Steve from minecraft",
                DateAdded = "09/09/1541",
                Description = "Block Block Block",
                HowTo = "Put Blocks Down",
                //Thumbnail = "/images/tetris.jpg"
            },
            new GameInfo { 
                //Id = 3,
                Title = "Pong",
                //Content = "~/js/pong.js",
                Author = "Forest Gump",
                DateAdded = "07/04/1742",
                Description = "RUN FOREST RUN!",
                HowTo = "Hit the back back",
                //Thumbnail = "/images/pong.jpg"
            },

        };

        private static readonly string aboutUs = "This website is the result of multiple semesters of Software Engineering Students learning how to build a project with good engineering habits. This webpage was added to be a vertical slice while adding a microservice to this website, and this text is loaded from a microservice.";

        private static readonly string snakeLogic = "/* \r\n * Snake\r\n * \r\n * Base game created by straker on GitHub\r\n *  https://gist.github.com/straker/81b59eecf70da93af396f963596dfdc5\r\n * \r\n * Implemented by Kyle Wittman\r\n * \r\n * Fall 2023, ETSU\r\n * \r\n */\r\n\r\nvar canvas = document.getElementById('game');\r\nvar context = canvas.getContext('2d');\r\n\r\n// the canvas width & height, snake x & y, and the apple x & y, all need to be a multiples of the grid size in order for collision detection to work\r\n// (e.g. 16 * 25 = 400)\r\nvar grid = 16;\r\nvar count = 0;\r\n\r\n// Structure for holding data for the snake\r\nvar snake = {\r\n  x: 160,\r\n  y: 160,\r\n\r\n  // snake velocity. moves one grid length every frame in either the x or y direction\r\n  dx: grid,\r\n  dy: 0,\r\n\r\n  // keep track of all grids the snake body occupies\r\n  cells: [],\r\n\r\n  // length of the snake. grows when eating an apple\r\n  maxCells: 4\r\n};\r\n\r\n// Structure for holding data for the current apple\r\nvar apple = {\r\n  x: 320,\r\n  y: 320\r\n};\r\n\r\n// get random whole numbers in a specific range\r\n// see https://stackoverflow.com/a/1527820/2124254\r\nfunction getRandomInt(min, max) {\r\n  return Math.floor(Math.random() * (max - min)) + min;\r\n}\r\n\r\n// game loop\r\nfunction loop() {\r\n  requestAnimationFrame(loop);\r\n\r\n  // slow game loop to 15 fps instead of 60 (60/15 = 4)\r\n  if (++count < 4) {\r\n    return;\r\n  }\r\n\r\n  count = 0; // Reset the FPS counter\r\n  context.clearRect(0,0,canvas.width,canvas.height);\r\n\r\n  // move snake by it's velocity\r\n  snake.x += snake.dx;\r\n  snake.y += snake.dy;\r\n\r\n  // wrap snake position horizontally on edge of screen\r\n  if (snake.x < 0) { // Left side of the screen\r\n    snake.x = canvas.width - grid;\r\n  }\r\n  else if (snake.x >= canvas.width) { // Right side of the screen\r\n    snake.x = 0;\r\n  }\r\n\r\n  // wrap snake position vertically on edge of screen\r\n  if (snake.y < 0) { // Top of the screen\r\n    snake.y = canvas.height - grid;\r\n  }\r\n  else if (snake.y >= canvas.height) { // Bottom of the screen\r\n    snake.y = 0;\r\n  }\r\n\r\n  // keep track of where snake has been. front of the array is always the head\r\n  snake.cells.unshift({x: snake.x, y: snake.y});\r\n\r\n  // remove cells as we move away from them\r\n  if (snake.cells.length > snake.maxCells) {\r\n    snake.cells.pop();\r\n  }\r\n\r\n  // draw apple\r\n  context.fillStyle = 'red';\r\n  context.fillRect(apple.x, apple.y, grid-1, grid-1);\r\n\r\n  // draw snake one cell at a time\r\n  context.fillStyle = 'green';\r\n  snake.cells.forEach(function(cell, index) {\r\n\r\n    // drawing 1 px smaller than the grid creates a grid effect in the snake body so you can see how long it is\r\n    context.fillRect(cell.x, cell.y, grid-1, grid-1);\r\n\r\n    // snake ate apple\r\n    if (cell.x === apple.x && cell.y === apple.y) {\r\n      snake.maxCells++;\r\n\r\n      // canvas is 400x400 which is 25x25 grids\r\n      apple.x = getRandomInt(0, 25) * grid;\r\n      apple.y = getRandomInt(0, 25) * grid;\r\n    }\r\n\r\n    // check collision with all cells after this one (modified bubble sort)\r\n    for (var i = index + 1; i < snake.cells.length; i++) {\r\n\r\n      // snake occupies same space as a body part. reset game\r\n      if (cell.x === snake.cells[i].x && cell.y === snake.cells[i].y) {\r\n        snake.x = 160;\r\n        snake.y = 160;\r\n        snake.cells = [];\r\n        snake.maxCells = 4;\r\n        snake.dx = grid;\r\n        snake.dy = 0;\r\n\r\n        apple.x = getRandomInt(0, 25) * grid;\r\n        apple.y = getRandomInt(0, 25) * grid;\r\n      }\r\n    }\r\n  });\r\n}\r\n\r\n// listen to keyboard events to move the snake\r\ndocument.addEventListener('keydown', function(e) {\r\n  // prevent snake from backtracking on itself by checking that it's\r\n  // not already moving on the same axis (pressing left while moving\r\n  // left won't do anything, and pressing right while moving left\r\n  // shouldn't let you collide with your own body)\r\n\r\n  // left arrow key\r\n  if (e.which === 37 && snake.dx === 0) {\r\n    snake.dx = -grid;\r\n    snake.dy = 0;\r\n  }\r\n  // up arrow key\r\n  else if (e.which === 38 && snake.dy === 0) {\r\n    snake.dy = -grid;\r\n    snake.dx = 0;\r\n  }\r\n  // right arrow key\r\n  else if (e.which === 39 && snake.dx === 0) {\r\n    snake.dx = grid;\r\n    snake.dy = 0;\r\n  }\r\n  // down arrow key\r\n  else if (e.which === 40 && snake.dy === 0) {\r\n    snake.dy = grid;\r\n    snake.dx = 0;\r\n  }\r\n});\r\n\r\n// start the game\r\nrequestAnimationFrame(loop);";

        private readonly ILogger<MicroController> _logger;

        public MicroController(ILogger<MicroController> logger)
        {
            _logger = logger;
        }

        [HttpGet("games")]
        public IEnumerable<GameInfo> Get()
        {
            return TheInfo;
        }

        [HttpGet("aboutus")]
        public string GetAboutUs()
        {
            return aboutUs;
        }

        [HttpGet("snake")]
        public string GetSnakeLogic()
        {
            return snakeLogic;
        }
    }
}