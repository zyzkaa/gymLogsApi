﻿using Microsoft.AspNetCore.Mvc;
using WebApp.DTO;
using WebApp.DTO.Meals;
using WebApp.Entities;
using WebApp.Services.MealServices;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MealController : ControllerBase
{
    private readonly IMealService _mealService;

    public MealController(IMealService mealService)
    {
        _mealService = mealService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddMeal([FromBody] MealDto mealDto)
    {
        var meal = await _mealService.AddMeal(mealDto);
        return CreatedAtAction(nameof(GetMealById), new { id = meal.Id }, meal);
    }

    [HttpGet("get_all")]
    public async Task<IActionResult> GetMeals()
    {
        var meals = await _mealService.GetMeals();
        return Ok(meals);
    }

    [HttpGet("get_by_id/{id}")]
    public async Task<IActionResult> GetMealById(int id)
    {
        var meal = await _mealService.GetMealById(id);
        return Ok(meal);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteMealById(int id)
    {
        var meal = await _mealService.DeleteMealById(id);
        return Ok(meal);
    }
}