using Microsoft.AspNetCore.Mvc;
using LoveForTennis.Application.DTOs;
using LoveForTennis.Application.Interfaces;

namespace LoveForTennis.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DummyController : ControllerBase
{
    private readonly IDummyService _dummyService;

    public DummyController(IDummyService dummyService)
    {
        _dummyService = dummyService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DummyDto>>> GetAll()
    {
        var dummies = await _dummyService.GetAllDummiesAsync();
        return Ok(dummies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DummyDto>> GetById(int id)
    {
        var dummy = await _dummyService.GetDummyByIdAsync(id);
        if (dummy == null)
        {
            return NotFound();
        }
        return Ok(dummy);
    }

    [HttpPost]
    public async Task<ActionResult<DummyDto>> Create(DummyDto dummyDto)
    {
        var createdDummy = await _dummyService.CreateDummyAsync(dummyDto);
        return CreatedAtAction(nameof(GetById), new { id = createdDummy.Id }, createdDummy);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DummyDto>> Update(int id, DummyDto dummyDto)
    {
        if (id != dummyDto.Id)
        {
            return BadRequest();
        }

        var updatedDummy = await _dummyService.UpdateDummyAsync(dummyDto);
        return Ok(updatedDummy);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _dummyService.DeleteDummyAsync(id);
        return NoContent();
    }
}