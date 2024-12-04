﻿namespace WebApp.Entities;

public class CarExerciseParameters
{
    public int Interval { get; set; }
    public int Speed { get; set; }
    public int Seconds { get; set; }
    public int CarExerciseInTrainingCardioExerciseId  { get; set; }
    public int CarExerciseInTrainingTrainingId { get; set; }
    public CarExerciseInTraining CarExerciseInTraining { get; set; }
}