using AutoMapper;
using ExampleAPI.AppDataContext;
using ExampleAPI.Contracts;
using ExampleAPI.Interface;
using ExampleAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleAPI.Services
{
    public class TodoServices : ITodoServices
    {
        private readonly TodoDbContext _context;
        private readonly ILogger<TodoServices> _logger;
        private readonly IMapper _mapper;

        public TodoServices(TodoDbContext context, ILogger<TodoServices> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task CreateTodoAsync(CreateTodoRequest request)
        {
            try
            {
                var todo = _mapper.Map<Todo>(request);
                
                // Ensure that DueDate is in UTC if needed
                if (request.DueDate.Kind == DateTimeKind.Unspecified)
                {
                    request.DueDate = DateTime.SpecifyKind(request.DueDate, DateTimeKind.Utc);
                }
                else
                {
                    request.DueDate = request.DueDate.ToUniversalTime();
                }

                // Set CreatedAt and UpdatedAt to UTC
                todo.CreatedAt = DateTime.UtcNow;
                todo.UpdatedAt = DateTime.UtcNow;

                _context.Todos.Add(todo);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the Todo item.");
                throw new Exception("An error occurred while creating the Todo item.");
            }
        }

        public Task DeleteTodoAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        // Get all TODO Items from the database 
        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            var todo= await _context.Todos.ToListAsync() ?? throw new Exception(" No Todo items found");
            return todo;

        }

        public async Task<Todo> GetByIdAsync(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id) ?? throw new KeyNotFoundException($"No Todo item with Id {id} found.");
            return todo;
        }

        public async Task UpdateTodoAsync(Guid id, UpdateTodoRequest request)
        {
            try
            {
                var todo = await _context.Todos.FindAsync(id) ?? throw new Exception($"Todo item with id {id} not found.");
                if (request.Title != null)
                {
                    todo.Title = request.Title;
                }

                if (request.Description != null)
                {
                    todo.Description = request.Description;
                }

                if (request.IsComplete != null)
                {
                    todo.IsComplete = request.IsComplete.Value;
                }

                if (request.DueDate != null)
                {
                    todo.DueDate = request.DueDate.Value;
                }

                if (request.Priority != null)
                {
                    todo.Priority = request.Priority.Value;
                }

                todo.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the todo item with id {id}.");
                throw;
            }
        }
    }
}