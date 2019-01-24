using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
public sealed class Script : MyGridProgram
{

    public Script()
    {

    }

    IMyTimerBlock timer;
    IMyTextPanel textPanel;
    IMyRemoteControl remCon;
    //int tickCount;
    //int clock = 10;
    bool stop;
    float gyroMult = 10;
    Vector3D testVector1 = new Vector3D(0, 1000000, 0);
    Vector3D testVector2 = new Vector3D(13369, 143970, -108901);

    public void Main(string arg)
    {
        //tickCount++;
        if (timer == null) timer = GridTerminalSystem.GetBlockWithName("Timer") as IMyTimerBlock;
        if (textPanel == null) textPanel = GridTerminalSystem.GetBlockWithName("LCD") as IMyTextPanel;
        if (remCon == null) remCon = GridTerminalSystem.GetBlockWithName("RemCon") as IMyRemoteControl;

        switch (arg)
        {
            case "start":
                {
                    stop = false; break;
                }
            case "stop":
                {
                    stop = true; break;
                }
            default: break;
        }

        GyroOverride(true, GetNavAngles(testVector1) * gyroMult, 1);
        if (!stop) timer.ApplyAction("TriggerNow");
        else GyroOverride(false, new Vector3(0, 0, 0));
    }

    Vector3D GetNavAngles(Vector3D target)
    {
        Vector3D center = remCon.GetPosition();
        Vector3D fwd = remCon.WorldMatrix.Forward;
        Vector3D up = remCon.WorldMatrix.Up;
        Vector3D left = remCon.WorldMatrix.Left;

        Vector3D targetNorm = Vector3D.Normalize(target - center);
        double targetPitch = Math.Acos(Vector3D.Dot(up, targetNorm)) - (Math.PI / 2);
        double targetYaw = Math.Acos(Vector3D.Dot(left, targetNorm)) - (Math.PI / 2);
        double targetRoll = 0;

        textPanel.WritePublicText("Yaw: " + Math.Round(targetYaw, 4) +
        "\n Pitch: " + Math.Round(targetPitch, 4) + "\n Roll: " + Math.Round(targetRoll, 4));

        return new Vector3D(targetYaw, -targetPitch, targetRoll);///////////////////////////////////////
    }

    void GyroOverride(bool isOverride, Vector3 v, float power = 1)
    {
        var gyros = new List<IMyTerminalBlock>();
        GridTerminalSystem.SearchBlocksOfName("Gyro", gyros);
        for (int i = 0; i < gyros.Count; i++)
        {
            IMyGyro gyro = gyros[i] as IMyGyro;
            if (gyro != null)
            {
                if ((!gyro.GyroOverride && isOverride) || (gyro.GyroOverride && !isOverride))
                    gyro.ApplyAction("Override");

                gyro.SetValue("Power", power);
                gyro.SetValue("Yaw", v.GetDim(0));
                gyro.SetValue("Pitch", v.GetDim(1));
                gyro.SetValue("Roll", v.GetDim(2));
            }
        }
    }

    public void Save()
    {

    }

}
