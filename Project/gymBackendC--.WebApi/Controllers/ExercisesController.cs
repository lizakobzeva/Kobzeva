using gymBackendC__.WebApi.Models.DTO;
using gymBackendC__.WebApi.Services;

using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace gymBackendC__.WebApi.Controllers;

[ApiController]
[Route("exercises")]
[Produces("application/json")]
public class ExercisesController(
    IExercisesService exercisesService, 
    IValidator<ExercisesRequestModel> validator
) : ControllerBase
{
    [Authorize(Policy = "CanRead")]
    [HttpGet(Name = "GetExercises")]
    [ProducesResponseType<List<ExercisesResponseModel>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetAsync([FromQuery] int trainingId)
    {
        var exercises = await exercisesService.GetAsync(trainingId);
        return TypedResults.Ok(exercises);
    }
    
    [Authorize(Policy = "CanRead")]
    [HttpGet("{id:int}", Name = "GetExerciseById")]
    [ProducesResponseType<ExercisesResponseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetByIdAsync([FromRoute] int id)
    {
        var exercise = await exercisesService.GetByIdAsync(id);
        return TypedResults.Ok(exercise);
    }
    
    [Authorize(Policy = "CanRead")]
    [HttpGet("stats", Name = "GetExercisesStats")]
    [ProducesResponseType<ExercisesStatsModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetStatsAsync()
    {
        var exercisesStats = await exercisesService.GetStatsAsync();
        return TypedResults.Ok(exercisesStats);
    }
    
    [Authorize(Policy = "CanCreate")]
    [HttpPost(Name = "CreateExercise")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<ExercisesResponseModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status409Conflict)]
    public async Task<IResult> PostAsync([FromBody] ExercisesRequestModel exercise)
    {
        var validation = await validator.ValidateAsync(exercise);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var result = await exercisesService.CreateAsync(exercise);
        return TypedResults.CreatedAtRoute(
            routeName: "GetExerciseById",
            routeValues: new { id = result.Id },
            value: result
        );
    }
    
    [Authorize(Policy = "CanUpdate")]
    [HttpPut("{id:int}", Name = "UpdateExercise")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> PutAsync([FromRoute] int id, [FromBody] ExercisesRequestModel exercise)
    {
        var validation = await validator.ValidateAsync(exercise);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }
        
        await exercisesService.UpdateAsync(id, exercise);
        return TypedResults.NoContent();
    }

    [Authorize(Policy = "CanDelete")]
    [HttpDelete("{id:int}", Name = "DeleteExercise")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteAsync([FromRoute] int id)
    {
        await exercisesService.DeleteAsync(id);
        return TypedResults.NoContent();
    }
}
