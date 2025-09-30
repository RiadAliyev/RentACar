using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarFeatureDto;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CarFeatureController : ControllerBase
{
    private readonly ICarFeatureService _carFeatureService;

    public CarFeatureController(ICarFeatureService carFeatureService)
    {
        _carFeatureService = carFeatureService;
    }



    [HttpGet]    
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _carFeatureService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }



    [HttpGet("{id:guid}")]   
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _carFeatureService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }



    [HttpPost]
    [Authorize(Policy = Permissions.CarFeature.Create)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] CarFeatureCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carFeatureService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }



    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permissions.CarFeature.Update)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CarFeatureUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carFeatureService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Permissions.CarFeature.Delete)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _carFeatureService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }
}
