using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Application.Abstracts.Services;
using RentACar.Application.DTOs.CarDtos;
using RentACar.Application.DTOs.CarImageDtos;
using RentACar.Application.Shared;

namespace RentACar.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    /// Bütün maşınları gətirir
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await _carService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    
    /// Maşını Id ilə gətirir   
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _carService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }


    /// Filtrlənmiş maşın siyahısı gətirir
    [HttpGet("filter")]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetByFilter([FromForm] CarFilterDto filter)
    {
        var response = await _carService.GetByFilterAsync(filter);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpPost]
    [Authorize(Policy = Permissions.Car.Create)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromForm] CarCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }
  


    [HttpPut("{id:guid}")]
    [Authorize(Policy = Permissions.Booking.Update)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CarUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(BaseResponse<string>.FailResponse("Validation error", HttpStatusCode.BadRequest));

        var response = await _carService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }


    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Permissions.Booking.Delete)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _carService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("{carId:guid}/images")]
    [Authorize(Policy = Permissions.Car.Update)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddImages(Guid carId, [FromForm] CarImageUploadDto dto)
    {
        if (dto.Files is null || dto.Files.Count == 0)
            return BadRequest(BaseResponse<string>.FailResponse("No files provided", System.Net.HttpStatusCode.BadRequest));

        var res = await _carService.AddImagesAsync(carId, dto.Files);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpPut("images/{imageId:guid}")]
    [Authorize(Policy = Permissions.Car.Update)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ReplaceImage(Guid imageId, [FromForm] CarImageReplaceDto dto)
    {
        if (dto.File is null)
            return BadRequest(BaseResponse<string>.FailResponse("File is required", System.Net.HttpStatusCode.BadRequest));

        var res = await _carService.ReplaceImageAsync(imageId, dto.File);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete("images/{imageId:guid}")]
    [Authorize(Policy = Permissions.Car.Delete)]
    public async Task<IActionResult> DeleteImage(Guid imageId)
    {
        var res = await _carService.DeleteImageAsync(imageId);
        return StatusCode((int)res.StatusCode, res);
    }

}
