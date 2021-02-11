﻿using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.ONIX
{
    [Description("Acquires data from the twelve 14-bit analog inputs on the Open Ephys FMC Host. " +
        "Optionally, sends data to the 16-bit analog outputs on the Open Ephys FMC Host, if those " +
        "channels are selected to be outputs.")]
    public class FMCAnalogIODevice : ONIFrameReaderAndWriter<Arr, AnalogInputDataFrame>
    {
        const int NUM_CHANNELS = 12;

        enum Register
        {
            NULLPARM = 0,
            CHDIR = 1,
            CH00INRANGE = 2,
            CH01INRANGE = 3,
            CH02INRANGE = 4,
            CH03INRANGE = 5,
            CH04INRANGE = 6,
            CH05INRANGE = 7,
            CH06INRANGE = 8,
            CH07INRANGE = 9,
            CH08INRANGE = 10,
            CH09INRANGE = 11,
            CH10INRANGE = 12,
            CH11INRANGE = 13,
        }

        public enum InputOutput
        {
            Input = 0,
            Output = 1
        }

        public enum VoltageRange
        {
            [Description("+/-10.0 V")]
            TenVolts = 0,
            [Description("+/-2.5 V")]
            TwoPointFiveVolts = 1,
            [Description("+/-5.0 V")]
            FiveVolts,
        }

        void SetVoltageRange(Register channel, VoltageRange range)
        {
            WriteRegister(DeviceAddress.Address, (uint)channel, (uint)range);
        }

        VoltageRange GetVoltageRange(Register channel)
        {
            return (VoltageRange)ReadRegister(DeviceAddress.Address, (uint)channel);
        }

        uint io_reg = 0;

        void SetIO(int channel, InputOutput io)
        {
            io_reg = (io_reg & ~((uint)1 << channel)) | ((uint)(io) << channel);
            WriteRegister(DeviceAddress.Address, (uint)Register.CHDIR, io_reg);
        }
        InputOutput GetIO(int channel)
        {
            var io_reg = ReadRegister(DeviceAddress.Address, (int)Register.CHDIR);
            return (InputOutput)((io_reg >> channel) & 1);
        }

        public FMCAnalogIODevice() : base(ONIXDevices.ID.FMCANALOG1R3) { }

        protected override IObservable<AnalogInputDataFrame> Process(IObservable<oni.Frame> source)
        {
            var data_block = new AnalogInputDataBlock(NUM_CHANNELS, BlockSize);

            return source
                .Where(f =>
                {
                    return data_block.FillFromFrame(f);
                })
                .Select(f =>
                {
                    var sample = new AnalogInputDataFrame(data_block);
                    data_block = new AnalogInputDataBlock(NUM_CHANNELS, BlockSize);
                    return sample;
                });
        }

        // TODO: The order of data in the matrix is reverse of the channel index.
        // m[11] => channel 0, etc.
        protected override void Write(ONIContextTask ctx, Arr input)
        {
            var m = input.GetMat();

            // Check dims
            if (m.Rows * m.Cols != NUM_CHANNELS)
            {
                throw new IndexOutOfRangeException("Source must be a 12 element vector.");
            }

            if (m.Depth != Depth.U16)
            {
                throw new InvalidOperationException("Source elements must be unsigned 16 bit integers");
            }

            ctx.Write(DeviceAddress.Address, m.Data, 2 * NUM_CHANNELS);
        }

        [Category("Acquisition")]
        [Range(1, 10000)]
        [Description("The size of data blocks, in samples, that are propagated as events in the observable sequence.")]
        public int BlockSize { get; set; } = 250;

        [Category("Configuration")]
        [Description("The input voltage range of channel 0.")]
        public VoltageRange InputRange00
        {
            get
            {
                return GetVoltageRange(Register.CH00INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH00INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 1.")]
        public VoltageRange InputRange01
        {
            get
            {
                return GetVoltageRange(Register.CH01INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH01INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 2.")]
        public VoltageRange InputRange02
        {
            get
            {
                return GetVoltageRange(Register.CH02INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH02INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 3.")]
        public VoltageRange InputRange03
        {
            get
            {
                return GetVoltageRange(Register.CH03INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH03INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 4.")]
        public VoltageRange InputRange04
        {
            get
            {
                return GetVoltageRange(Register.CH04INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH04INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 5.")]
        public VoltageRange InputRange05
        {
            get
            {
                return GetVoltageRange(Register.CH05INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH05INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 6.")]
        public VoltageRange InputRange06
        {
            get
            {
                return GetVoltageRange(Register.CH06INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH06INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 7.")]
        public VoltageRange InputRange07
        {
            get
            {
                return GetVoltageRange(Register.CH07INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH07INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 8.")]
        public VoltageRange InputRange08
        {
            get
            {
                return GetVoltageRange(Register.CH08INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH08INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 9.")]
        public VoltageRange InputRange09
        {
            get
            {
                return GetVoltageRange(Register.CH09INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH09INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 10.")]
        public VoltageRange InputRange10
        {
            get
            {
                return GetVoltageRange(Register.CH10INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH10INRANGE, value);
            }
        }

        [Category("Configuration")]
        [Description("The input voltage range of channel 11.")]
        public VoltageRange InputRange11
        {
            get
            {
                return GetVoltageRange(Register.CH11INRANGE);
            }
            set
            {
                SetVoltageRange(Register.CH11INRANGE, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 0.")]
        public InputOutput Direction00
        {
            get
            {
                return GetIO(0);
            }
            set
            {
                SetIO(0, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 1.")]
        public InputOutput Direction01
        {
            get
            {
                return GetIO(1);
            }
            set
            {
                SetIO(1, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 2.")]
        public InputOutput Direction02
        {
            get
            {
                return GetIO(2);
            }
            set
            {
                SetIO(2, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 3.")]
        public InputOutput Direction03
        {
            get
            {
                return GetIO(3);
            }
            set
            {
                SetIO(3, value);
            }
        }


        [Category("Acquisition")]
        [Description("The direction of channel 4.")]
        public InputOutput Direction04
        {
            get
            {
                return GetIO(4);
            }
            set
            {
                SetIO(4, value);
            }
        }


        [Category("Acquisition")]
        [Description("The direction of channel 5.")]
        public InputOutput Direction05
        {
            get
            {
                return GetIO(5);
            }
            set
            {
                SetIO(5, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 6.")]
        public InputOutput Direction06
        {
            get
            {
                return GetIO(6);
            }
            set
            {
                SetIO(6, value);
            }
        }
        [Category("Acquisition")]
        [Description("The direction of channel 7.")]
        public InputOutput Direction07
        {
            get
            {
                return GetIO(7);
            }
            set
            {
                SetIO(7, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 8.")]
        public InputOutput Direction08
        {
            get
            {
                return GetIO(8);
            }
            set
            {
                SetIO(8, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 9.")]
        public InputOutput Direction09
        {
            get
            {
                return GetIO(9);
            }
            set
            {
                SetIO(9, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 10.")]
        public InputOutput Direction10
        {
            get
            {
                return GetIO(10);
            }
            set
            {
                SetIO(10, value);
            }
        }

        [Category("Acquisition")]
        [Description("The direction of channel 11.")]
        public InputOutput Direction11
        {
            get
            {
                return GetIO(11);
            }
            set
            {
                SetIO(11, value);
            }
        }
    }
}