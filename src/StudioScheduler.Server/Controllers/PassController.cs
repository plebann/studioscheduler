using Microsoft.AspNetCore.Mvc;
using StudioScheduler.Core.Interfaces.Services;
using StudioScheduler.Core.Interfaces.Repositories;
using StudioScheduler.Core.Models;
using StudioScheduler.Core.Enums;
using StudioScheduler.Shared.Dtos;
using StudioScheduler.Core.Services;

namespace StudioScheduler.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PassController : ControllerBase
{
    private readonly IPassService _passService;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ILogger<PassController> _logger;

    public PassController(
        IPassService passService,
        IAttendanceRepository attendanceRepository,
        ILogger<PassController> logger)
    {
        _passService = passService;
        _attendanceRepository = attendanceRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all passes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PassDto>>> GetAllPasses()
    {
        try
        {
            _logger.LogInformation("Getting all passes");
            var passes = await _passService.GetAllAsync();
            var passDtos = new List<PassDto>();

            foreach (var pass in passes)
            {
                var attendances = await _attendanceRepository.GetByPassIdAsync(pass.Id);
                passDtos.Add(await MapToPassDtoAsync(pass, attendances));
            }

            _logger.LogInformation("Retrieved {Count} passes", passDtos.Count);
            return Ok(passDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all passes");
            return StatusCode(500, new { message = "An error occurred while retrieving passes" });
        }
    }

    /// <summary>
    /// Get pass by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PassDto>> GetPassById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting pass {PassId}", id);
            var pass = await _passService.GetByIdAsync(id);
            
            if (pass == null)
            {
                _logger.LogWarning("Pass {PassId} not found", id);
                return NotFound($"Pass with ID {id} not found");
            }

            var attendances = await _attendanceRepository.GetByPassIdAsync(pass.Id);
            var passDto = await MapToPassDtoAsync(pass, attendances);

            _logger.LogInformation("Successfully retrieved pass {PassId}", id);
            return Ok(passDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pass {PassId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the pass" });
        }
    }

    /// <summary>
    /// Get passes for a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<PassDto>>> GetPassesByUserId(Guid userId)
    {
        try
        {
            _logger.LogInformation("Getting passes for user {UserId}", userId);
            var passes = await _passService.GetByUserIdAsync(userId);
            var passDtos = new List<PassDto>();

            foreach (var pass in passes)
            {
                var attendances = await _attendanceRepository.GetByPassIdAsync(pass.Id);
                passDtos.Add(await MapToPassDtoAsync(pass, attendances));
            }

            _logger.LogInformation("Retrieved {Count} passes for user {UserId}", passDtos.Count, userId);
            return Ok(passDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving passes for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving user passes" });
        }
    }

    /// <summary>
    /// Get current active pass for a user
    /// </summary>
    [HttpGet("user/{userId}/current")]
    public async Task<ActionResult<PassDto>> GetCurrentActivePassForUser(Guid userId)
    {
        try
        {
            _logger.LogInformation("Getting current active pass for user {UserId}", userId);
            var pass = await _passService.GetCurrentActivePassForUserAsync(userId);
            
            if (pass == null)
            {
                _logger.LogInformation("No active pass found for user {UserId}", userId);
                return NotFound($"No active pass found for user {userId}");
            }

            var attendances = await _attendanceRepository.GetByPassIdAsync(pass.Id);
            var passDto = await MapToPassDtoAsync(pass, attendances);

            _logger.LogInformation("Retrieved current active pass {PassId} for user {UserId}", pass.Id, userId);
            return Ok(passDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current active pass for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving the current pass" });
        }
    }

    /// <summary>
    /// Get active passes
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<PassDto>>> GetActivePasses()
    {
        try
        {
            _logger.LogInformation("Getting all active passes");
            var passes = await _passService.GetActivePassesAsync();
            var passDtos = new List<PassDto>();

            foreach (var pass in passes)
            {
                var attendances = await _attendanceRepository.GetByPassIdAsync(pass.Id);
                passDtos.Add(await MapToPassDtoAsync(pass, attendances));
            }

            _logger.LogInformation("Retrieved {Count} active passes", passDtos.Count);
            return Ok(passDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active passes");
            return StatusCode(500, new { message = "An error occurred while retrieving active passes" });
        }
    }

    /// <summary>
    /// Get passes expiring in specified days
    /// </summary>
    [HttpGet("expiring")]
    public async Task<ActionResult<IEnumerable<PassDto>>> GetPassesExpiringInDays([FromQuery] int days = 7)
    {
        try
        {
            _logger.LogInformation("Getting passes expiring in {Days} days", days);
            var passes = await _passService.GetPassesExpiringInDaysAsync(days);
            var passDtos = new List<PassDto>();

            foreach (var pass in passes)
            {
                var attendances = await _attendanceRepository.GetByPassIdAsync(pass.Id);
                passDtos.Add(await MapToPassDtoAsync(pass, attendances));
            }

            _logger.LogInformation("Retrieved {Count} passes expiring in {Days} days", passDtos.Count, days);
            return Ok(passDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving passes expiring in {Days} days", days);
            return StatusCode(500, new { message = "An error occurred while retrieving expiring passes" });
        }
    }

    /// <summary>
    /// Create a new pass
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PassDto>> CreatePass([FromBody] CreatePassDto createPassDto)
    {
        try
        {
            _logger.LogInformation("Creating new pass for user {UserId} of type {Type}", 
                createPassDto.UserId, createPassDto.Type);

            if (!Enum.TryParse<PassType>(createPassDto.Type, out var passType))
            {
                return BadRequest($"Invalid pass type: {createPassDto.Type}");
            }

            var pass = new Pass
            {
                Id = Guid.NewGuid(),
                UserId = createPassDto.UserId,
                StartDate = createPassDto.StartDate,
                EndDate = createPassDto.EndDate,
                Type = passType,
                ClassesPerWeek = createPassDto.ClassesPerWeek,
                TotalClasses = createPassDto.TotalClasses,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdPass = await _passService.CreatePassAsync(pass);
            var attendances = await _attendanceRepository.GetByPassIdAsync(createdPass.Id);
            var passDto = await MapToPassDtoAsync(createdPass, attendances);

            _logger.LogInformation("Successfully created pass {PassId}", createdPass.Id);
            return CreatedAtAction(nameof(GetPassById), new { id = createdPass.Id }, passDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid pass creation request: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating pass");
            return StatusCode(500, new { message = "An error occurred while creating the pass" });
        }
    }

    /// <summary>
    /// Update an existing pass
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<PassDto>> UpdatePass(Guid id, [FromBody] UpdatePassDto updatePassDto)
    {
        try
        {
            _logger.LogInformation("Updating pass {PassId}", id);
            
            var existingPass = await _passService.GetByIdAsync(id);
            if (existingPass == null)
            {
                _logger.LogWarning("Pass {PassId} not found for update", id);
                return NotFound($"Pass with ID {id} not found");
            }

            existingPass.EndDate = updatePassDto.EndDate;
            existingPass.IsActive = updatePassDto.IsActive;
            
            if (updatePassDto.AdditionalClasses.HasValue)
            {
                existingPass.TotalClasses += updatePassDto.AdditionalClasses.Value;
            }

            var updatedPass = await _passService.UpdatePassAsync(existingPass);
            var attendances = await _attendanceRepository.GetByPassIdAsync(updatedPass.Id);
            var passDto = await MapToPassDtoAsync(updatedPass, attendances);

            _logger.LogInformation("Successfully updated pass {PassId}", id);
            return Ok(passDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid pass update request: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating pass {PassId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the pass" });
        }
    }

    /// <summary>
    /// Delete a pass
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePass(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting pass {PassId}", id);
            
            var existingPass = await _passService.GetByIdAsync(id);
            if (existingPass == null)
            {
                _logger.LogWarning("Pass {PassId} not found for deletion", id);
                return NotFound($"Pass with ID {id} not found");
            }

            await _passService.DeletePassAsync(id);
            
            _logger.LogInformation("Successfully deleted pass {PassId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting pass {PassId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the pass" });
        }
    }

    /// <summary>
    /// Get available pass types with SalsaMe pricing
    /// </summary>
    [HttpGet("types")]
    public ActionResult<IEnumerable<PassTypeDto>> GetAvailablePassTypes()
    {
        try
        {
            _logger.LogInformation("Getting available pass types");
            
            var passTypes = new List<PassTypeDto>
            {
                new()
                {
                    Type = "SingleClass",
                    Description = "Single class pass - valid for one class only",
                    Price = 25.00m,
                    ClassesPerWeek = 1,
                    TotalClasses = 1,
                    ValidityPeriodInDays = 1,
                    IsFlexiPass = false,
                    AllowsMakeUpClasses = false,
                    WeekDefinition = "Monday to Sunday"
                },
                new()
                {
                    Type = "Flexi4Classes",
                    Description = "FLEXI 4 Classes - 1 class per week, valid for 28 days",
                    Price = 85.00m,
                    ClassesPerWeek = 1,
                    TotalClasses = 4,
                    ValidityPeriodInDays = 28,
                    IsFlexiPass = true,
                    AllowsMakeUpClasses = true,
                    WeekDefinition = "Monday to Sunday"
                },
                new()
                {
                    Type = "Flexi8Classes",
                    Description = "FLEXI 8 Classes - 2 classes per week, valid for 28 days",
                    Price = 160.00m,
                    ClassesPerWeek = 2,
                    TotalClasses = 8,
                    ValidityPeriodInDays = 28,
                    IsFlexiPass = true,
                    AllowsMakeUpClasses = true,
                    WeekDefinition = "Monday to Sunday"
                },
                new()
                {
                    Type = "Monthly1Course",
                    Description = "Monthly 1 Course - 1 class per week, valid for 28 days",
                    Price = 70.00m,
                    ClassesPerWeek = 1,
                    TotalClasses = 4,
                    ValidityPeriodInDays = 28,
                    IsFlexiPass = false,
                    AllowsMakeUpClasses = true,
                    WeekDefinition = "Monday to Sunday"
                },
                new()
                {
                    Type = "Monthly2Courses",
                    Description = "Monthly 2 Courses - 2 classes per week, valid for 28 days",
                    Price = 130.00m,
                    ClassesPerWeek = 2,
                    TotalClasses = 8,
                    ValidityPeriodInDays = 28,
                    IsFlexiPass = false,
                    AllowsMakeUpClasses = true,
                    WeekDefinition = "Monday to Sunday"
                },
                new()
                {
                    Type = "Monthly3Courses",
                    Description = "Monthly 3 Courses - 3 classes per week, valid for 28 days",
                    Price = 185.00m,
                    ClassesPerWeek = 3,
                    TotalClasses = 12,
                    ValidityPeriodInDays = 28,
                    IsFlexiPass = false,
                    AllowsMakeUpClasses = true,
                    WeekDefinition = "Monday to Sunday"
                },
                new()
                {
                    Type = "Monthly4Courses",
                    Description = "Monthly 4 Courses - 4 classes per week, valid for 28 days",
                    Price = 230.00m,
                    ClassesPerWeek = 4,
                    TotalClasses = 16,
                    ValidityPeriodInDays = 28,
                    IsFlexiPass = false,
                    AllowsMakeUpClasses = true,
                    WeekDefinition = "Monday to Sunday"
                },
                new()
                {
                    Type = "Monthly5Courses",
                    Description = "Monthly 5 Courses - 5 classes per week, valid for 28 days",
                    Price = 275.00m,
                    ClassesPerWeek = 5,
                    TotalClasses = 20,
                    ValidityPeriodInDays = 28,
                    IsFlexiPass = false,
                    AllowsMakeUpClasses = true,
                    WeekDefinition = "Monday to Sunday"
                },
                new()
                {
                    Type = "FullPass",
                    Description = "Full Pass - Unlimited classes, valid for 28 days",
                    Price = 350.00m,
                    ClassesPerWeek = null,
                    TotalClasses = null,
                    ValidityPeriodInDays = 28,
                    IsFlexiPass = false,
                    AllowsMakeUpClasses = true,
                    WeekDefinition = "Monday to Sunday"
                }
            };

            _logger.LogInformation("Retrieved {Count} pass types", passTypes.Count);
            return Ok(passTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pass types");
            return StatusCode(500, new { message = "An error occurred while retrieving pass types" });
        }
    }

    /// <summary>
    /// Purchase a new pass with class selection
    /// </summary>
    [HttpPost("purchase")]
    public async Task<ActionResult<PassPurchaseResponseDto>> PurchasePass([FromBody] BuyPassRequestDto request)
    {
        try
        {
            _logger.LogInformation("Processing pass purchase for student {StudentId} of type {PassType}", 
                request.StudentId, request.PassType);

            var createdPass = await _passService.PurchasePassAsync(
                request.StudentId, 
                request.PassType, 
                request.StartDate, 
                request.SelectedScheduleIds);

            _logger.LogInformation("Successfully purchased pass {PassId} for student {StudentId}", 
                createdPass.Id, request.StudentId);

            return Ok(new PassPurchaseResponseDto
            {
                Success = true,
                PassId = createdPass.Id,
                ErrorMessage = null
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid pass purchase request: {Message}", ex.Message);
            return BadRequest(new PassPurchaseResponseDto
            {
                Success = false,
                PassId = null,
                ErrorMessage = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purchasing pass for student {StudentId}", request.StudentId);
            return StatusCode(500, new PassPurchaseResponseDto
            {
                Success = false,
                PassId = null,
                ErrorMessage = "An error occurred while purchasing the pass"
            });
        }
    }

    /// <summary>
    /// Get available monthly pass types for purchase
    /// </summary>
    [HttpGet("types/monthly")]
    public ActionResult<IEnumerable<object>> GetAvailableMonthlyPassTypes()
    {
        try
        {
            _logger.LogInformation("Getting available monthly pass types for purchase");
            
            var monthlyPassTypes = PassConfigurationService.GetAvailableMonthlyPasses()
                .Select(passType => new
                {
                    Type = passType.ToString(),
                    DisplayName = PassConfigurationService.GetPassDisplayName(passType),
                    ClassesPerWeek = PassConfigurationService.GetClassesPerWeek(passType),
                    TotalClasses = PassConfigurationService.GetTotalClasses(passType)
                })
                .ToList();

            _logger.LogInformation("Retrieved {Count} monthly pass types", monthlyPassTypes.Count);
            return Ok(monthlyPassTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly pass types");
            return StatusCode(500, new { message = "An error occurred while retrieving monthly pass types" });
        }
    }

    /// <summary>
    /// Get pass usage statistics
    /// </summary>
    [HttpGet("{id}/stats")]
    public async Task<ActionResult<PassUsageStatsDto>> GetPassUsageStats(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting usage stats for pass {PassId}", id);
            
            var pass = await _passService.GetByIdAsync(id);
            if (pass == null)
            {
                _logger.LogWarning("Pass {PassId} not found for stats", id);
                return NotFound($"Pass with ID {id} not found");
            }

            var attendances = await _attendanceRepository.GetByPassIdAsync(id);
            var usageStats = CreatePassUsageStats(pass, attendances);

            _logger.LogInformation("Successfully retrieved usage stats for pass {PassId}", id);
            return Ok(usageStats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving usage stats for pass {PassId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving pass statistics" });
        }
    }

    // Private helper methods
    private Task<PassDto> MapToPassDtoAsync(Pass pass, IEnumerable<Attendance> attendances)
    {
        var attendanceList = attendances.ToList();
        
        var passDto = new PassDto
        {
            Id = pass.Id,
            UserId = pass.UserId,
            UserName = pass.User?.FirstName + " " + pass.User?.LastName ?? "Unknown User",
            StartDate = pass.StartDate,
            EndDate = pass.EndDate,
            Type = pass.Type.ToString(),
            ClassesPerWeek = pass.ClassesPerWeek,
            TotalClasses = pass.TotalClasses,
            RemainingClasses = pass.CalculateRemainingClasses(attendanceList),
            UsedClasses = pass.GetUsedClassesCount(attendanceList),
            ClassesUsedThisWeek = pass.GetClassesUsedThisWeek(attendanceList),
            CompleteWeeksRemaining = pass.GetCompleteWeeksRemaining(),
            PassStatus = pass.GetPassStatus(attendanceList).ToString(),
            AllowsMakeUpClasses = pass.AllowsMakeUpClasses(),
            IsActive = pass.IsActive,
            CreatedAt = pass.CreatedAt,
            UpdatedAt = pass.UpdatedAt
        };
        
        return Task.FromResult(passDto);
    }

    private static PassUsageStatsDto CreatePassUsageStats(Pass pass, IEnumerable<Attendance> attendances)
    {
        var attendanceList = attendances.Where(a => a.WasPresent).ToList();
        var weeklyBreakdown = new List<WeeklyUsageDto>();
        
        // Create weekly breakdown from pass start date
        var currentWeekStart = GetWeekStart(pass.StartDate);
        var passEndDate = pass.EndDate;
        var today = DateTime.Today;
        
        while (currentWeekStart <= passEndDate)
        {
            var weekEnd = currentWeekStart.AddDays(6);
            var weekAttendances = attendanceList
                .Where(a => a.ClassDate >= currentWeekStart && a.ClassDate <= weekEnd)
                .ToList();
            
            var maxAllowed = pass.Type switch
            {
                PassType.Flexi4Classes => 1,
                PassType.Flexi8Classes => 2,
                PassType.FullPass => int.MaxValue,
                _ => pass.ClassesPerWeek
            };
            
            weeklyBreakdown.Add(new WeeklyUsageDto
            {
                WeekStartDate = currentWeekStart,
                ClassesAttended = weekAttendances.Count,
                MaxAllowedForWeek = maxAllowed,
                IsCurrentWeek = currentWeekStart <= today && today <= weekEnd
            });
            
            currentWeekStart = currentWeekStart.AddDays(7);
        }
        
        return new PassUsageStatsDto
        {
            PassId = pass.Id,
            TotalClassesAttended = attendanceList.Count,
            ClassesThisWeek = pass.GetClassesUsedThisWeek(attendanceList),
            WeeksUsed = weeklyBreakdown.Count(w => w.ClassesAttended > 0),
            WeeksRemaining = pass.GetCompleteWeeksRemaining(),
            LastAttendanceDate = attendanceList.OrderByDescending(a => a.ClassDate).FirstOrDefault()?.ClassDate,
            WeeklyBreakdown = weeklyBreakdown
        };
    }
    
    private static DateTime GetWeekStart(DateTime date)
    {
        // Week starts on Monday (SalsaMe standard)
        var daysFromMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
        return date.Date.AddDays(-daysFromMonday);
    }
}
