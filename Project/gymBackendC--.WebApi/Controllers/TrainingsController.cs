using gymBackendC__.WebApi.Models.DTO;
using gymBackendC__.WebApi.Services;

using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace gymBackendC__.WebApi.Controllers;

[ApiController]
[Route("trainings")]
[Produces("application/json")]
public class TrainingsController(
    ITrainingsService trainingsService, 
    IValidator<TrainingsRequestModel> validator
) : ControllerBase
{
    [Authorize(Policy = "CanRead")]
    [HttpGet(Name = "GetTrainings")]
    [ProducesResponseType<TrainingsSearchModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetAsync([FromQuery] DateTime? searchDate, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var trainings = await trainingsService.GetAsync(page, pageSize, searchDate);
        return TypedResults.Ok(trainings);
    }
    
    [Authorize(Policy = "CanRead")]
    [HttpGet("{id:int}", Name = "GetTrainingById")]
    [ProducesResponseType<TrainingsResponseModel>(StatusCodes.Status200OK)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetByIdAsync([FromRoute] int id)
    {
        var training = await trainingsService.GetByIdAsync(id);
        return TypedResults.Ok(training);
    }
    
    [Authorize(Policy = "CanCreate")]
    [HttpPost(Name = "CreateTraining")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<TrainingsResponseModel>(StatusCodes.Status201Created)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status409Conflict)]
    public async Task<IResult> PostAsync([FromBody] TrainingsRequestModel training)
    {
        var validation = await validator.ValidateAsync(training);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var result = await trainingsService.CreateAsync(training);
        return TypedResults.CreatedAtRoute(
            routeName: "GetTrainingById",
            routeValues: new { id = result.Id },
            value: result
        );
    }
    
    [Authorize(Policy = "CanUpdate")]
    [HttpPut("{id:int}", Name = "UpdateTraining")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> PutAsync([FromRoute] int id, [FromBody] TrainingsRequestModel training)
    {
        var validation = await validator.ValidateAsync(training);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }
        
        await trainingsService.UpdateAsync(id, training);
        return TypedResults.NoContent();
    }

    [Authorize(Policy = "CanDelete")]
    [HttpDelete("{id:int}", Name = "DeleteTraining")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ExceptionModel>(StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteAsync([FromRoute] int id)
    {
        await trainingsService.DeleteAsync(id);
        return TypedResults.NoContent();
    }
}
