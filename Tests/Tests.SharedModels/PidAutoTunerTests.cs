// Copyright (c) 2026 Claudio Fiorani
// All rights reserved.

using SharedModels;
using Xunit;

namespace Tests.SharedModels;

public class PidAutoTunerTests
{
    [Fact]
    public void ComputeGains_PID_ZieglerNichols()
    {
        // Ku=10, Pu=2s → Kp=6, Ti=1, Td=0.25, Ki=6, Kd=1.5
        var result = PidAutoTuner.ComputeGains(ku: 10, pu: 2, amplitude: 1, cycles: 3, pidType: "PID");

        Assert.True(result.Success);
        Assert.Equal(6.0, result.Kp);
        Assert.Equal(1.0, result.Ti);
        Assert.Equal(0.25, result.Td);
        Assert.Equal(6.0, result.Ki);   // Kp / Ti = 6 / 1 = 6
        Assert.Equal(1.5, result.Kd);   // Kp * Td = 6 * 0.25 = 1.5
    }

    [Fact]
    public void ComputeGains_PI_ZieglerNichols()
    {
        var result = PidAutoTuner.ComputeGains(ku: 10, pu: 2, amplitude: 1, cycles: 3, pidType: "PI");

        Assert.True(result.Success);
        Assert.Equal(4.5, result.Kp);             // 0.45 * 10
        Assert.Equal(Math.Round(2.0 / 1.2, 4), result.Ti);  // Pu / 1.2
        Assert.Equal(0.0, result.Td);
        Assert.True(result.Ki > 0);
        Assert.Equal(0.0, result.Kd);
    }

    [Fact]
    public void ComputeGains_P_ZieglerNichols()
    {
        var result = PidAutoTuner.ComputeGains(ku: 10, pu: 2, amplitude: 1, cycles: 3, pidType: "P");

        Assert.True(result.Success);
        Assert.Equal(5.0, result.Kp);   // 0.50 * 10
        Assert.Equal(0.0, result.Ti);
        Assert.Equal(0.0, result.Td);
        Assert.Equal(0.0, result.Ki);
        Assert.Equal(0.0, result.Kd);
    }

    [Fact]
    public void GenerateStCode_PID_ContainsGains()
    {
        var result = PidAutoTuner.ComputeGains(10, 2, 1, 3, "PID");
        var code = result.GeneratedCode;

        Assert.Contains("Kp : REAL := 6", code);
        Assert.Contains("Ki : REAL := 6", code);
        Assert.Contains("Kd : REAL := 1.5", code);
        Assert.Contains("Integral", code);
        Assert.Contains("Derivative", code);
        Assert.Contains("Write('TODO_OUTPUT_PATH'", code);
    }

    [Fact]
    public void GenerateStCode_PI_NoDerivative()
    {
        var result = PidAutoTuner.ComputeGains(10, 2, 1, 3, "PI");
        var code = result.GeneratedCode;

        Assert.Contains("Kp : REAL", code);
        Assert.Contains("Ki : REAL", code);
        Assert.DoesNotContain("Kd : REAL", code);
        Assert.Contains("Integral", code);
        Assert.DoesNotContain("Derivative", code);
    }

    [Fact]
    public void GenerateStCode_P_NoIntegralNoDerivative()
    {
        var result = PidAutoTuner.ComputeGains(10, 2, 1, 3, "P");
        var code = result.GeneratedCode;

        Assert.Contains("Kp : REAL", code);
        Assert.DoesNotContain("Ki : REAL", code);
        Assert.DoesNotContain("Kd : REAL", code);
        Assert.DoesNotContain("Integral := Integral", code);
        Assert.DoesNotContain("Derivative", code);
    }

    [Fact]
    public void RelayExperiment_ConvergesWithSimulatedProcess()
    {
        // Simulate a first-order process: dPV/dt = (-PV + K*output) / tau
        var request = new PidAutoTuner.TuneRequest
        {
            Setpoint = 50,
            RelayAmplitude = 10,
            OutputBias = 50,
            Hysteresis = 0.5,
            MinCycles = 3,
            MaxDurationSeconds = 120,
            PidType = "PID"
        };

        var experiment = new PidAutoTuner.RelayExperiment(request);

        double pv = 45; // start below setpoint
        double K = 1.0; // process gain
        double tau = 5.0; // time constant
        double dt = 0.1; // sample interval (seconds)
        double elapsed = 0;
        bool complete = false;

        for (int i = 0; i < 5000 && !complete; i++)
        {
            var (output, done) = experiment.ProcessSample(pv, elapsed);
            complete = done;

            // Simple first-order process simulation
            double dpv = (-pv + K * output) / tau;
            pv += dpv * dt;
            elapsed += dt;
        }

        var result = experiment.ComputeResult();

        Assert.True(result.Success, result.Message);
        Assert.True(result.UltimateGain > 0, "Ku should be positive");
        Assert.True(result.UltimatePeriod > 0, "Pu should be positive");
        Assert.True(result.Kp > 0, "Kp should be positive");
        Assert.True(result.Ki > 0, "Ki should be positive");
        Assert.True(result.Kd > 0, "Kd should be positive");
        Assert.NotEmpty(result.GeneratedCode);
    }

    [Fact]
    public void RelayExperiment_InsufficientOscillation_ReportsFailure()
    {
        var request = new PidAutoTuner.TuneRequest
        {
            Setpoint = 50,
            RelayAmplitude = 0.001, // too small — won't oscillate
            OutputBias = 50,
            Hysteresis = 100, // huge hysteresis — won't switch
            MinCycles = 3,
            MaxDurationSeconds = 2 // short timeout
        };

        var experiment = new PidAutoTuner.RelayExperiment(request);

        // Feed constant PV — no oscillation
        for (int i = 0; i < 100; i++)
            experiment.ProcessSample(50, i * 0.05);

        var result = experiment.ComputeResult();

        Assert.False(result.Success);
        Assert.Contains("Insufficient", result.Message);
    }
}
