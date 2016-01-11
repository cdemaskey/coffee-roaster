namespace CoffeeRoaster.Enums
{
    public enum LcdCommand : uint
    {
        WakeUp = 0x30,
        FunctionSet = 0x39,
        InternalOscFrequency = 0x14,
        PowerControl = 0x56,
        FollowerControl = 0x6D,
        Contrast = 0x70,
        DisplayOn = 0x0C,
        EntryMode = 0x06,
        Clear = 0x01,
        ZeroTwo = 0x02,
        LcdFirstLine = 0x80,
        LcdSecondLine = 0xC0
    }
}
