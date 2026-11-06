using Godot;
using System;

public partial class Player : RigidBody2D
{
    [Export]
    public float maxFuel { get; set; } = 100;
    public float fuel;

    [Export]
    public int speed { get; set; } = 400;

    [Export]
    public int rotationTorque { get; set; } = 20;

    [ExportGroup("Fuel Loss")]
    [Export]
    public double idleLoss { get; set; } = 0.1;
    [Export]
    public double activeLoss { get; set; } = 1;

    [ExportCategory("References")]
    [Export]
    public ProgressBar fuelBar;

    [Export]
    public CpuParticles2D thrustParticles;

    [Export]
    public AudioStreamPlayer2D thrustSoundEffect;

    Vector2 thrust = Vector2.Zero;
    float rotationDir = 0;

    [Export]
    public RigidBody2D rigidBody2D;

    public void getInput(double delta)
    {
        thrust = Vector2.Zero;
        if (Input.IsActionPressed("forward"))
        {
            thrust = Transform.X * speed;
        }
        if (Input.IsActionPressed("back"))
        {
            thrust = Transform.X * -speed;
        }
        rotationDir = Input.GetAxis("left", "right") * rotationTorque;
        fuel = Math.Clamp(fuel, 0, maxFuel);
    }

    public override void _Ready()
    {
        fuel = maxFuel;
        fuelBar.MaxValue = maxFuel;
        fuelBar.MinValue = 0;
        fuelBar.Value = fuel;
    }

    public override void _PhysicsProcess(double delta)
    {
        fuelBar.Value = fuel;
        getInput(delta);
        if (fuel > 0)
        {
            rigidBody2D.ConstantForce = thrust;
            rigidBody2D.ConstantTorque = rotationDir;
        }
        else
        {
            rigidBody2D.ConstantForce = Vector2.Zero;
            rigidBody2D.ConstantTorque = 0;
        }
        if (ConstantForce != Vector2.Zero)
        {
            fuel -= (float)delta * (float)activeLoss;
            if (!thrustSoundEffect.Playing)
            {
                thrustSoundEffect.Play();
            }
            thrustParticles.Emitting = true;
        }
        else
        {
            fuel -= (float)delta * (float)idleLoss;
            thrustSoundEffect.Stop();
            thrustParticles.Emitting = false;
        }
    }
}
