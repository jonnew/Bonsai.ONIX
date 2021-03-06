﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.ONIX
{
    [Description("Triad TS4231 optical to digital converter array for V1 SteamVR base stations.")]
    public class TS4231V1Device : ONIFrameReaderDeviceBuilder<TS4231V1DataFrame>
    {
        public TS4231V1Device() : base(ONIXDevices.ID.TS4231V1ARR) { }

        public override IObservable<TS4231V1DataFrame> Process(IObservable<oni.Frame> source)
        {
            return source
                .Where(f => f.DeviceIndex() == DeviceIndex.SelectedIndex)
                .Select(f => { return new TS4231V1DataFrame(f, FrameClockHz, DataClockHz); });
        }
    }
}
