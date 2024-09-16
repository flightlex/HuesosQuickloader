using HuesosQuickloader.Enums;
using System;

namespace HuesosQuickloader.Models;

public sealed class DataSize
{
    public double Value { get; set; }
    public DataUnit Unit { get; set; }

    public void Clear()
    {
        Value = 0;
        Unit = DataUnit.B;
    }

    public void RoundFromBytes(long? value)
    {
        if (value is null)
            return;

        const double kb = 1024;
        const double mb = kb * 1024;
        const double gb = mb * 1024;

        if (value >= gb)
        {
            Value = Math.Round(value.Value / gb, 2);
            Unit = DataUnit.GB;
        }

        else if (value >= mb)
        {
            Value = Math.Round(value.Value / mb, 2);
            Unit = DataUnit.MB;
        }

        else if (value >= kb)
        {
            Value = Math.Round(value.Value / kb, 2);
            Unit = DataUnit.KB;
        }

        else
        {
            Value = (double)value;
            Unit = DataUnit.B;
        }
    }
}
